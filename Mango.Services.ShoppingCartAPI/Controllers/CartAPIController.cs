using Mango.MessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Repository;
using Mango.Services.ShoppingCartAPI.CQRS.Commands;
using Mango.Services.ShoppingCartAPI.CQRS.Queries;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.OrderAPI.Controllers;

[ApiController]
[Route("api/cart")]
public class CartAPIController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICouponRepository _couponRepository;
    private readonly IMessageBus _messageBus;
    protected ResponseDto _response;

    public CartAPIController(IMediator mediator, IMessageBus messageBus, ICouponRepository couponRepository)
    {
        _mediator = mediator;
        _couponRepository = couponRepository;
        _messageBus = messageBus;
        _response = new ResponseDto();
    }

    [HttpGet("GetCart/{userId}")]
    public async Task<object> GetCart(string userId)
    {
        try
        {
            CartDto cartDto = await _mediator.Send(new GetCartByUserIdQuery { UsertId = userId });
            _response.Result = cartDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }
        return _response;
    }

    [HttpPost("AddCart")]
    public async Task<object> AddCart(CartDto cartDto)
    {
        try
        {
            CartDto cartDt = await _mediator.Send(new CreateUpdateCartCommand { Cart = cartDto });
            _response.Result = cartDt;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }
        return _response;
    }

    [HttpPost("UpdateCart")]
    public async Task<object> UpdateCart(CartDto cartDto)
    {
        try
        {
            CartDto cartDt = await _mediator.Send(new CreateUpdateCartCommand { Cart = cartDto });
            _response.Result = cartDt;

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [HttpPost("RemoveCart")]
    public async Task<object> RemoveCart([FromBody] int cartId)
    {
        try
        {
            bool isSuccess = await _mediator.Send(new RemoveFromCartCommand { CartId = cartId });
            _response.Result = isSuccess;

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [HttpPost("ApplyCoupon")]
    public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
    {
        try
        {
            bool isSuccess = await _mediator.Send(new ApplyCouponCommand { UserId = cartDto.CartHeader.UserId, CouponCode = cartDto.CartHeader.CouponCode });
            _response.Result = isSuccess;

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [HttpPost("RemoveCoupon")]
    public async Task<object> RemoveCoupon([FromBody] string userId)
    {
        try
        {
            bool isSuccess = await _mediator.Send(new RemoveCouponCommand { UserId = userId});
            _response.Result = isSuccess;

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [HttpPost("Checkout")]
    public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
    {
        try
        {
            CartDto cartDto = await _mediator.Send(new GetCartByUserIdQuery { UsertId = checkoutHeader.UserId });

            if (cartDto == null)
            {
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
            {
                CouponDto coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                if (checkoutHeader.DiscountTotal != coupon.DiscountAmount)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Coupon Price has changed, please confirm" };
                    _response.DisplayMessage = "Coupon Price has changed, please confirm";
                    return _response;
                }
            }

            checkoutHeader.CartDetails = cartDto.CartDetails;

            // logic to add message to process order.
            await _messageBus.PublishMessage(checkoutHeader, "checkoutqueue");
            await _mediator.Send(new ClearCartCommand { UserId = checkoutHeader.UserId });
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
}
