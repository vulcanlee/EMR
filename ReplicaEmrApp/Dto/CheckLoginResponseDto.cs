using Newtonsoft.Json;

namespace ReplicaEmrApp.Dto
{
    public class CheckLoginResponseDto
    {
        [JsonProperty("returnCode")]
        public string ReturnCode { get; set; }
        [JsonProperty("returnMessage")]
        public string ReturnMessage { get; set; }
    }
}
