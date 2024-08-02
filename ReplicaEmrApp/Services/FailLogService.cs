using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using System.Net.Http.Json;

#if ANDROID
using ReplicaEmrApp;
#endif

#if IOS
using ReplicaEmrApp.Platforms.iOS;
#endif

namespace ReplicaEmrApp.Services;

public class FailLogService
{
    private readonly GlobalObject globalObject;
    private readonly ParameterService parameterService;
    private readonly IDeviceService deviceService;
    private readonly IStorageJSONService<List<OperlogDto>> storageJSONService;

    public FailLogService(GlobalObject globalObject, ParameterService parameterService,
        IDeviceService deviceService,
        IStorageJSONService<List<OperlogDto>> storageJSONService)
    {
        this.globalObject = globalObject;
        this.parameterService = parameterService;
        this.deviceService = deviceService;
        this.storageJSONService = storageJSONService;
    }

    public async Task<(ApiResultModel<OperlogDto>, OperlogDto)> ListPostAsync()
    {
        ApiResultModel<OperlogDto> apiResult = new ApiResultModel<OperlogDto>();

        string endpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.OperlogListApi}";

        OperlogDto requestOperlog = new OperlogDto()
        {
            @params = new Params()
            {
                beginTime = DateTime.Now.AddDays(-parameterService.GetFailLogRange()).ToString("yyyy-MM-dd"),
                endTime = DateTime.Now.ToString("yyyy-MM-dd"),
            },
            businessTypes = parameterService.GetBusinessTypes().Select(x => x.GetHashCode()).ToList(),
            operName = globalObject.UserName,
            operatorType = parameterService.GetOperatorType().GetHashCode(),
            operLocation = deviceService.GetDeviceId(),
            status = StatusEnum.異常.GetHashCode()

        };

        OperlogDto responseOperlog = new OperlogDto()
        {
            businessType = BusinessTypeEnum.其他.GetHashCode(),
            method = nameof(ListPostAsync),
            requestMethod = "POST",
            operUrl = endpoint,
            operParam = JsonConvert.SerializeObject(requestOperlog),
            operName = globalObject.UserName,
        };
        try
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            HttpClient httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {globalObject.Token}");

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(endpoint, requestOperlog);

            string responseContent = await response.Content.ReadAsStringAsync();
            responseOperlog.jsonResult = responseContent;
            apiResult = JsonConvert.DeserializeObject<ApiResultModel<OperlogDto>>(responseContent);

        }
        catch (Exception ex)
        {
            responseOperlog.errorMsg = ex.ToString();
        }


        return (apiResult, responseOperlog);
    }

    public async Task<ApiResultModel<string>> AddPostAsync(OperlogDto operlog)
    {
        ApiResultModel<string> apiResult = new ApiResultModel<string>();
        try
        {
            string endpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.OperlogAddApi}";

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            HttpClient httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {globalObject.Token}");

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(endpoint, operlog);

            string responseContent = await response.Content.ReadAsStringAsync();
            apiResult = JsonConvert.DeserializeObject<ApiResultModel<string>>(responseContent);
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return apiResult;
    }

    public async Task AddPostToFileAsync(OperlogDto operlog)
    {
        List<OperlogDto> failLogs = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.FailLogRecordFilename);
        failLogs.Add(operlog);
        await storageJSONService.WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.FailLogRecordFilename, failLogs);
        await AddPostUploadAsync();
    }

    public async Task<ApiResultModel<string>> AddPostUploadAsync()
    {
        ApiResultModel<string> apiResult = new ApiResultModel<string>();
        List<OperlogDto> failLogs = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.FailLogRecordFilename);
        List<OperlogDto> uploadSuccess = new();
        foreach (var log in failLogs) {
            var result = await AddPostAsync(log);
            if (result != null && result.code == MagicValueHelper.SuccessStatus)
            {
                uploadSuccess.Add(log);
            }
        }
        failLogs = failLogs.Except(uploadSuccess).ToList();
        await storageJSONService.WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.FailLogRecordFilename, failLogs);

        return apiResult;
    }
}
