using VendingMachineApi.Dtos;

namespace VendingMachineApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterUserAsync(RegisterModel registerModel);
        Task<AuthModel> LoginUserAsync(LoginModel loginModel);
    }
}
