using NUnit.Framework;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            var provider = WalletServiceServiceProvider.Instance;

            //await provider.ReverseAllTransactions();
        }
    }
}
