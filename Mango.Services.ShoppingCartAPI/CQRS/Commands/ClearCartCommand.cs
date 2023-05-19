using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Commands;

public class ClearCartCommand : IRequest<bool>
{
    public string UserId { get; set; }
}
