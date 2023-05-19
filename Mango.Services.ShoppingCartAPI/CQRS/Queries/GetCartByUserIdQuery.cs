using Mango.Services.OrderAPI.Models.Dto;

using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Queries;

public class GetCartByUserIdQuery : IRequest<CartDto>
{
    public string UsertId { get; set; }
}
