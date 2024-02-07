using Microsoft.AspNetCore.Identity;

namespace VendingMachineApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Deposit { get; set; }
    }
}
