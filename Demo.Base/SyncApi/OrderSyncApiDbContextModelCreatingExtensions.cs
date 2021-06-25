using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Demo.Base.SyncApi
{
    public static class OrderSyncApiDbContextModelCreatingExtensions
    {
        public static void ConfigureOrderSyncApi(
            this ModelBuilder builder,
            Action<OrderSyncApiModelBuilderConfigurationOptions> optionsAction = null)
        {

            var options = new OrderSyncApiModelBuilderConfigurationOptions(
                OrderSyncApiDbProperties.DbTablePrefix,
                OrderSyncApiDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);


            // 平台订单的核心表
            builder.Entity<PlatformOrderInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "PlatformOrderInfos", options.Schema);
                b.HasComment("平台订单的核心表");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasComment("表主键");
                b.Property(x => x.ChannelCode).HasComment("平台/渠道编码").IsRequired().HasConversion<int>();
                b.Property(x => x.GetOrderType).HasComment("获取订单的方式 （1、Job同步；2、三方推送；3、手工触发）").HasConversion<sbyte>();
                b.Property(x => x.ShopCode).HasComment("店铺编号").HasMaxLength(32);
                b.Property(x => x.ShopName).HasComment("店铺名称").HasMaxLength(64);
                b.Property(x => x.PlatformShopCode).HasComment("三方合作渠道店铺编码").HasMaxLength(128);
                b.Property(x => x.PlatformOrderNo).HasComment("平台订单号").HasMaxLength(128);
                b.Property(x => x.PlatformOrderStatus).HasComment("平台订单状态").HasMaxLength(64);
                b.Property(x => x.OriginalOrderJson).HasComment("原始订单Json串").HasColumnType("json");
                b.Property(x => x.State).HasComment("抓单状态（0、默认待入库；1、待抓取明细；2、待转Eto；3、拉取明细失败；4、待生成订单；5、手动完成；6、完成；7、未决，待人工处理）");
                b.Property(x => x.RxAuditState).HasComment("处方审核状态（0、默认，1、已审核，2、未审核）").HasDefaultValue(RxAuditStateEnum.Default).HasConversion<sbyte>();
                b.Property(x => x.OrderState)
                    .HasComment(
                        "订单状态（业务状态--> 0：默认处理中；1、等待买家代付；2、买家付款后取消；3：买家关闭订单；4、超时未完成；5、待发货待审未审；6、退款中；7、已退款；8、内部完成；9、订单完成，生命周期结束）");
                b.Property(x => x.HandleState).HasComment("处理状态  默认 0 处理中  1 成功完成 2、失败完成").HasDefaultValue(0);
                b.Property(x => x.Remark).HasComment("备注/描述").HasMaxLength(255);

                b.Property(x => x.IsDeleted).HasComment("是否删除");
                b.Property(x => x.DeleterId).HasComment("删除人ID");
                b.Property(x => x.DeletionTime).HasComment("删除时间");
                b.Property(x => x.DeleterName).HasComment("删除人姓名");
                /*
                b.Property(x => x.LastModificationTime).HasComment("修改时间");
                b.Property(x => x.LastModifierId).HasComment("修改人ID");
                b.Property(x => x.LastModifierName).HasComment("修改人");
                b.Property(x => x.CreatorName).HasComment("创建人");
                b.Property(x => x.CreatorId).HasComment("创建人ID");
                b.Property(x => x.CreationTime).HasComment("创建时间");*/

                // 设置索引
                b.HasIndex(x => new { x.Id });
                b.HasIndex(x => new { x.PlatformOrderNo });
                /*
                b.HasIndex(x => new { x.CreationTime });
                b.HasIndex(x => new { x.LastModificationTime });
                b.HasIndex(x => new { x.CreationTime, x.LastModificationTime });
                */
                b.ConfigureByConvention();
                /* Configure more properties here */
            });
        }
    }
}
