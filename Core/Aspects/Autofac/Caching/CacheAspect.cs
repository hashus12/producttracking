using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Caching;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Core.Utilities.Results;

namespace Core.Aspects.Autofac.Caching
{
    public class CacheAspect : MethodInterception
    {
        private readonly int _duration;
        private readonly ICacheManager _cacheManager;
        public CacheAspect(int duration = 60)
        {
            _duration = duration;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        public override void Intercept(IInvocation invocation)
        {
            var methodName = string.Format($"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}");
            var arguments = invocation.Arguments.ToList();
            var key = $"{methodName}({string.Join(",", arguments.Select(x => x?.ToString() ?? "<Null>"))})";
            if (_cacheManager.IsAdd(key))
            {
                var returnType = invocation.Method.ReturnType.GenericTypeArguments[0].GenericTypeArguments[0];
                var generatedList = typeof(List<>).MakeGenericType(returnType);
                var cacheData = _cacheManager.Get(key);
                var instanceResult = Activator.CreateInstance(typeof(SuccessDataResult<>).MakeGenericType(generatedList),
                    JsonSerializer.Deserialize(cacheData.ToString(), generatedList));
                invocation.ReturnValue = instanceResult;
                return;
            }
            invocation.Proceed();
            var returnValue = JsonSerializer.Serialize(invocation.ReturnValue.GetType().GetProperty("Data").GetValue
                (invocation.ReturnValue, null));
            _cacheManager.Add(key, returnValue, _duration);
        }


      

    }
}
