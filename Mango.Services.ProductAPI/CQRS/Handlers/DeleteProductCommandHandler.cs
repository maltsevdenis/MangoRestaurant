using Mango.Services.ProductAPI.CQRS.Commands;
using Mango.Services.ProductAPI.Repository;

using MediatR;

namespace Mango.Services.ProductAPI.CQRS.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            return await _productRepository.DeleteProduct(request.ProductId);
        }
    }
}
