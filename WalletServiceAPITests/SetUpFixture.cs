using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            var provider = WalletServiceServiceProvider.Instance;

            await provider.ReverseAllTransactions();
        }
    }
}
