using Mango.Services.ProductAPI.CQRS.Queries;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository;

using MediatR;

namespace Mango.Services.ProductAPI.CQRS.Handlers;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _productRepository.GetProducts();
    }
}
