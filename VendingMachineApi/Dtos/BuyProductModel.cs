namespace VendingMachineApi.Dtos
{
    public class BuyProductModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Message { get; set; }
    }
}
