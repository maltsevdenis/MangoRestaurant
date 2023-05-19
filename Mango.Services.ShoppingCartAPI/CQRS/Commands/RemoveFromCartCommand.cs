using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Commands
{
    public class RemoveFromCartCommand : IRequest<bool>
    {
        public int CartId { get; set; }
    }
}
