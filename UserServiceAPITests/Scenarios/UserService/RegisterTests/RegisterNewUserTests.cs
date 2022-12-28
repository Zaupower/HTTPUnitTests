using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.Models.Responses.UserService;
using UserServiceAPITests.Scenarios;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.RegisterTests
{
    [TestFixture]
    public class RegisterNewUserTests : UserServiceBaseTest
    {        
        [Test]
        public async Task ValidUser_RegisterNewUSer_ResponseStatusIsOk([Values(0, 1, 7, 15)] int numberOfUsers)
        {
            //Precondition
            List<CreateUserRequest> requestUsers = _generateUsersRequest.generateUsers(numberOfUsers);

            foreach (CreateUserRequest requestUser in requestUsers)
            {
                //Action
                HttpResponse<int> response = await _serviceProvider.CreateUser(requestUser);
                //Assert
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
                Assert.Greater(response.Body, 0);
                Assert.AreEqual(response.Body.ToString(), response.Content);
            }                        
        }

        [Test, Combinatorial]
        public async Task InvalidUser_RegisterNewUserFirstNameAllwaysNULL_ResponseStatusIsInternalServerError(
            [Values(null)] string firstName,
            [Values("TestCombinatorialLastName", null )] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(0, response.Body);
            Assert.AreEqual("An error occurred while saving the entity changes. See the inner exception for details.", response.Content);
        }

        [Test, Combinatorial]
        public async Task InvalidUser_RegisterNewUserLastNameAllwaysNULL_ResponseStatusIsInternalServerError(
            [Values("TestCombinatorialFirstName", null)] string firstName,
            [Values( null)] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.HttpStatusCode);
            Assert.AreEqual(0, response.Body);
            Assert.AreEqual("An error occurred while saving the entity changes. See the inner exception for details.", response.Content);
        }

        [Test]
        public async Task ValidUser_RegisterNewUserFirstNameLastNameEmpty_ResponseStatusIsOk(
            [Values("")] string firstName,
            [Values("")] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.Greater(response.Body, 0);
            Assert.AreEqual(response.Body.ToString(), response.Content);
        }

        [Test]
        public async Task ValidUser_RegisterNewUserFirstNameLastNameDigits_ResponseStatusIsOk(
            [Values("1", "124670")] string firstName,
            [Values("1", "124670")] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.Greater(response.Body, 0);
            Assert.AreEqual(response.Body.ToString(), response.Content);
        }

        [Test, Combinatorial]
        public async Task ValidUser_RegisterNewUserFirstNameLastNameSpecialCarachters_ResponseStatusIsOk(
            [Values(".", ".*", "{}_+_+)?<>:?<!@$#^%$&*()")] string firstName,
            [Values(".", ".*", "{}_+_+)?<>:?<!@$#^%$&*()")] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.Greater(response.Body, 0);
            Assert.AreEqual(response.Body.ToString(), response.Content);
        }

        [Test, Combinatorial]
        public async Task ValidUser_RegisterNewUserFirstNameLastNameLenght1_ResponseStatusIsOk(
            [Values(".", "1", "a", "A")] string firstName,
            [Values(".", "1", "a", "A")] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.Greater(response.Body, 0);
            Assert.AreEqual(response.Body.ToString(), response.Content);
        }
        [Test, Combinatorial]
        public async Task ValidUser_RegisterNewUserFirstNameLastNameLenght100Plus_ResponseStatusIsOk(
            [Values(".1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~",
                    "1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~",
                    "1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~",
                    "1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~")] string firstName,
            [Values(".1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~",
                    "1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~",
                    "1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~",
                    "1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~1qwertyuioplkjhgfdsazxcvbn!@#$%^&*()_+}{\"\\:?><~")] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.Greater(response.Body, 0);
            Assert.AreEqual(response.Body.ToString(), response.Content);
        }

        [Test, Combinatorial]
        public async Task ValidUser_RegisterNewUserFirstNameLastNameUpperCase_ResponseStatusIsOk(
            [Values("A", "QWERRTTYUIOPP", "ZXCVBNMJYTREWQQ", "PPPPPPPPPPPPPPPPPPPPPPPPPPPP")] string firstName,
            [Values("A", "QWERRTTYUIOPP", "ZXCVBNMJYTREWQQ", "PPPPPPPPPPPPPPPPPPPPPPPPPPPP")] string lastName)
        {
            //Precondition
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = firstName,
                lastName = lastName
            };

            //Action
            HttpResponse<int> response = await _serviceProvider.CreateUser(request);
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.Greater(response.Body, 0);
            Assert.AreEqual(response.Body.ToString(), response.Content);
        }
        [Test]
        public async Task ValidUser_RegisterTwoUsers_SecondIdIsFisrtAutoIncremented([Values(2)] int numberOfUsers)

        {
            //Precondition
            List<CreateUserRequest> requestUsers = _generateUsersRequest.generateUsers(numberOfUsers);
            List<int> ids = new List<int>();

            foreach (CreateUserRequest requestUser in requestUsers)
            {
                //Action
                HttpResponse<int> response = await _serviceProvider.CreateUser(requestUser);
                ids.Add(response.Body);             
            }

            int firstUserId = ids[0];
            int secondUserId = ids[1];
            //Assert  
            Assert.AreEqual(secondUserId,firstUserId+1);
        }
    }
}
