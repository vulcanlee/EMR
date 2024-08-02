using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Enums
{
    public enum BusinessTypeEnum
    {
        其他 = 0,
        新增 = 1,
        修改 = 2,
        刪除 = 3,
        授權 = 4,
        匯出 = 5,
        匯入 = 6,
        強退 = 7,
        生成代碼 = 8,
        清空數據 = 9,
        簽章 = 10,
    }
}
