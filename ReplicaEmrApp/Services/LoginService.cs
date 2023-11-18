using DryIoc;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services;

public class LoginService
{
    private readonly GlobalObject globalObject;
    private readonly ReportCodeService reportCodeService;
    private readonly CurrentDeviceInformationService currentDeviceInformationService;

    public LoginService(GlobalObject globalObject,ReportCodeService reportCodeService,
        CurrentDeviceInformationService currentDeviceInformationService)
    {
        this.globalObject = globalObject;
        this.reportCodeService = reportCodeService;
        this.currentDeviceInformationService = currentDeviceInformationService;
    }
    public async Task<bool> LoginAsync(string username, string password)
    {
        bool result = true;

        string loginEndpoint = $"http://office.exentric.com.tw:8080/webemr/comm/login.do?pageid=pageLogin&LOGIN=6&userid={username}&password={password}";

        HttpClientHandler handler = new HttpClientHandler();
        HttpClient client = new HttpClient(handler);
        client.BaseAddress = new Uri(@"http://office.exentric.com.tw:8080/webemr");
        HttpResponseMessage response = await client.PostAsync(loginEndpoint, null);
       
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(responseContent);

            #region 判斷登入是否成功
            if (string.IsNullOrEmpty(loginResponseDto.userid))
            {
                result = false;
            }
            else
            {
                CookieContainer cookieContainer = handler.CookieContainer;
                IEnumerable<Cookie> responseCookies = handler.CookieContainer.GetCookies(client.BaseAddress).Cast<Cookie>();

                string cookieValue = responseCookies.FirstOrDefault(x => x.Name == "JSESSIONID")?.Value;
                if (string.IsNullOrEmpty(cookieValue))
                {
                    result = false;
                }
                else
                {
                    globalObject.JSESSIONID = cookieValue;
                    globalObject.UserId = loginResponseDto.userid;
                    globalObject.UserName = loginResponseDto.username;
                    currentDeviceInformationService.CurrentDeviceInformation
                        .Account = globalObject.UserId;
                    result = true;
                }
            }
            #endregion
        }
        else
        {
            result = false;
        }
        return result;
    }
}
