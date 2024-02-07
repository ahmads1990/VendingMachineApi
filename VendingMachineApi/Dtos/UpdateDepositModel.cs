using System.ComponentModel.DataAnnotations;

namespace VendingMachineApi.Dtos
{
    public class UpdateDepositModel
    {
        [Required]
        public int Deposit { get; set; }
        public string BuyerId { get; set; }
        public string Message { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
