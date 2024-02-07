namespace VendingMachineApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthModel> RegisterUserAsync(RegisterModel registerModel);
        Task<AuthModel> LoginUserAsync(LoginModel loginModel);
        Task<bool> CheckHaveEnoughDeposit(string userId, int deposit);
        Task<UpdateDepositModel> BuyerAddToDepositAsync(UpdateDepositModel updateDepositModel);
        Task<UpdateDepositModel> BuyerResetDepositAsync(UpdateDepositModel updateDepositModel);
    }
}
