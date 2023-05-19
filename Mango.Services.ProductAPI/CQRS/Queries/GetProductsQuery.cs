using Mango.Services.ProductAPI.Models.Dto;

using MediatR;

namespace Mango.Services.ProductAPI.CQRS.Queries;

public class GetProductsQuery : IRequest<IEnumerable<ProductDto>>
{
}
