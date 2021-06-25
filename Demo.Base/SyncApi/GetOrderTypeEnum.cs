using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base.SyncApi
{
    /// <summary>
    /// GetOrderTypeEnum  
    /// Author：Samuel
    /// CreateTime：2021/5/24 16:12:14 
    /// </summary>
    public enum GetOrderTypeEnum
    {
        /// <summary>
        /// 未知，默认
        /// </summary>
        [Description("未知，默认")]
        Default = 0,

        /// <summary>
        /// Job同步
        /// </summary>
        [Description("Job同步")]
        JobSync = 1,

        /// <summary>
        /// 三方/合作方推送
        /// </summary>
        [Description("三方/合作方推送")]
        PartnerPush = 2,

        /// <summary>
        /// 手工触发
        /// </summary>
        [Description("手工触发")]
        Handle = 3
    }
}
