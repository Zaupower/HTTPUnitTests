using CommonLogic.Models.Responses.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests.UserService;

namespace UserServiceAPITests
{
    public class DataContext
    {
        public string UserId { get; set; }
        public HttpResponse<int> CreateUserResponse { get; set; }
        public CreateUserRequest CreateUserRequest{ get; set; }

    }
}
