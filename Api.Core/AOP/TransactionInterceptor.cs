using Api.Core.Common.Attributes;
using Api.Core.IRepository.IUnitOfWork;
using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.AOP
{
    public class TransactionInterceptor : IInterceptor
    {
        public TransactionInterceptor(ISugarUnitOfWork sugarUnitOfWork)
        {
            _sugarUnitOfWork = sugarUnitOfWork;
        }

        public ISugarUnitOfWork _sugarUnitOfWork;

        public async void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            bool IsTrans = method.GetCustomAttributes(true).Any(a => a.GetType() == typeof(TransactionAttribute));
            if (IsTrans)
            {
                try
                {
                    _sugarUnitOfWork.BeginTran();
                    invocation.Proceed();
                    if (invocation.Method.ReturnType == typeof(Task))
                    {
                        await (Task)invocation.ReturnValue;
                    }
                    _sugarUnitOfWork.CommitTran();
                }
                catch (Exception)
                {
                    _sugarUnitOfWork.RollbackTran();
                    throw;
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
