using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base.SyncApi
{
    /// <summary>
    /// PlatformOrderInfo  平台订单的核心表
    /// Author：Samuel
    /// CreateTime：2021/5/24 15:57:28 
    /// </summary>
    public class PlatformOrderInfo : DkFullAuditedEntity<Guid>
    {
        /// <summary>
        /// 平台/渠道编码 如：101 天猫
        /// </summary>
        [Description("平台/渠道编码")]
        public PlatformEnum ChannelCode { get; set; }

        /// <summary>
        /// 获取订单的方式 （1、Job同步；2、三方推送；3、手工触发）
        /// </summary>
        [Description("获取订单的方式")]
        public GetOrderTypeEnum GetOrderType { get; set; }

        /// <summary>
        /// 店铺编号
        /// </summary>
        [Description("店铺编号")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Description("店铺名称")]
        public string ShopName { get; set; }

        /// <summary>
        /// 三方合作渠道店铺编码
        /// </summary>
        [Description("三方合作渠道店铺编码")]
        public string PlatformShopCode { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        [Description("平台订单号")]
        public string PlatformOrderNo { get; set; }

        /// <summary>
        /// 平台订单状态 (支付、取消等)，主要是直接记录平台给的状态
        /// </summary>
        [Description("平台订单状态")]
        public string PlatformOrderStatus { get; set; }

        /// <summary>
        /// 原始订单Json串
        /// </summary>
        [Description("原始订单Json串")]
        public string OriginalOrderJson { get; set; }

        /// <summary>
        /// 抓单状态（0、默认待入库；1、待抓取明细；2、待转Eto；3、待生成订单；4、完成）
        /// </summary>
        [Description("抓单状态（0、默认待入库；1、待抓取明细；2、待转Eto；3、待生成订单；4、完成）")]
        public sbyte State { get; set; }

        /// <summary>
        /// 订单状态（业务状态--> 0：默认处理中；1、等待买家代付；2、买家付款后取消；3：买家关闭订单；4、超时未完成；5、待发货待审未审；6、退款中；7、已退款；8、内部完成；9、订单完成，生命周期结束）
        /// </summary>
        [Description("订单状态（业务状态--> 0：默认处理中；1、等待买家代付；2、买家付款后取消；3：买家关闭订单；4、超时未完成；5、待发货待审未审；6、退款中；7、已退款；8、内部完成；9、订单完成，生命周期结束）")]
        public sbyte OrderState { get; set; }

        /// <summary>
        /// 处方审核状态（0、默认，1、已审核，2、未审核）
        /// </summary>
        [Description("处方状态")]
        public RxAuditStateEnum RxAuditState { get; set; }

        /// <summary>
        /// 处理状态  默认 0 处理中  1 成功完成 2、失败完成 
        /// </summary>
        [Description("处理状态  默认 0 处理中  1 成功完成 2、失败完成")]
        public sbyte HandleState { get; set; }

        /// <summary>
        /// 备注/描述
        /// </summary>
        [Description("备注/描述")]
        public string Remark { get; set; }


        protected PlatformOrderInfo()
        {
        }

        public PlatformOrderInfo(
            Guid id,
            PlatformEnum channelCode,
            GetOrderTypeEnum getOrderType,
            string shopCode,
            string shopName,
            string platformShopCode,
            string platformOrderNo,
            string platformOrderStatus,
            string originalOrderJson,
            sbyte state,
            sbyte orderState,
            RxAuditStateEnum rxAuditState
        ) : base(id)
        {
            ChannelCode = channelCode;
            GetOrderType = getOrderType;
            ShopCode = shopCode;
            ShopName = shopName;
            PlatformShopCode = platformShopCode;
            PlatformOrderNo = platformOrderNo;
            PlatformOrderStatus = platformOrderStatus;
            OriginalOrderJson = originalOrderJson;
            State = state;
            OrderState = orderState;
            RxAuditState = rxAuditState;
        }

        public PlatformOrderInfo(
            Guid id,
            sbyte state,
            sbyte handState,
            string remark
        ) : base(id)
        {

            State = state;
            this.HandleState = handState;
            Remark = remark;
        }
    }
}
