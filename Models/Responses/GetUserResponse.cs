using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceAPITests.Models.Responses
{
    public class GetUserResponse
    {
        [JsonProperty("id")]
        public int id { get; set; }
    }
}
