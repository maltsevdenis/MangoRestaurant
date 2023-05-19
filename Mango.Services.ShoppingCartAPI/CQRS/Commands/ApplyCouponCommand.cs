
using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Commands;

public class ApplyCouponCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public string CouponCode { get; set; }
}
