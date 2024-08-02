using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Services;
using ShareResource.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Dto
{
    public class OperlogDto
    {
        public async void PrepareBaseData(ParameterService parameterService, IDeviceService deviceService) {
            title = $"EMR APP({await deviceService.GetDevicePlatformAsync()})";
            operatorType = parameterService.GetOperatorType().GetHashCode();
            operIp = await deviceService.GetDeviceIpAsync();
            operLocation = deviceService.GetDeviceId();
            status = StatusEnum.異常.GetHashCode();
        }
        public void PrepareExceptionRecordData(ExceptionRecord exceptionRecord)
        {
            businessType = BusinessTypeEnum.其他.GetHashCode();
            method = "Exception";
            requestMethod = "N/A";
            operUrl = "Crash";
            operName = exceptionRecord.UserId;
            jsonResult = exceptionRecord.Exception;
            errorMsg = exceptionRecord.Message;
            operTime = exceptionRecord.CreateAt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public void PrepareExceptionData(OperlogDto specifyLog)
        {
            businessType = specifyLog.businessType;
            method = specifyLog.method;
            requestMethod = specifyLog.requestMethod;
            operUrl = specifyLog.operUrl;
            operParam = specifyLog.operParam;
            operName = specifyLog.operName;
            jsonResult = specifyLog.jsonResult;
            errorMsg = specifyLog.errorMsg;
            operTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public Params @params { get; set; }
        public string title { get; set; }
        public int? businessType { get; set; }
        public List<int> businessTypes { get; set; }
        public string method { get; set; }
        public string requestMethod { get; set; }
        public int? operatorType { get; set; }
        public string operName { get; set; }
        public string operUrl { get; set; }
        public string operIp { get; set; }
        public string operLocation { get; set; }
        public string operParam { get; set; }
        public string jsonResult { get; set; }
        public int? status { get; set; }
        public string errorMsg { get; set; }
        public string operTime { get; set; }

        public string reportdateFormat
        {
            get
            {
                return $"{operTime}";
                //return $"{operTime?.ToString("yyyy-MM-dd HH:mm:ss")}";
            }
        }
    }

    public class Params
    {
        public string beginTime { get; set; }
        public string endTime { get; set; }
    }
}
