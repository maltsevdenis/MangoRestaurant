using Mango.Services.OrderAPI.Repository;
using Mango.Services.ShoppingCartAPI.CQRS.Commands;

using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Handlers;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;

    public ClearCartCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        return await _cartRepository.ClearCart(request.UserId);
    }
}
