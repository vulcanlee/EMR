using AutoReport.Helpers;
using AutoReport.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace AutoReport.Services;

public class DocumentAddService
{
    private readonly ILogger<Document2Base64Service> logger;

    public DocumentAddService(ILogger<Document2Base64Service> logger)
    {
        this.logger = logger;
    }

    public async Task<ApiSingleResultModel<string>> ConvertBase64Async(string token,
        string contentBase64)
    {
        ApiSingleResultModel<string> apiResult = new();
        try
        {
            string loginEndpoint = $"{MagicValueHelper.WebemrEndpoint}" +
                $"{MagicValueHelper.DocumentAddApi}";

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => true
            };

            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders
                .Add("Authorization", $"Bearer {token}");

            var requestBody = JsonConvert.SerializeObject(new
            {
                xml = contentBase64,
                format = "base64",
            });

            var content = new StringContent(requestBody,
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
