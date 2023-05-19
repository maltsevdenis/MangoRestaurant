using Mango.Services.ProductAPI.Models.Dto;

using MediatR;

namespace Mango.Services.ProductAPI.CQRS.Commands;

public class CreateUpdateProductCommand : IRequest<ProductDto>
{
    public ProductDto Product { get; set; }
}
