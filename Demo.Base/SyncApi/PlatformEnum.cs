using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base.SyncApi
{
    /// <summary>
    /// PlatformEnum  
    /// Author：Samuel
    /// CreateTime：2021/5/22 16:47:47 
    /// </summary>
    public enum PlatformEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Default = 0,

        /// <summary>
        /// 天猫
        /// </summary>
        [Description("天猫")]
        Tmall = 101,

        /// <summary>
        /// 京东
        /// </summary>
        [Description("京东")]
        JD = 102,

        /// <summary>
        /// 拼多多
        /// </summary>
        [Description("拼多多")]
        PDD = 103,

        /// <summary>
        /// 药房网
        /// </summary>
        [Description("药房网")]
        YaoFangWang = 104,

        /// <summary>
        /// 饿了么
        /// </summary>
        [Description("饿了么")]
        ELeMe = 105,

        /// <summary>
        /// 美团
        /// </summary>
        [Description("美团")]
        MeiTuan = 106,

        /// <summary>
        /// 健康之路
        /// </summary>
        [Description("健康之路")]
        JianKangZhiLu = 107,

        /// <summary>
        /// 京东到家
        /// </summary>
        [Description("京东到家")]
        JDDJ = 108,

        /// <summary>
        /// 唯品会
        /// </summary>
        [Description("唯品会")]
        Vip = 109,

        /// <summary>
        /// 好大夫
        /// </summary>
        [Description("好大夫")]
        HaoDF = 110,

        /// <summary>
        /// 好心情
        /// </summary>
        [Description("好心情")]
        HaoXinQing = 111,

        /// <summary>
        /// 药联
        /// </summary>
        [Description("药联")]
        UnionDrug = 112,

        /// <summary>
        /// 药急送
        /// </summary>
        [Description("药急送")]
        MedicineRushedTo = 113,

        /// <summary>
        /// 有赞
        /// </summary>
        [Description("有赞")]
        YouZan = 114,

        /// <summary>
        /// 平安医生
        /// </summary>
        [Description("平安医生")]
        PingAnDoctor = 115,

        /// <summary>
        /// 微医
        /// </summary>
        [Description("微医")]
        WeDoctor = 116,

        /// <summary>
        /// 萌推
        /// </summary>
        [Description("萌推")]
        MengTui = 117,

        /// <summary>
        /// 苏宁
        /// </summary>
        [Description("苏宁")]
        Suning = 118,

        /// <summary>
        /// 百度健康
        /// </summary>
        [Description("百度健康")]
        BaiduHealth = 119,

        /// <summary>
        /// 蜜橙好医
        /// </summary>
        [Description("蜜橙好医")]
        MCGoodDoctor = 120,

        /// <summary>
        /// 蜜橙健康
        /// </summary>
        [Description("蜜橙健康")]
        MCHealth = 121,

        /// <summary>
        /// 智慧E保
        /// </summary>
        [Description("智慧E保")]
        ZHYB = 122,

        /// <summary>
        /// 药精选
        /// </summary>
        [Description("药精选")]
        DrugSelection = 123,

        /// <summary>
        /// 云药库
        /// </summary>
        [Description("云药库")]
        CloudPharmacy = 124,

        /// <summary>
        /// 山西官网
        /// </summary>
        [Description("山西官网")]
        SXGW = 125,

        /// <summary>
        /// 轻松健康
        /// </summary>
        [Description("轻松健康")]
        QingSongHealth = 129,



        /// <summary>
        /// 美团-团好货
        /// </summary>
        [Description("美团-团好货")]
        MeiTuanTHH = 130

    }
}
