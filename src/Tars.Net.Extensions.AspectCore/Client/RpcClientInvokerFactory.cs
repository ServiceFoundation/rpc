﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using Tars.Net.Attributes;
using Tars.Net.Clients;

namespace Tars.Net.Extensions.AspectCore
{
    public class RpcClientInvokerFactory
    {
        private readonly IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> invokers;
        private readonly IRpcClientFactory clientFactory;

        public RpcClientInvokerFactory(IEnumerable<Type> rpcClients, IRpcClientFactory clientFactory)
        {
            invokers = CreateRpcClientInvokers(rpcClients);
            this.clientFactory = clientFactory;
        }

        public IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> CreateRpcClientInvokers(IEnumerable<Type> rpcClients)
        {
            var dictionary = new Dictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>>();
            foreach (var item in rpcClients)
            {
                var attribute = item.GetCustomAttribute<RpcAttribute>();
                foreach (var method in item.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    var isOneway = method.GetReflector().IsDefined<OnewayAttribute>();
                    var outParameters = method.GetParameters().Where(i => i.IsOut).ToArray();
                    dictionary.Add(method, async (context, next) =>
                    {
                        var value = await clientFactory.SendAsync(attribute.ServantName, method.Name, outParameters, method.ReturnParameter, isOneway, attribute.Codec, context.Parameters);
                        context.ReturnValue = value;
                    });
                }
            }
            return dictionary;
        }

        public Func<AspectContext, AspectDelegate, Task> GetClientInvoker(MethodInfo method)
        {
            return invokers[method];
        }
    }
}