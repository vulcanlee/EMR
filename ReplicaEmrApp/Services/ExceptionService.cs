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
    private readonly FailLogService failLogService;
    private readonly ParameterService parameterService;
    private readonly CheckSessionService checkSessionService;
    private readonly IDeviceService deviceService;

    public ExceptionService(IStorageJSONService<List<ExceptionRecord>> storageJSONService,
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

    public async Task<bool> UploadAsync(List<ExceptionRecord> exceptionRecords)
    {
        List<ExceptionRecord> newExceptionRecord = new List<ExceptionRecord>(exceptionRecords);
        foreach (var record in exceptionRecords)
        {
            OperlogDto operlog = new OperlogDto();
            operlog.PrepareBaseData(parameterService, deviceService);
            operlog.PrepareExceptionRecordData(record);

            var dto = await failLogService.AddPostAsync(operlog);

            if (dto != null && dto.code != MagicValueHelper.SuccessStatus) return false;

            newExceptionRecord.Remove(record);
            await storageJSONService
          .WriteToDataFileAsync(MagicValueHelper.DataPath,
          MagicValueHelper.ExceptionRecordFilename, newExceptionRecord);

        }
        return true;
    }
}
