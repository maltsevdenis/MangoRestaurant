﻿namespace Mango.Services.ProductAPI.Models.Dto;

public class ResposeDto
{
    public bool IsSuccess { get; set; } = true;
    public object Result { get; set; }
    public string DisplayMessage { get; set; } = "";
    public List<string> ErrorMessages { get; set; }
}