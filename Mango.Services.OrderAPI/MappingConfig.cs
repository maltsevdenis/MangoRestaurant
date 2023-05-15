using AutoMapper;

namespace Mango.Services.OrderAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            //config.CreateMap<OrderHeaderDto, OrderHeader>().ReverseMap();
            //config.CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();
        });

        return mappingConfig;
    }
}
