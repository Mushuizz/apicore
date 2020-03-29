using Api.Core.AOP;
using Api.Core.Auth;
using Api.Core.Common.Auth;
using Api.Core.Common.Cache;
using Api.Core.Common.DB;
using Api.Core.Common.Helper;
using Api.Core.Filter;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Api.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration
            , IWebHostEnvironment env)
        {
            Env = env;
            Configuration = configuration;
        }

        private IWebHostEnvironment Env;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Appsetting(Env.ContentRootPath));
            #region cache             
            services.AddMemoryCache();
            services.AddSingleton<IMemoryCaching, MemoryCaching>();
            services.AddSingleton<IRedisCache, RedisCache>();
            #endregion

            #region Cors 跨域
            services.AddCors(c =>
            {
                c.AddPolicy("Cors", policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            #endregion

            #region Swagger
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "zsm.api", Version = "v1" });

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {new OpenApiSecurityScheme{Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }}, new List<string>()}
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });
            #endregion

            #region Jwt
            var AuthSettingsSection = Configuration.GetSection("AuthSetting");
            services.Configure<AuthSetting>(AuthSettingsSection);

            var AuthSettings = AuthSettingsSection.Get<AuthSetting>();
            var key = Encoding.ASCII.GetBytes(AuthSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
           .AddJwtBearer(x =>
           {
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidIssuer = AuthSettings.Issuer,

                   ValidateAudience = true,
                   ValidAudience = AuthSettings.Audience,

                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),

                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.FromSeconds(30),
                   RequireExpirationTime = true
               };
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
           });

            services.AddScoped<IAuthServer, AuthServer>();

            #endregion

            #region SqlSugar
            services.AddScoped<ISqlSugarClient>(o =>
            {
                var listConfig = new List<ConnectionConfig>();
                listConfig.Add(new ConnectionConfig()
                {
                    ConnectionString = DbSetting.connectionStrings,
                    DbType = (DbType)DbSetting.Dbtype,
                    IsAutoCloseConnection = true,
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true
                    }
                });
                return new SqlSugarClient(listConfig);
            });
            # endregion
            services.AddAutoMapper(typeof(Startup));

            services.AddControllers(option =>
            {
                option.Filters.Add<GlobalExceptionsFilter>();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;

            #region 服务注入
            var servicesDllFile = Path.Combine(basePath, "Api.Core.Services.dll");
            var repositoryDllFile = Path.Combine(basePath, "Api.Core.Repository.dll");
            if (!(File.Exists(servicesDllFile) && File.Exists(repositoryDllFile)))
            {
                throw new Exception("Repository.dll和service.dll 丢失，因为项目解耦了，所以需要先F6编译，再F5运行，请检查 bin 文件夹，并拷贝。");
            }

            var cacheType = new List<Type>();
            if (Configuration["AppSettings:TranAOP:Enabled"].Equals("true"))
            {
                builder.RegisterType<TransactionInterceptor>();
                cacheType.Add(typeof(TransactionInterceptor));
            }
            if (Configuration["AppSettings:RedisCacheAOP:Enabled"].Equals("true"))
            {
                builder.RegisterType<CacheInterceptor>();
                cacheType.Add(typeof(CacheInterceptor));
            }
            // 获取 Service.dll 程序集服务，并注册
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);
            builder.RegisterAssemblyTypes(assemblysServices)
                      .AsImplementedInterfaces()
                      .InstancePerDependency()
                      .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                      .InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。

            // 获取 Repository.dll 程序集服务，并注册
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository)
                   .AsImplementedInterfaces()
                   .InstancePerDependency();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "api.core");
            });

            app.UseCors("Cors");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            // 先开启认证
            app.UseAuthentication();
            // 然后是授权中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
