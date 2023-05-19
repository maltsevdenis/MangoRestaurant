using Mango.Services.OrderAPI.Repository;
using Mango.Services.ShoppingCartAPI.CQRS.Commands;

using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Handlers;


public class RemoveCouponCommandHandler : IRequestHandler<RemoveCouponCommand, bool>
{
    private readonly ICartRepository _cartRepository;

    public RemoveCouponCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<bool> Handle(RemoveCouponCommand request, CancellationToken cancellationToken)
    {
        return await _cartRepository.RemoveCoupon(request.UserId);
    }
}
