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

public class UnsignReportStorageService
{
    private readonly IStorageJSONService<List<UnsignReportData>> storageJSONService;

    public UnsignReportStorageService(IStorageJSONService<List<UnsignReportData>> storageJSONService)
    {
        this.storageJSONService = storageJSONService;
    }

    public async Task WriteAsync(List<UnsignReportData> data)
    {
        await storageJSONService
               .WriteToDataFileAsync(MagicValueHelper.DataPath,
               MagicValueHelper.UnsignReportDataFilename, data);
    }

    public async Task<List<UnsignReportData>> ReadAsync()
    {
        return await storageJSONService
               .ReadFromFileAsync(MagicValueHelper.DataPath,
               MagicValueHelper.UnsignReportDataFilename);
    }
}
