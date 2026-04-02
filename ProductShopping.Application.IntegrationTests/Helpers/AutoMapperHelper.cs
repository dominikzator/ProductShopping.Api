using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductShopping.Application.MappingProfiles;

namespace ProductShopping.Application.IntegrationTests.Helpers;

public class AutoMapperHelper
{
    public static IMapper Create()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });

        var expression = new MapperConfigurationExpression();
        expression.AddProfile<ProductMappingProfile>();

        var mapperConfig = new MapperConfiguration(expression, loggerFactory);

        return mapperConfig.CreateMapper();
    }
}
