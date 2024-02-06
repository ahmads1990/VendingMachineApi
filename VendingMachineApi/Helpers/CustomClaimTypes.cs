namespace VendingMachineApi.Helpers
{
    public static class CustomClaimTypes
    {
        public static readonly string ISSELLER = "Seller";
        public static readonly string ISBUYER = "Buyer";
        public static readonly List<string> ALLOWEDTYPES = new List<string> { ISSELLER, ISBUYER };
    }
}
