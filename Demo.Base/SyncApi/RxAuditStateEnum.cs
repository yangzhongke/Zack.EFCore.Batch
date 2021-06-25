using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base.SyncApi
{
    /// <summary>
    /// RaAuditStateEnum  
    /// Author：Samuel
    /// CreateTime：2021/5/25 15:04:41 
    /// </summary>
    public enum RxAuditStateEnum
    {
        /// <summary>
        /// 默认情况 则不存在处方
        /// </summary>
        [Description("默认，无")]
        Default = 0,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audit = 1,

        /// <summary>
        /// 未审核
        /// </summary>
        [Description("未审核")]
        UnAudit = 2
    }
}
