namespace UserServiceAPITests.Models.Responses.WalletService
{
    public class GetTransactionModel
    {
        public int userId { get; set; }
        public double amount { get; set; }
        public string transactionId { get; set; }
        public DateTime time { get; set; }
        public int status { get; set; }
        public string baseTransactionId { get; set; }
    }
}
