using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceAPITests.Models.Requests.WalletService
{
    public class ChargeModel
    {
        public int userId { get; set; }
        public double amount { get; set; }
    }
}
