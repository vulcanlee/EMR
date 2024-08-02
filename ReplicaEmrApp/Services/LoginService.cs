using DryIoc;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services;

public class LoginService
{
    private readonly GlobalObject globalObject;
    private readonly CurrentDeviceInformationService currentDeviceInformationService;
    private readonly CheckSessionService checkSessionService;
    private readonly IDeviceService deviceService;
    private readonly ParameterService parameterService;

    public LoginService(GlobalObject globalObject,
        CurrentDeviceInformationService currentDeviceInformationService,
        CheckSessionService checkSessionService,
        IDeviceService deviceService,
        ParameterService parameterService)
    {
        this.globalObject = globalObject;
        this.currentDeviceInformationService = currentDeviceInformationService;
        this.checkSessionService = checkSessionService;
        this.deviceService = deviceService;
        this.parameterService = parameterService;
    }
    public async Task<ApiResultModel<string>> LoginAsync(string username, string password, string tenantCode)
    {
        ApiResultModel<string> apiResult = new ApiResultModel<string>();
        try
        {
            string loginEndpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.LoginApi}";

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            HttpClient client = new HttpClient(handler);
            //TODO:修改租戶代碼
            var requestBody = JsonConvert.SerializeObject(new
            {
                username = username,
                password = password,
                code = "",
                uuid = "",
                tenantCode = tenantCode,
                deviceId = deviceService.GetDeviceId()
            });

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(loginEndpoint, content);

            string responseContent = await response.Content.ReadAsStringAsync();

            apiResult = JsonConvert.DeserializeObject<ApiResultModel<string>>(responseContent);
        }
        catch (Exception e)
        {
            apiResult = new ApiResultModel<string>
            {
                code = "-1",
                msg = e.Message
            };
        }
        return apiResult;
    }
}
