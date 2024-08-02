using DryIoc;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ShareResource.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services;

public class CheckUploadService
{
    private readonly GlobalObject globalObject;
    private readonly ParameterService parameterService;

    public CheckUploadService(GlobalObject globalObject, ParameterService parameterService)
    {
        this.globalObject = globalObject;
        this.parameterService = parameterService;
    }

    public async Task<(ApiResultModel<object>, OperlogDto)> PostAsync(CheckUploadRequestDto checkUploadRequest)
    {
        ApiResultModel<object> apiResult = new ApiResultModel<object>();

        string checkUploadEndpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.CheckUploadApi}";
        OperlogDto operlog = new OperlogDto()
        {
            businessType = BusinessTypeEnum.其他.GetHashCode(),
            method = nameof(PostAsync),
            requestMethod = "POST",
            operUrl = checkUploadEndpoint,
            operParam = JsonConvert.SerializeObject(checkUploadRequest),
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

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(checkUploadEndpoint, checkUploadRequest);

            string responseContent = await response.Content.ReadAsStringAsync();
            operlog.jsonResult = responseContent;
            apiResult = JsonConvert.DeserializeObject<ApiResultModel<object>>(responseContent);

        }
        catch (Exception ex)
        {
            operlog.errorMsg = ex.ToString();
        }

        return (apiResult,operlog);
    }

}
