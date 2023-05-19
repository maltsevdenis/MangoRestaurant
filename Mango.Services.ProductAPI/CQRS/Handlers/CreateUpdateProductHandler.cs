using Mango.Services.ProductAPI.CQRS.Commands;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository;

using MediatR;

namespace Mango.Services.ProductAPI.CQRS.Handlers
{
    public class CreateUpdateProductHandler : IRequestHandler<CreateUpdateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;

        public CreateUpdateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> Handle(CreateUpdateProductCommand request, CancellationToken cancellationToken)
        {
            return await _productRepository.CreateUpdateProduct(request.Product);
        }
    }
}
