using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Repository;
using Mango.Services.ShoppingCartAPI.CQRS.Queries;

using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Handlers;

public class GetChartByUserIdQueryHandler : IRequestHandler<GetCartByUserIdQuery, CartDto>
{
    private readonly ICartRepository _cartRepository;

    public GetChartByUserIdQueryHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _cartRepository.GetCartByUserId(request.UsertId);
    }
}
