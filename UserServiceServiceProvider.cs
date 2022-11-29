using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Responses.Base;

namespace UserServiceAPITests
{
    public class UserServiceServiceProvider
    {
        private readonly string _baseUrl = "https://userservice-uat.azurewebsites.net";
        private HttpClient httpClient = new HttpClient();

        public async Task<HttpResponse<string>> CreateUser(CreateUserRequest request)
        {
            

            string serializedBody = JsonConvert.SerializeObject(request);

            HttpRequestMessage createUserrequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Register/RegisterNewUser"),
                Content = new StringContent(serializedBody, Encoding.UTF8, "application/json")

            };
            HttpResponseMessage response = await httpClient.SendAsync(createUserrequest);
            string responseContent = await response.Content.ReadAsStringAsync();

            return new HttpResponse<string> 
            {
              HttpStatusCode = response.StatusCode,
              Body = responseContent
            };
        }

        public async Task<HttpResponse<string>> DeleteUser(int userId)
        {
            HttpRequestMessage createUserrequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_baseUrl}/DeleteUser?userId={userId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(createUserrequest);
            string responseContent = await response.Content.ReadAsStringAsync();

            return new HttpResponse<string>
            {
                HttpStatusCode = response.StatusCode,
                Body = responseContent
            };
        }
    }
}
