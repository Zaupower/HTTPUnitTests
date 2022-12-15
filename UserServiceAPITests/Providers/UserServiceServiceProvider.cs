using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Extensions;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.Models.Responses.UserService;

namespace UserServiceAPITests.ServiceProvider
{
    public class UserServiceServiceProvider : IObservable<int>
    {
        private static Lazy<UserServiceServiceProvider> _instance = new Lazy<UserServiceServiceProvider> (()=> new UserServiceServiceProvider());

        public static UserServiceServiceProvider Instance => _instance.Value;

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


            var communResponse = await response.ToCommonResponse<int>();

            if (communResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                //ActiveModel
                //createdTransactionsIdCollection.Add(communResponse.Body);

                //Reactive
                NotifyAllObserversAboutNewUser(communResponse.Body);
            }

            return communResponse;
        }

        public async Task<HttpResponse<object>> DeleteUser(int userId)
        {
            HttpRequestMessage createUserrequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_baseUrl}/Register/DeleteUser?userId={userId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(createUserrequest);
            var communResponse = await response.ToCommonResponse<object>();
            if (communResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                //ActiveModel
                //createdTransactionsIdCollection.Add(communResponse.Body);

                //Reactive
                NotifyAllObserversAboutDeleteUser(userId);
            }

            return communResponse;
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
        public async Task<HttpResponse<object>> DeleteCacheManagement()
        {
            HttpRequestMessage setUserStatusRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_baseUrl}/CacheManagement")

            };
            HttpResponseMessage response = await httpClient.SendAsync(setUserStatusRequest);
            return await response.ToCommonResponse<object>();
        }

        #region ReactiveModel
        private List<IObserver<int>> _observerNewUser = new List<IObserver<int>>();
        private List<IObserver<int>> _observerDeleteUser = new List<IObserver<int>>();


        public IDisposable Subscribe(IObserver<int> observer)
        {

            _observerNewUser.Add(observer);
            return null;
        }
        public IDisposable SubscribeDeleteUser(IObserver<int> observer)
        {
            _observerDeleteUser.Add(observer);
            return null;
        }

        //Notify All Observers about new user
        private void NotifyAllObserversAboutNewUser(int id)
        {
            foreach (IObserver<int> observer in _observerNewUser)
            {
                observer.OnNext(id);
            }
        }

        //Notify All Observers about delete user
        private void NotifyAllObserversAboutDeleteUser(int id)
        {
            foreach (IObserver<int> observer in _observerDeleteUser)
            {
                observer.OnNext(id);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
