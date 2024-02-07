namespace VendingMachineApi.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Cost { get; set; }
        public int AmountAvailable { get; set; }
    }
}
