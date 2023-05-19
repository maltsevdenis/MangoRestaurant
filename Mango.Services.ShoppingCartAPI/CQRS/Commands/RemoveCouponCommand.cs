
using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Commands;

public class RemoveCouponCommand : IRequest<bool>
{
    public string UserId { get; set; }
}
