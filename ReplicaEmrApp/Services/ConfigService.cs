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
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services;

public class ConfigService
{
    private readonly IStorageJSONService<List<ConfigModel>> storageJSONService;
    private readonly GlobalObject globalObject;
    private readonly ParameterService parameterService;

    public ConfigService(IStorageJSONService<List<ConfigModel>> storageJSONService,
        GlobalObject globalObject, ParameterService parameterService)
    {
        this.storageJSONService = storageJSONService;
        this.globalObject = globalObject;
        this.parameterService = parameterService;
    }

    public async Task WriteAsync(List<ConfigModel> data)
    {
        await storageJSONService
               .WriteToDataFileAsync(MagicValueHelper.DataPath,
               MagicValueHelper.ConfigDataFilename, data);
    }

    public async Task<List<ConfigModel>> ReadAsync()
    {
        return await storageJSONService
               .ReadFromFileAsync(MagicValueHelper.DataPath,
               MagicValueHelper.ConfigDataFilename);
    }

    public async Task<(ApiResultModel<ConfigModel>, OperlogDto)> GetAsync()
    {
        ApiResultModel<ConfigModel> apiResult = new ApiResultModel<ConfigModel>();

        string unsignReportEndpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.ConfigApi}?configName={MagicValueHelper.ConfigName}";
        OperlogDto operlog = new OperlogDto()
        {
            businessType = BusinessTypeEnum.其他.GetHashCode(),
            method = nameof(GetAsync),
            requestMethod = "GET",
            operUrl = unsignReportEndpoint.Split('?')[0],
            operParam = unsignReportEndpoint.Split('?')[1],
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

            HttpResponseMessage response = await httpClient.GetAsync(unsignReportEndpoint);

            string responseContent = await response.Content.ReadAsStringAsync();
            operlog.jsonResult = responseContent;
            apiResult = JsonConvert.DeserializeObject<ApiResultModel<ConfigModel>>(responseContent);

        }
        catch (Exception ex)
        {
            operlog.errorMsg = ex.ToString();
        }

        return (apiResult,operlog);
    }

}
