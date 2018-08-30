﻿using System;
using System.Reflection;
using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Tars.Net.Hosting;

namespace Tars.Net
{
    public static class HostBuilderExtensions
    {
        public static IServerHostBuilder ReigsterRpcServices(this IServerHostBuilder builder, params Assembly[] assemblies)
        {
            var services = builder.Services;
            foreach (var (service, implementation) in builder.RpcServices)
            {
                services.TryAddSingleton(service.GetReflector().GetMemberInfo().AsType(), implementation.GetReflector().GetMemberInfo().AsType());
            }

            services.TryAddSingleton<IServerInvoker, ServerInvoker>();
            services.TryAddSingleton<IServerHandler, ServerHandler>();
            services.TryAddSingleton(builder.RpcServices);
            return builder;
        }

        public static IServerHostBuilder ConfigureLog(this IServerHostBuilder builder, Action<ILoggingBuilder> configure)
        {
            return builder.ConfigureServices(i => i.AddLogging(configure));
        }

        public static IServerHostBuilder ReigsterRpcClients(this IServerHostBuilder builder, params Assembly[] assemblies)
        {
            builder.ReigsterRpcClients();
            return builder;
        }
    }
}