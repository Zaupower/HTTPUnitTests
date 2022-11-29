using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Extensions;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Responses;
using UserServiceAPITests.Models.Responses.Base;

namespace UserServiceAPITests
{
    public class UserServiceServiceProvider
    {
        private readonly string _baseUrl = "https://userservice-uat.azurewebsites.net";
        private HttpClient httpClient = new HttpClient();

        public async Task<HttpResponse<GetUserResponse>> CreateUser(CreateUserRequest request)
        {
            

            string serializedBody = JsonConvert.SerializeObject(request);

            HttpRequestMessage createUserrequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Register/RegisterNewUser"),
                Content = new StringContent(serializedBody, Encoding.UTF8, "application/json")

            };
            HttpResponseMessage response = await httpClient.SendAsync(createUserrequest);

            return await response.ToCommonResponse<GetUserResponse>();
        }

        public async Task<HttpResponse<object>> DeleteUser(int userId)
        {
            HttpRequestMessage createUserrequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_baseUrl}/DeleteUser?userId={userId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(createUserrequest);
            return await response.ToCommonResponse<object>();
        }
    }
}
