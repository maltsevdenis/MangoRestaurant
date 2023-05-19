using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Repository;
using Mango.Services.ShoppingCartAPI.CQRS.Commands;

using MediatR;

namespace Mango.Services.ShoppingCartAPI.CQRS.Handlers;

public class CreateUpdateCartCommandHandler : IRequestHandler<CreateUpdateCartCommand, CartDto>
{
    private readonly ICartRepository _cartRepository;

    public CreateUpdateCartCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto> Handle(CreateUpdateCartCommand request, CancellationToken cancellationToken)
    {
        return await _cartRepository.CreateUpdateCart(request.Cart);
    }
}
