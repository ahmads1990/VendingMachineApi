namespace VendingMachineApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthModel> RegisterUserAsync(RegisterModel registerModel);
        Task<AuthModel> LoginUserAsync(LoginModel loginModel);
    }
}
