namespace VendingMachineApi.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Cost { get; set; }
        public int AmountAvailable { get; set; }
        //public string SellerId { get; set; }
    }
}
