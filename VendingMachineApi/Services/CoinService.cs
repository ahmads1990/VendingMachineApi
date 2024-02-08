namespace VendingMachineApi.Services
{
    public class CoinService : ICoinService
    {
        public string GetCoinValuesString(int amount)
        {
            int[] coins = { 100, 50, 20, 10, 5 };
            Dictionary<int, int> result = new Dictionary<int, int>();

            foreach (int coin in coins)
            {
                // Calculate the number of coins for the current denomination
                int count = amount / coin;

                // Update the result dictionary if the count is greater than 0
                if (count > 0)
                {
                    result[coin] = count;
                }

                // Update the remaining amount
                amount %= coin;
            }

            // Build the result string
            string resultString = $"Amount: {amount} here are your coints:\n";
            foreach (var coin in result)
            {
                resultString += $"{coin.Value} coins of {coin.Key}\n";
            }

            return resultString.Trim();
        }
    }
}
