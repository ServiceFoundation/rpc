﻿using Microsoft.Extensions.DependencyInjection;
using System;
using Tars.Net.Hosting;

namespace Tars.Net
{
    public static class ServerHostBuilderExtensions
    {
        public static IServerHostBuilder ConfigureServices(this IServerHostBuilder builder, Action<IServiceCollection> configure)
        {
            configure?.Invoke(builder.Services);
            return builder;
        }
    }
}