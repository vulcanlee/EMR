using AutoReport.Helpers;
using AutoReport.Models;
using Newtonsoft.Json;
using System.Text;

namespace AutoReport.Services;

public class LoginService
{
    private readonly ILogger<LoginService> logger;

    public LoginService(ILogger<LoginService> logger)
    {
        this.logger = logger;
    }

    public async Task<ApiResultModel<string>> LoginAsync(string username, string password
        , string tenantCode= "70400845")
    {
        ApiResultModel<string> apiResult = new();
        try
        {
            string loginEndpoint = $"{MagicValueHelper.WebemrEndpoint}" +
                $"{MagicValueHelper.LoginApi}";

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = 
                (sender, cert, chain, sslPolicyErrors) => true
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
            });

            var content = new StringContent(requestBody,
                Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client
                .PostAsync(loginEndpoint, content);

            string responseContent = await response.Content.ReadAsStringAsync();

            apiResult = JsonConvert
                .DeserializeObject<ApiResultModel<string>>(responseContent);
        }
        catch (Exception e)
        {
            logger.LogError(e, "LoginAsync Error");
        }
        return apiResult;
    }
}
