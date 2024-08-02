using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services
{
    public class DocumentReportService
    {
        private readonly GlobalObject globalObject;
        private readonly ParameterService parameterService;

        public DocumentReportService(GlobalObject globalObject, ParameterService parameterService)
        {
            this.globalObject = globalObject;
            this.parameterService = parameterService;
        }

        public async Task<(string, OperlogDto)> GetAsync(string docId)
        {
            string endpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.DocumentReportApi}/{docId}";
            string responseContent = string.Empty;
            OperlogDto operlog = new OperlogDto()
            {
                businessType = BusinessTypeEnum.其他.GetHashCode(),
                method = nameof(GetAsync),
                requestMethod = "GET",
                operUrl = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.DocumentReportApi}",
                operParam = docId,
                operName = globalObject.UserName,
            };
            try {
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                HttpClient httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {globalObject.Token}");

                HttpResponseMessage response = await httpClient.GetAsync(endpoint);

                responseContent = await response.Content.ReadAsStringAsync();

                operlog.jsonResult = responseContent;
            }
            catch (Exception ex)
            {
                operlog.errorMsg = ex.ToString();
            }
                return (responseContent, operlog);
        }
    }
}
