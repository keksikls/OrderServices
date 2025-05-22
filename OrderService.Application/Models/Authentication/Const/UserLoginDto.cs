namespace OrderService.Application.Models.Authentication.Const;

public record UserLoginDto(string Username, string Password, string Email,string Phone);