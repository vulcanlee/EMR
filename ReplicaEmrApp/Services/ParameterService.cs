using Microsoft.Extensions.Configuration;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services
{
    public class ParameterService
    {
        private readonly IConfiguration configuration;
        private readonly IStorageJSONService<SettingModel> storageJSONService;
        private readonly GlobalObject globalObject;

        public ParameterService(IConfiguration configuration, IStorageJSONService<SettingModel> storageJSONService,
      GlobalObject globalObject)
        {
            this.configuration = configuration;
            this.storageJSONService = storageJSONService;
            this.globalObject = globalObject;

        }

        public async Task<string> GetEmrServiceUrl(SettingModel setting = null)
        {
            if (setting == null)
            {
                setting = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
            }
            if (string.IsNullOrEmpty(setting.EndPoint))
            {
                return configuration.GetRequiredSection("AppSettings").Get<AppSettings>().EmrService;
            }
            else
            {
                return setting.EndPoint;
            }
        }

        /// <summary>
        /// 取得是否為工程模式
        /// </summary>
        /// <param name="setting">不需要帶此參數</param>
        /// <returns></returns>
        public async Task<bool> GetEngineerModeAsync(SettingModel setting = null)
        {
            if (setting == null)
            {
                setting = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
            }
            return (setting.EngineerMode);
        }

        /// <summary>
        /// 取得未簽章清單的時間區間
        /// </summary>
        /// <param name="setting">不需要帶此參數</param>
        /// <returns></returns>
        public async Task<string> GetUnsignListDateRangeAsync(SettingModel setting = null)
        {
            return globalObject.config.FirstOrDefault(x => x.configKey == MagicValueHelper.行動簽章時間限制)?.configValue;
        }

        /// <summary>
        /// 行動簽章自動執行間隔時間(秒)
        /// </summary>
        /// <param name="setting">不需要帶此參數</param>
        /// <returns></returns>
        public async Task<string> GetRepeatIntervalAsync(SettingModel setting = null)
        {
            if (setting == null)
            {
                setting = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
            }
            if (string.IsNullOrEmpty(setting.IntervalSeconds))
            {
                return globalObject.config.FirstOrDefault(x => x.configKey == MagicValueHelper.行動簽章自動執行間隔時間)?.configValue;
            }
            else
            {
                return setting.IntervalSeconds;
            }
        }

        /// <summary>
        /// 取得醫院代碼
        /// </summary>
        /// <param name="setting">不需要帶此參數</param>
        /// <returns></returns>
        public async Task<string> GetTenantCodeAsync(SettingModel setting = null)
        {
            if (setting == null)
            {
                setting = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
            }
            if (string.IsNullOrEmpty(setting.Hid))
            {
                //return configuration.GetRequiredSection("AppSettings").Get<AppSettings>().TenantCode;
                return globalObject.TenantCode;
            }
            else
            {
                return setting.Hid;
            }
        }

        /// <summary>
        /// 取得醫院名稱
        /// </summary>
        /// <param name="setting">不需要帶此參數</param>
        /// <returns></returns>
        public async Task<string> GetAppHosNameAsync(SettingModel setting = null)
        {
            if (setting == null)
            {
                setting = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
            }
            if (string.IsNullOrEmpty(setting.Name))
            {
                return configuration.GetRequiredSection("AppSettings").Get<AppSettings>().AppHospName;
            }
            else
            {
                return setting.Name;
            }
        }

        /// <summary>
        /// 取得報告列表名稱
        /// </summary>
        /// <param name="setting">不需要帶此參數</param>
        /// <returns></returns>
        public async Task<string> GetReportListNameAsync(SettingModel setting = null)
        {
            if (setting == null)
            {
                setting = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
            }
            if (string.IsNullOrEmpty(setting.ReportListName))
            {
                return configuration.GetRequiredSection("AppSettings").Get<AppSettings>().UnsingListName;
            }
            else
            {
                return setting.ReportListName;
            }
        }

        /// <summary>
        /// 取得設定模型
        /// </summary>
        /// <returns></returns>
        public async Task<SettingModel> GetSettingModelAsync()
        {
            var setting = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
            var model = new SettingModel();
            model.EndPoint = await GetEmrServiceUrl(setting);
            model.IntervalSeconds = await GetRepeatIntervalAsync(setting);
            model.Hid = await GetTenantCodeAsync(setting);
            model.Name = await GetAppHosNameAsync(setting);
            model.ReportListName = await GetReportListNameAsync(setting);
            model.EngineerMode = await GetEngineerModeAsync(setting);

            return model;
        }

        /// <summary>
        /// 取得簽張失敗紀錄日期區間
        /// </summary>
        /// <returns></returns>
        public int GetFailLogRange()
        {
            string range = globalObject.config.FirstOrDefault(x => x.configKey == MagicValueHelper.簽張失敗紀錄時間限制)?.configValue;
            if (int.TryParse(range, out int result))
            {
                return result;
            }
            else return 0;
            
        }

        /// <summary>
        /// 取得簽章HashFlag
        /// </summary>
        /// <returns></returns>
        public string GetHashFlag()
        {
            return globalObject.config.FirstOrDefault(x => x.configKey == MagicValueHelper.簽章Hash型態)?.configValue;
        }

        /// <summary>
        /// 是否顯示報告內容
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetShowReportContentAsync()
        {
            var value = globalObject.config.FirstOrDefault(x => x.configKey == MagicValueHelper.是否顯示報告內容)?.configValue;
            if (value == "Y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public OperatorTypeEnum GetOperatorType()
        {
            return OperatorTypeEnum.手機端使用者;

        }

        public List<BusinessTypeEnum> GetBusinessTypes()
        {
            return new List<BusinessTypeEnum>() { BusinessTypeEnum.其他, BusinessTypeEnum .簽章};

        }
    }
}
