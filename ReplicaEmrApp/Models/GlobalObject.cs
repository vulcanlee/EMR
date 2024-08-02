using ReplicaEmrApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public class GlobalObject
    {
        public string Token  { get; set; }
        public string UserName  { get; set; }
        public string PWord { get; set; }
        public string Version { get; set; }
        public string TenantCode { get; set; }
        public List<UnsignReportData> unSignItem { get; set; } = new();
        public List<ConfigModel> config { get; set; } = new();
        public string IdentityNo { get; set; }
        public string CertificateData { get; set; }

        public void Copy(GlobalObject source,GlobalObject destination)
        {
            destination.Token = source.Token;
            destination.UserName = source.UserName;
            destination.Version = source.Version;
            destination.TenantCode = source.TenantCode;
            destination.PWord = source.PWord;
            destination.unSignItem = source.unSignItem;
            destination.config = source.config;
            destination.IdentityNo = source.IdentityNo;
            destination.CertificateData = source.CertificateData;
        }

        public void CleanUp(bool configClear = false)
        {
            Token = null;
            UserName = null;
            PWord = null;
            Version = null;
            TenantCode = null;
            unSignItem.Clear();
            IdentityNo = null;
            CertificateData = null;
            if(configClear) config.Clear();
        }
    }
}
