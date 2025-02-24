﻿using ScriptBee.Domain.Service;
using ScriptBee.Ports.Driving.UseCases;

namespace ScriptBee.Calculation.Web.Extensions;

public static class CommonServicesExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        return services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}
