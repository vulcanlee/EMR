using DryIoc;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
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

public class ExceptionService
{
    private readonly IStorageJSONService<List<ExceptionRecord>> storageJSONService;

    public ExceptionService(IStorageJSONService<List<ExceptionRecord>> storageJSONService)
    {
        this.storageJSONService = storageJSONService;
    }

    public async Task UploadAsync(List<ExceptionRecord> exceptionRecords)
    {

        string endpoint = $"{MagicValueHelper.ExceptionRecord}";

        HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10),
        };
        var content = new StringContent(JsonConvert
            .SerializeObject(exceptionRecords), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client
            .PostAsync(endpoint, content).ConfigureAwait(false);
       
        if (response.IsSuccessStatusCode)
        {
            exceptionRecords.Clear();
            await storageJSONService
                .WriteToDataFileAsync(MagicValueHelper.DataPath,
                MagicValueHelper.ExceptionRecordFilename, exceptionRecords);
        }
        
    }
}
