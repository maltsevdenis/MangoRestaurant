using Mango.Services.OrderAPI.Repository;
using Mango.Services.ShoppingCartAPI.CQRS.Commands;

using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Handlers;

public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, bool>
{
    private readonly ICartRepository _cartRepository;

    public ApplyCouponCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<bool> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
    {
        return await _cartRepository.ApplyCoupon(request.UserId, request.CouponCode);
    }
}
