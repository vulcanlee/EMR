using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services
{
    public class SignatureAddService
    {
        private readonly GlobalObject globalObject;
        private readonly ParameterService parameterService;
        private readonly IDeviceService deviceService;
        private readonly FailLogService failLogService;

        public SignatureAddService(GlobalObject globalObject,
            ParameterService parameterService, IDeviceService deviceService,
            FailLogService failLogService)
        {
            this.globalObject = globalObject;
            this.parameterService = parameterService;
            this.deviceService = deviceService;
            this.failLogService = failLogService;
        }

        public async Task<(ApiResultModel<SignatureAddRequestDto>, OperlogDto)> PostAsync(SignatureAddRequestDto signatureAddRequest)
        {
            ApiResultModel<SignatureAddRequestDto> apiResult = new ApiResultModel<SignatureAddRequestDto>();
            string signatureAddEndpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.SignatureAddApi}";

            var operParam = signatureAddRequest.Clone();
            OperlogDto operlog = new OperlogDto()
            {
                businessType = BusinessTypeEnum.簽章.GetHashCode(),
                method = nameof(PostAsync),
                requestMethod = "POST",
                operUrl = signatureAddEndpoint,
                operParam = JsonConvert.SerializeObject(operParam),
                operName = globalObject.UserName,
            };

            try {
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                HttpClient httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {globalObject.Token}");

                HttpResponseMessage response = await httpClient.PostAsJsonAsync(signatureAddEndpoint, signatureAddRequest);

                string responseContent = await response.Content.ReadAsStringAsync();
                operlog.jsonResult = responseContent;
                apiResult = JsonConvert.DeserializeObject<ApiResultModel<SignatureAddRequestDto>>(responseContent);

            }
            catch (Exception ex)
            {
                operlog.errorMsg = ex.ToString();
            }

            return (apiResult, operlog);
        }
    }
}
