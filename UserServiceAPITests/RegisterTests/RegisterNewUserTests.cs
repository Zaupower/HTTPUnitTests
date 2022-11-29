using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models;

namespace UserServiceAPITests.RegisterTests
{
    
    public class RegisterNewUserTests
    {
        private readonly string _baseUrl = "https://userservice-uat.azurewebsites.net";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CanRegister_NewUSer()
        {
            HttpClient client = new HttpClient();

            CreateUserRequest body = new CreateUserRequest
            {
                firstName = "fisrt_name_test1",
                lastName = "last_name_test1"
            };
            
            string serializedBody = JsonConvert.SerializeObject(body);

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Register/RegisterNewUser"),
                Content = new StringContent(serializedBody)

            };

            HttpResponseMessage response = client.SendAsync(request).Result;

            Assert.Pass();
        }
    }
}
