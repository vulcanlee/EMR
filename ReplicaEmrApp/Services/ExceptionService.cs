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
    public async Task UploadAsync(List<ExceptionRecord> exceptionRecords)
    {

        string endpoint = $"http://192.168.82.157:5000/api/ExceptionRecord";

        HttpClient client = new HttpClient();
        var content = new StringContent(JsonConvert.SerializeObject(exceptionRecords), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(endpoint, content);
       
        if (response.IsSuccessStatusCode)
        {
            exceptionRecords.Clear();
            StorageJSONService<List<ExceptionRecord>>.WriteToDataFileAsync("data", "ExceptionRecord.json", exceptionRecords).Wait();
        }
        
    }
}
