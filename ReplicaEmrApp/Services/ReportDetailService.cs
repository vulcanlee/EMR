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
    public class ReportDetailService
    {
        private readonly GlobalObject globalObject;

        public ReportDetailService(GlobalObject globalObject) {
            this.globalObject = globalObject;
        }

        public async Task<ReportDetailResponseDto> GetAsync(string reportCode)
        {            
            string reportDetailEndpoint = $"http://office.exentric.com.tw:8080/webemr/APP/APP1S001.do;jsessionid={globalObject.JSESSIONID}?pageid=APP1S001&QRY=1&reportCode={reportCode}";
           
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(reportDetailEndpoint,null);
            
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var reportDetail = JsonConvert.DeserializeObject<ReportDetailResponseDto>(responseContent);

                return reportDetail;
            }
            else
            {
                return new();
            }
        }
    }
}
