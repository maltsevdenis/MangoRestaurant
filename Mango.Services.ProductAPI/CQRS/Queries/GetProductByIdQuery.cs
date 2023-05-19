using Mango.Services.ProductAPI.Models.Dto;

using MediatR;

namespace Mango.Services.ProductAPI.CQRS.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public int ProductId { get; set; }
    }
}
