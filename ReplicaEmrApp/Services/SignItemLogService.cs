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

public class SignItemLogService
{
    private readonly IStorageJSONService<List<SignItemLog>> storageJSONService;
    private readonly FailLogService failLogService;
    private readonly ParameterService parameterService;
    private readonly CheckSessionService checkSessionService;
    private readonly IDeviceService deviceService;
    public List<SignItemLog> Items { get; set; } = new List<SignItemLog>();

    public SignItemLogService(IStorageJSONService<List<SignItemLog>> storageJSONService,
        FailLogService failLogService,
        ParameterService parameterService,
        CheckSessionService checkSessionService,
        IDeviceService deviceService
        )
    {
        this.storageJSONService = storageJSONService;
        this.failLogService = failLogService;
        this.parameterService = parameterService;
        this.checkSessionService = checkSessionService;
        this.deviceService = deviceService;
    }

    public async Task ReadAsync()
    {
        var storageJSONService = ServiceProviderHelper.Current
         .GetService<IStorageJSONService<List<SignItemLog>>>();
        Items = await storageJSONService
        .ReadFromFileAsync(MagicValueHelper.DataPath,
        MagicValueHelper.SignItemLogFilename);
        return;
    }

    public async Task WriteAsync()
    {
        await storageJSONService
          .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.ExceptionRecordFilename, Items);
        return;
    }

    public void Add(SignItemLog item)
    {
        Items.Insert(0, item);

        Items = Items.Take(3000).ToList();
    }
}
