using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceAPITests.Models.Requests.UserService
{
    public class SetUserStatusModel
    {
        public int UserId { get; set; }
        public bool NewStatus { get; set; }
    }
}
