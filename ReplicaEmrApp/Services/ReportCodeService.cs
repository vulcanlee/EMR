using Newtonsoft.Json;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services
{
    public class ReportCodeService
    {
        private readonly GlobalObject globalObject;

        public ReportCodeService(GlobalObject globalObject) {
            this.globalObject = globalObject;
        }

        public async Task GetAsync()
        {
            globalObject.reportCodes.Clear();
            
            string reportCodeEndpoint = $"http://office.exentric.com.tw:8080/webemr/param/type.do;jsessionid={globalObject.JSESSIONID}?MIGHT=&app=&cardType=3";
           
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(reportCodeEndpoint,null);
            
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                globalObject.reportCodes = JsonConvert.DeserializeObject<List<ReportCodeNode>>(responseContent);
            }
        }
    }
}
