﻿using Mango.Web.Models;

namespace Mango.Web.Services.IServices;

public interface ICartService
{
    Task<T> GetCartByUserIdAsync<T>(string userId, string token=null);
    Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null);
    Task<T> UpdateToCartAsync<T>(CartDto cartDto, string token = null);
    Task<T> RemoveFromCartAsync<T>(int cartId, string token = null);
    Task<T> ApplyCoupon<T>(CartDto cartDto, string token = null);
    Task<T> RemoveCoupon<T>(string userId, string token = null);
    Task<T> Checkout<T>(CartHeaderDto cartHeader, string token = null);
}
