using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services
{
    public class CheckSessionService
    {
        private readonly GlobalObject globalObject;

        public CheckSessionService(GlobalObject globalObject)
        {
            this.globalObject = globalObject;
        }
        public async Task<bool> CheckLoginAsync()
        {
            string endpoint = $"http://office.exentric.com.tw:8080/webemr/comm/checkLogin.do;" +
                $"jsessionid={globalObject.JSESSIONID}?sessionid={globalObject.JSESSIONID}";

            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(@"http://office.exentric.com.tw:8080/webemr");
            HttpResponseMessage response = await client.PostAsync(endpoint, null);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    CheckLoginResponseDto checkLoginResponseDto = JsonConvert.DeserializeObject<CheckLoginResponseDto>(responseContent);
                    #region 判斷登入是否成功
                    if (checkLoginResponseDto != null && checkLoginResponseDto.ReturnCode == "0000")
                    {
                        return true;
                    }

                }
                catch (Exception e)
                {
                    return false;
                }
                #endregion
            }
            return false;

        }

    }
}
