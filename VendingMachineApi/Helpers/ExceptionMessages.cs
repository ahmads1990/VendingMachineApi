namespace VendingMachineApi.Helpers
{
    public static class ExceptionMessages
    {
        // Message for invalid entity data, which maybe fields are null or bad format.
        public static string InvalidEntityData = "Invalid entity data, check format and value for required fields.";
        // Message for an invalid entity ID (create -> id=0, update/del -> +ve id>0).
        public static string InvalidEntitytId = "Invalid ProductId, wanted format (create -> id=0, update/del -> id>0).";
        // Message for when an entity doesn't exist for update/delete operations.
        public static string EntityDoesntExist = "Check ID, can't find entity to update/delete.";
        public static string InvalidProductCostOrAmount = "Invalid product cost or amount, amount should be >0,cost should be >0 and divisble by 5 only accepting 5,10,15,20,50,100 coins";
        public static string UnAuthorizedSeller = "You cant update other sellers products";

        // for controller
        public static string OnlySellerUser = "Only seller user are allowed to add/update/delete products";
        public static string OnlyBuyerUser = "Only buyer user are allowed to purchase products";
    }
}
