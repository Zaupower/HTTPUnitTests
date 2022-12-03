using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests;

namespace UserServiceAPITests.Helper
{
    public class GenerateUsersRequest
    {
        public List<CreateUserRequest> generateUsers(int numberOfUSers)
        {
            List<CreateUserRequest> users = new List<CreateUserRequest>();
            for (int i = 0; i < numberOfUSers; i++)
            {
                users.Add(new CreateUserRequest
                {
                    firstName = "fisrt_name_test" + (i + 1),
                    lastName = "last_name_test" + (i + 1)
                });
            }

            return users;
        }
    }
}
