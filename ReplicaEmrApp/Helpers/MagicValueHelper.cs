using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Helpers
{
    public class MagicValueHelper
    {
        #region String
        public const string LoginPage = "LoginPage";
        public const string HomePage = $"/MDPage/NaviPage/HomePage";
        public const string EmrApiUrl = $"http://emrrestapi.posly.cc/api/";
        public const string DataPath = $"data";
        public const string ExceptionRecordFilename = $"ExceptionRecord.json";
        public const string GlobalObjectFilename = $"GlobalObject.json";
        #endregion
    }
}
