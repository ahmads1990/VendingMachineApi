namespace VendingMachineApi.Dtos
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserTypeClaim { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
