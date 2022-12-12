using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UserServiceAPITests.Extensions;
using UserServiceAPITests.Models.Requests;
using UserServiceAPITests.Models.Requests.WalletService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.Models.Responses.WalletService;

namespace UserServiceAPITests.ServiceProvider
{
    public class WalletServiceServiceProvider
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
                createdTransactionsIdCollection.Add(communResponse.Body);
            }

            return communResponse;
        }

        public async Task<HttpResponse<List<GetTransactionModel>>> GetTransactions(int userId)
        {

            HttpRequestMessage chargeRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Balance/GetTransactions?userId={userId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(chargeRequest);

            return await response.ToCommonResponse<List<GetTransactionModel>>();
        }

        public async Task<HttpResponse<string>> RevertTransaction(string transactionId)
        {

            HttpRequestMessage chargeRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Balance/RevertTransaction?transactionId={transactionId}")

            };
            HttpResponseMessage response = await httpClient.SendAsync(chargeRequest);

            return await response.ToCommonResponse<string>();
        }

        private List<string> createdTransactionsIdCollection = new();
        public async Task ReverseAllTransactions()
        {
            foreach(string createdTransactionId in createdTransactionsIdCollection)
            {
                await RevertTransaction(createdTransactionId);
            }
            createdTransactionsIdCollection = new();
        }

    }
}
