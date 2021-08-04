using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Base.Issue24
{
    [Index(nameof(Id), IsUnique = true)]
    public abstract class BaseEntity
    {
        ///

        /// 主键
        ///

        [Key]
        [Column("id", TypeName = "char(36)")]
        public Guid? Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time", TypeName = "datetime(6)")]
        [Comment("创建时间")]
        public DateTime? Create_Time { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [MaxLength(200)]
        [Column("create_user")]
        [Comment("创建者")]
        public string Create_User { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("update_time", TypeName = "datetime(6)")]
        [Comment("更新时间")]
        public DateTime? Update_Time { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [MaxLength(200)]
        [Column("update_user")]
        [Comment("更新者")]
        public string Update_User { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [Timestamp]
        [Column("row_rersion", TypeName = "timestamp(6)")]
        [Comment("行版本号")]
        public DateTime? Row_Rersion { get; set; }

        /// <summary>
        /// 是否软删除
        /// </summary>
        [Column("is_soft_deleted")]
        [Comment("是否软删除")]
        public bool Is_Soft_Deleted { get; set; }
    }
}
