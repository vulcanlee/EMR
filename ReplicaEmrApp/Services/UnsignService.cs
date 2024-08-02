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
    public class UnsignService
    {
        private readonly GlobalObject globalObject;
        private readonly ParameterService parameterService;

        public UnsignService(GlobalObject globalObject, ParameterService parameterService)
        {
            this.globalObject = globalObject;
            this.parameterService = parameterService;
        }

        public async Task<(ApiResultModel<UnsignReportData>, OperlogDto)> GetAsync(DateTime docTmS, DateTime docTmE, string userNm,SignTpEnum signTp, CertTpEnum certTp)
        {
            ApiResultModel<UnsignReportData> apiResult = new ApiResultModel<UnsignReportData>();
            string startDate = docTmS.ToString("yyyy-MM-dd");
            string endDate = docTmE.ToString("yyyy-MM-dd");
            string unsignReportEndpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.UnsignReportApi}?docTmS={startDate}&docTmE={endDate}&userNm={userNm}&signTp={(int)signTp}&certTp={(int)certTp}";

            OperlogDto operlog = new OperlogDto()
            {
                businessType = BusinessTypeEnum.其他.GetHashCode(),
                method = nameof(GetAsync),
                requestMethod = "GET",
                operUrl = unsignReportEndpoint.Split('?')[0],
                operParam = unsignReportEndpoint.Split('?')[1],
                operName = globalObject.UserName,
            };
            try {
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                HttpClient httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {globalObject.Token}");

                HttpResponseMessage response = await httpClient.GetAsync(unsignReportEndpoint);

                string responseContent = await response.Content.ReadAsStringAsync();
                operlog.jsonResult = responseContent;
                apiResult = JsonConvert.DeserializeObject<ApiResultModel<UnsignReportData>>(responseContent);

            }
            catch (Exception ex)
            {
                operlog.errorMsg = ex.ToString();
            }
           
            return (apiResult, operlog);
        }
    }
}
