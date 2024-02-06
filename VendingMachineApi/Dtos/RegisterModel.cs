using System.ComponentModel.DataAnnotations;

namespace VendingMachineApi.Dtos
{
    public class RegisterModel
    {
        /* Used for Register process has {UserName, Email, Password, UserTypeClaim} */
        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string UserTypeClaim { get; set; }
    }
}
