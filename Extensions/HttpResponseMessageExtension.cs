using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Models.Responses.Base;

namespace UserServiceAPITests.Extensions
{
    public static class HttpResponseMessageExtension
    {
        public static async Task<HttpResponse<T>> ToCommonResponse<T>(this HttpResponseMessage message)
        {
            string responseString = await message.Content.ReadAsStringAsync();

            var commonResponse = new HttpResponse<T>
            {
                HttpStatusCode = message.StatusCode,
                Content = responseString
            };

            try
            {
                commonResponse.Body = JsonConvert.DeserializeObject<T>(responseString);
            }
            catch(JsonReaderException){}

            return commonResponse;
        }
    }
}
