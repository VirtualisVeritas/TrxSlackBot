namespace TrxSlackBot
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                await TrxSlackDeserializer.SendSlackMessage();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}