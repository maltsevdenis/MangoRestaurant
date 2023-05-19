using MediatR;

namespace Mango.Services.ProductAPI.CQRS.Commands;

public class DeleteProductCommand : IRequest<bool>
{
    public int ProductId { get; set; }
}
