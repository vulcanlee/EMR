using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Helpers
{
    public class MagicValueHelper
    {
        #region Page Navigation
        public const string SplashPage = "SplashPage";
        public const string LoginPage = "/NaviPage/LoginPage";
        public const string HomePage = $"/NaviPage/HomePage";
        public const string ReportDetailPage = "ReportDetailPage";
        public const string ReportContentPage = "ReportContentPage";
        public const string FaillLogPage = "FailLogPage";
        public const string SettingPage = "SettingPage";
        #endregion
        public const string VerifyCode = "70400845";
        public const string ExceptionLogApiUrl = $"http://192.168.31.117:5229/";
        public const string ExceptionRecord = $"{ExceptionLogApiUrl}api/ExceptionRecord";
        #region File Name
        public const string DataPath = $"data";
        public const string ExceptionRecordFilename = $"ExceptionRecord.json";
        public const string SignItemLogFilename = $"SignItemLogs.json";
        public const string FailLogRecordFilename = $"FailLogRecord.json";
        public const string UnsignReportDataFilename = $"UnsignReportData.json";
        public const string ConfigDataFilename = $"ConfigData.json";
        public const string GlobalObjectFilename = $"GlobalObject.json";
        public const string SettingModelFilename = $"SettingModel.json";
        public const string UserInfoFilename = $"UserInfo.json";
        public const string CurrentDeviceInformationFilename = $"CurrentDeviceInformation.json";
        #endregion
        #region Api Method
        //public const string WebemrEndpoint = $"https://office.exentric.com.tw:8002/emr";
        public const string LoginApi = $"/login";
        public const string UserInfoApi = $"/getInfo";
        public const string CheckUploadApi = $"/emrApp/checkUpload";
        public const string UnsignReportApi = $"/emrApp/unsignList";
        public const string DocumentReportApi = $"/emrApp/docReportHtml";
        public const string ConfigApi = $"/emrApp/sysConfigList";
        public const string OperlogAddApi = $"/emrApp/operlogAdd";
        public const string OperlogListApi = $"/emrApp/operlogList";
        public const string SignatureAddApi = $"/emrApp/signatureAdd";
        public const string TenantApi = $"/system/tenant/optionselect";
        #endregion
        #region Config
        public const string ConfigName = $"APP - 行動簽章參數";
        public const string 行動簽章時間限制 = $"APP.firstSign.timeLimit";
        public const string 行動簽章自動執行間隔時間 = $"APP.repeatInterval";
        public const string 簽張失敗紀錄時間限制 = $"APP.operlog.timeLimit";
        public const string 簽章Hash型態 = $"APP.sign.hashFlag";
        public const string 是否顯示報告內容 = $"APP.report.showContent";
        #endregion
        #region Emc Scheme
        public const string GetCertOperation = "getcert";
        public const string GetCertScheme = "cgappgetcert";
        public const string SignOperation = "sign";
        public const string SignScheme = "cgappsign";
        #endregion
        #region Status Code
        public const string NeedLoginStatus = "401";
        public const string SuccessStatus = "200";
        public static readonly ImmutableList<ErrorMessage> McsErrorMessages;
        #endregion
        #region
        public static Color Primary = Color.Parse("#00A9B4");
        public static Color Danger = Color.Parse("#EB5757");
        #endregion

        static MagicValueHelper()
        {
            McsErrorMessages = ImmutableList.Create(
                new ErrorMessage() { code="0",message= "SUCCESS", description= "成功" },
                new ErrorMessage() { code="5001",message= "ERROR", description= "一般錯誤, 可能為其他導致無法正常運作的錯誤，請排除不正常的使用流程或無法使用的的網路環境或參數" },
                new ErrorMessage() { code="5004",message= "FUNCTION_UNSUPPORT", description= "未支援功能" },
                new ErrorMessage() { code= "5005", message= "INVALID_PARAM", description= "錯誤的參數" },
                new ErrorMessage() { code="5008",message= "BASE64_ERROR", description= "Base64編碼錯誤" },
                new ErrorMessage() { code="5012",message= "CERT_NOT_YET_VALID", description= "憑證尚未合法,無法使用" },
                new ErrorMessage() { code="5013",message= "CERT_EXPIRE_OR_NOT_YET_USE", description= "憑證可能過期或無法使用" },
                new ErrorMessage() { code="5302",message= "TOKEN_API_ERROR", description= "取 Server token 異常" },
                new ErrorMessage() { code="5303",message= "AUTHENTICATION_API_ERROR", description= "身分驗證 API 異常" },
                new ErrorMessage() { code="5304",message= "AUTHENTICATED_RESULT_NULL", description= "身分驗證結果Null" },
                new ErrorMessage() { code="5305",message= "ACCESS_SERVER_FAILED", description= "無法存取行動憑證管理系統伺服器" },
                new ErrorMessage() { code="5306",message= "NO_PRIVATE_KEY", description= "沒有私密金鑰" },
                new ErrorMessage() { code="5307",message= "KEYSTORE_OPERATION_FAILED", description= "Keystore操作失敗" },
                new ErrorMessage() { code="5308",message= "CERTIFICATE_API_ERROR", description= "憑證狀態 API 異常" },
                new ErrorMessage() { code="5309",message= "CERTIFICATE_RESULT_NULL", description= "憑證狀態結果 Null" },
                new ErrorMessage() { code="5310",message= "CERTIFICATE_VALUE_ERROR", description= "APP憑證值異常" },
                new ErrorMessage() { code="5311",message= "CERTIFICATE_VALUE_EMPTY", description= "APP憑證值empty" },
                new ErrorMessage() { code="5312",message= "BATCH_COUNT_EXCEED", description= "批次簽章數量超過範圍" },
                new ErrorMessage() { code="5313",message= "SERVER_CONNECT_ERROR", description= "與伺服器網路連線異常" },
                new ErrorMessage() { code="5314",message= "USERDEVICE_API_ERROR", description= "取使用者及裝置資訊異常" },
                new ErrorMessage() { code="5060",message= "UNMATCH_CERT_KEY", description= "錯誤的憑證或金鑰" }
                );
        }
    }

    public class ErrorMessage
    {
        public string code { get; set; }
        public string message { get; set; }
        public string description { get; set; }

    }
}
