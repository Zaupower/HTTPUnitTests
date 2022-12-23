using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.Helper
{
    public class GenerateUsersRequest
    {
        private static Lazy<GenerateUsersRequest> _instance = new Lazy<GenerateUsersRequest>(() => new GenerateUsersRequest());

        public static GenerateUsersRequest Instance => _instance.Value;

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

        public CreateUserRequest generateUser()
        {
            CreateUserRequest user;

            user = new CreateUserRequest
            {
                firstName = "fisrt_name_test",
                lastName = "last_name_test"
            };            

            return user;
        }
    }
}
