using CommonLogic.Models.Responses.Base;
using Newtonsoft.Json;

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
