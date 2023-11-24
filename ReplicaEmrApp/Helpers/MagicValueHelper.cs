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
        public const string SplashPage = "SplashPage";
        public const string LoginPage = "/NaviPage/LoginPage";
        public const string HomePage = $"/NaviPage/HomePage";
        public const string ReportDetailPage = "ReportDetailPage";
        public const string ReportContentPage = "ReportContentPage";
        public const string SettingPage = "SettingPage";
        public const string EmrApiUrl = $"http://emrrestapi.posly.cc/api/";
        public const string ExceptionLogApiUrl = $"http://192.168.31.117:5229/";
        public const string ExceptionRecord = $"{ExceptionLogApiUrl}api/ExceptionRecord";
        public const string DataPath = $"data";
        public const string ExceptionRecordFilename = $"ExceptionRecord.json";
        public const string GlobalObjectFilename = $"GlobalObject.json";
        public const string SettingModelFilename = $"SettingModel.json";
        public const string CurrentDeviceInformationFilename = $"CurrentDeviceInformation.json";
        #endregion
    }
}
