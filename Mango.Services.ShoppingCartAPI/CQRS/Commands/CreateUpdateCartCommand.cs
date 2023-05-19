using Mango.Services.OrderAPI.Models.Dto;

using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Commands;

public class CreateUpdateCartCommand : IRequest<CartDto>
{
    public CartDto Cart { get; set; }
}
