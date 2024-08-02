using AutoReport.Helpers;
using AutoReport.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace AutoReport.Services;

public class Document2Base64Service
{
    private readonly ILogger<Document2Base64Service> logger;

    public Document2Base64Service(ILogger<Document2Base64Service> logger)
    {
        this.logger = logger;
    }

    public async Task<ApiSingleResultModel<string>> ConvertBase64Async(string token,
        string contentXml)
    {
        ApiSingleResultModel<string> apiResult = new();
        try
        {
            string loginEndpoint = $"{MagicValueHelper.WebemrEndpoint}" +
                $"{MagicValueHelper.DocumentToBase64Api}";

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => true
            };

            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders
                .Add("Authorization", $"Bearer {token}");

            var content = new StringContent(contentXml,
                Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client
                .PostAsync(loginEndpoint, content);

            string responseContent = await response.Content.ReadAsStringAsync();

            apiResult = JsonConvert
                .DeserializeObject<ApiSingleResultModel<string>>(responseContent);
        }
        catch (Exception e)
        {
            logger.LogError(e, "LoginAsync Error");
        }
        return apiResult;
    }
}
