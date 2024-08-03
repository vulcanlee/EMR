using Microsoft.VisualBasic;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services
{
    public class CheckSessionService
    {
        private readonly GlobalObject globalObject;
        private readonly IPageDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IStorageJSONService<GlobalObject> storageJSONService;
        private readonly FailLogService failLogService;
        private readonly ParameterService parameterService;
        private readonly IDeviceService deviceService;

        public CheckSessionService(GlobalObject globalObject,
            IPageDialogService dialogService,
            INavigationService navigationService,
            IStorageJSONService<GlobalObject> storageJSONService,
            FailLogService failLogService,
                    ParameterService parameterService,
        IDeviceService deviceService)
        {
            this.globalObject = globalObject;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.storageJSONService = storageJSONService;
            this.failLogService = failLogService;
            this.parameterService = parameterService;
            this.deviceService = deviceService;
        }

        /// <summary>
        /// 檢查是否需要重新登入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="specifyLog"></param>
        /// <param name="showAlertDialog"></param>
        /// <returns></returns>
        public async Task<bool> ReloadDataAsync<T>(ApiResultModel<T> dto, OperlogDto specifyLog, bool showAlertDialog = true)
        {
            if (dto != null && dto.code == MagicValueHelper.NeedLoginStatus)
            {
                if (showAlertDialog) await dialogService.DisplayAlertAsync("登入逾時", "請重新登入", "確定");
                globalObject.CleanUp();
                await storageJSONService
                    .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename,
                                   globalObject);
                await navigationService.NavigateAsync(MagicValueHelper.LoginPage);

                return true;
            }
            else if (dto != null && dto.code != MagicValueHelper.SuccessStatus)
            {
                OperlogDto operlog = new OperlogDto();
                operlog.PrepareBaseData(parameterService, deviceService);
                operlog.PrepareExceptionData(specifyLog);
                if(string.IsNullOrEmpty(specifyLog.errorMsg)) operlog.errorMsg = dto.msg;

                await failLogService.AddPostToFileAsync(operlog);
                if(showAlertDialog) await dialogService.DisplayAlertAsync("警告", $"code:{dto.code} , {dto.msg}", "確定");              

                return true;
            }

            return false;
        }

    }
}
