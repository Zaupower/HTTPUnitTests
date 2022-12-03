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

namespace UserServiceAPITests.ServiceProvider
{
    public class UserServiceServiceProvider
    {
        private readonly string _baseUrl = "https://userservice-uat.azurewebsites.net";
        private HttpClient httpClient = new HttpClient();

        public async Task<HttpResponse<int>> CreateUser(CreateUserRequest request)
        {
            string serializedBody = JsonConvert.SerializeObject(request);

            HttpRequestMessage createUserrequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Register/RegisterNewUser"),
                Content = new StringContent(serializedBody, Encoding.UTF8, "application/json")

            };
            HttpResponseMessage response = await httpClient.SendAsync(createUserrequest);

            return await response.ToCommonResponse<int>();
        }

        public async Task<HttpResponse<object>> DeleteUser(int userId)
        {
            HttpRequestMessage createUserrequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_baseUrl}/Register/DeleteUser?userId={userId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(createUserrequest);
            return await response.ToCommonResponse<object>();
        }

        public async Task<HttpResponse<object>> SetUserStatus(SetUserStatusModel userStatus)
        {
            HttpRequestMessage setUserStatusRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_baseUrl}/UserManagement/SetUserStatus?userId={userStatus.UserId}&newStatus={userStatus.NewStatus}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(setUserStatusRequest);
            return await response.ToCommonResponse<object>();
        }

        public async Task<HttpResponse<object>> GetUserStatus(int userId)
        {
            HttpRequestMessage setUserStatusRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/UserManagement/GetUserStatus?userId={userId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(setUserStatusRequest);
            return await response.ToCommonResponse<object>();
        }

        public async Task<HttpResponse<List<GetUserResponse>>> GetCacheManagement()
        {
            HttpRequestMessage setUserStatusRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/CacheManagement")

            };
            HttpResponseMessage response = await httpClient.SendAsync(setUserStatusRequest);
            return await response.ToCommonResponse<List<GetUserResponse>>();
        }
    }
}
