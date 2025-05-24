using OrderService.Application.Models.Authentication;
using OrderService.Application.Models.Authentication.Const;

namespace OrderService.Application.Abstractions;

public interface IAuthService
{
    Task<UserResponse> Register(UserRegisterDto userRegisterModel);
    Task<UserResponse> Login(UserLoginDto userLoginDto);
}