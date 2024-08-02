using DryIoc;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services;

public class TenantService
{
    private readonly GlobalObject globalObject;
    private readonly CurrentDeviceInformationService currentDeviceInformationService;
    private readonly ConfigService configService;
    private readonly ParameterService parameterService;

    public TenantService(GlobalObject globalObject,
        CurrentDeviceInformationService currentDeviceInformationService, ConfigService configService,
        ParameterService parameterService)
    {
        this.globalObject = globalObject;
        this.currentDeviceInformationService = currentDeviceInformationService;
        this.configService = configService;
        this.parameterService = parameterService;
    }
    public async Task<(ApiResultModel<TenantResponseDto>, OperlogDto)> GetAsync()
    {
        ApiResultModel<TenantResponseDto> apiResult = new ApiResultModel<TenantResponseDto>();
        string tenantEndpoint = $"{await parameterService.GetEmrServiceUrl()}{MagicValueHelper.TenantApi}";
        OperlogDto operlog = new OperlogDto()
        {
            businessType = BusinessTypeEnum.其他.GetHashCode(),
            method = nameof(GetAsync),
            requestMethod = "GET",
            operUrl = tenantEndpoint,
            operParam = string.Empty,
            operName = globalObject.UserName,
        };
        try
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            HttpClient client = new HttpClient(handler);

            HttpResponseMessage response = await client.GetAsync(tenantEndpoint);

            string responseContent = await response.Content.ReadAsStringAsync();

            operlog.jsonResult = responseContent;

            apiResult = JsonConvert.DeserializeObject<ApiResultModel<TenantResponseDto>>(responseContent);
        }
        catch (Exception ex)
        {
            operlog.errorMsg = ex.ToString();
        }
        return (apiResult, operlog);
    }
}
