using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using WalletServiceAPITests.Extensions;
using WalletServiceAPITests.Models.Requests.WalletService;
using WalletServiceAPITests.Models.Responses.Base;
using WalletServiceAPITests.Models.Responses.WalletService;

namespace WalletServiceAPITests.ServiceProvider
{
    public class WalletServiceServiceProvider : IObservable<string>
    {
        private static Lazy<WalletServiceServiceProvider> _instance = new Lazy<WalletServiceServiceProvider>(() => new WalletServiceServiceProvider());
        public static WalletServiceServiceProvider Instance => _instance.Value;

        private readonly string _baseUrl = "https://walletservice-uat.azurewebsites.net";
        private HttpClient httpClient = new HttpClient();
        

        public async Task<HttpResponse<double>> GetBalance(int userId)
        {
            
            HttpRequestMessage getBalanceRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/Balance/GetBalance?userId={userId}"),               
            };

            HttpResponseMessage response = await httpClient.SendAsync(getBalanceRequest);
            return await response.ToCommonResponse<double>();
        }
        public async Task<HttpResponse<string>> PostCharge(ChargeModel charge)
        {
            string serializedBody = JsonConvert.SerializeObject(charge);

            HttpRequestMessage chargeRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Balance/Charge"),
                Content = new StringContent(serializedBody, Encoding.UTF8, "application/json")

            };
            HttpResponseMessage response = await httpClient.SendAsync(chargeRequest);
            var communResponse = await response.ToCommonResponse<string>();

            if (communResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                //ActiveModel
                //createdTransactionsIdCollection.Add(communResponse.Body);

                //Reactive
                NotifyAllObserversAboutNewTransaction(communResponse.Body);
            }

            return communResponse;
        }

        public async Task<HttpResponse<List<GetTransactionModel>>> GetTransactions(int userId)
        {

            HttpRequestMessage chargeRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/Balance/GetTransactions?userId={userId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(chargeRequest);

            return await response.ToCommonResponse<List<GetTransactionModel>>();
        }

        public async Task<HttpResponse<string>> RevertTransaction(string transactionId)
        {

            HttpRequestMessage chargeRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_baseUrl}/Balance/RevertTransaction?transactionId={transactionId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(chargeRequest);
            var communResponse = await response.ToCommonResponse<string>();
            if (communResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                //ActiveModel
                //createdTransactionsIdCollection.Add(communResponse.Body);

                //Reactive

                NotifyAllObserversAboutNewRevertTransaction(transactionId);
            }
            return communResponse;
        }

        #region ActiveModel
        //private List<string> createdTransactionsIdCollection = new();
        //public async Task ReverseAllTransactions()
        //{
        //    foreach(string createdTransactionId in createdTransactionsIdCollection)
        //    {
        //        await RevertTransaction(createdTransactionId);
        //    }
        //    createdTransactionsIdCollection = new();
        //}
        #endregion

        #region ReactiveModel
        private List<IObserver<string>> _observer = new List<IObserver<string>>();
        private List<IObserver<string>> _observerRevert = new List<IObserver<string>>();


        public IDisposable Subscribe(IObserver<string> observer)
        {
            _observer.Add(observer);
            return null;
        }
        public IDisposable SubscribeRevert(IObserver<string> observer)
        {
            _observerRevert.Add(observer);
            return null;
        }

        //Notify All Observers about new transaction
        private void NotifyAllObserversAboutNewTransaction(string id)
        {
            foreach(IObserver<string> observer in _observer)
            {
                observer.OnNext(id);
            }
        }

        //Notify All Observers about new transaction
        private void NotifyAllObserversAboutNewRevertTransaction(string id)
        {
            foreach (IObserver<string> observer in _observerRevert)
            {
                observer.OnNext(id);
            }
        }

        
        #endregion
    }
}
