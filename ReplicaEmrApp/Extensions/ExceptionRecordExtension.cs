using Newtonsoft.Json;
using ShareResource.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Extensions
{
    public static class ExceptionRecordExtension
    {
        public static void CopyToClipboard(this ExceptionRecord exceptionRecord)
        {
            Clipboard.Default.SetTextAsync(JsonConvert.SerializeObject(exceptionRecord)).Wait();
        }
    }
}
