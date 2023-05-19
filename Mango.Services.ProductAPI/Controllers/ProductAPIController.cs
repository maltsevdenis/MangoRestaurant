using Mango.Services.ProductAPI.CQRS.Commands;
using Mango.Services.ProductAPI.CQRS.Queries;
using Mango.Services.ProductAPI.Models.Dto;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[Route("api/products")]
public class ProductAPIController : ControllerBase
{
    private IMediator _mediator;
    protected ResposeDto _response;

    public ProductAPIController(IMediator mediator)
    {
        _response = new ResposeDto();
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<object> Get()
    {
        try
        {
            IEnumerable<ProductDto> productsDtos = await _mediator.Send(new GetProductsQuery());
            _response.Result = productsDtos; 
        }
        catch (Exception ex) 
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<object> Get(int id)
    {
        try
        {
            ProductDto productDto = await _mediator.Send(new GetProductByIdQuery { ProductId = id});
            _response.Result = productDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpPost]
    [Authorize]
    public async Task<object> Post([FromBody] ProductDto productDto)
    {
        try
        {
            ProductDto model = await _mediator.Send(new CreateUpdateProductCommand { Product = productDto });
            _response.Result = model;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpPut]
    public async Task<object> Put([FromBody] ProductDto productDto)
    {
        try
        {
            ProductDto model = await _mediator.Send(new CreateUpdateProductCommand { Product = productDto });
            _response.Result = model;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }


    [HttpDelete]
    [Authorize(Roles ="Admin")]
    [Route("{id}")]
    public async Task<object> Delete(int id)
    {
        try
        {
            bool isSuccess = await _mediator.Send(new DeleteProductCommand { ProductId = id });
            _response.Result = isSuccess;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }
}
