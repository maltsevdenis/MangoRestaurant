using Mango.Services.OrderAPI.Repository;
using Mango.Services.ShoppingCartAPI.CQRS.Commands;

using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Handlers;

public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;

    public RemoveFromCartCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<bool> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        return await _cartRepository.RemoveFromCart(request.CartId);
    }
}
