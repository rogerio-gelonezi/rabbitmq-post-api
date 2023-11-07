using MessagePublisher.WebApi.Filters;
using MessagePublisher.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MessagePublisher.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddErrorFilter(this IServiceCollection services)
    {
        services.AddMvc(options => options.Filters.Add(typeof(ErrorResponseFilter)))
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(ErrorResponseMapper.FromModelState(context.ModelState));
            });
    }
    
    public static void AddNewtonsoftJson(this IServiceCollection services)
    {
        services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
    }
}