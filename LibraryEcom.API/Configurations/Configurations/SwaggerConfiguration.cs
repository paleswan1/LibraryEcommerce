﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace LibraryEcom.Configurations;

public static class SwaggerConfiguration
{
    private static readonly string[] value = new string[] { };

    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
    {
        return services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LibraryEcom - Connect Web API",
                Version = "v1",
                Contact = new OpenApiContact()
                {
                    Name = "LibraryEcom",
                    Email = "siddhartha.affinity@gmail.com",
                    Url = new Uri("https://www.affinity.com")
                },
                Description = "LibraryEcom - Training Portal Web API",
                License = new OpenApiLicense
                {
                    Name = "LibrayEcom",
                    Url = new Uri("https://www.affinity.com")
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by a space and then your valid JWT token."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }, value }
            });

            c.MapType<DateOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date"
            });

            c.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("00:00:00")
            });
        });
    }
}