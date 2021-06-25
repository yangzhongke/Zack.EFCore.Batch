using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base.SyncApi
{
    public abstract class DkFullAuditedEntity<TKey> 
    {

        public TKey Id { get; set; }
        public virtual bool IsDeleted
        {
            get;
            set;
        }

        public virtual Guid? DeleterId
        {
            get;
            set;
        }

        public virtual DateTime? DeletionTime
        {
            get;
            set;
        }

        public virtual string DeleterName { get; set; }

        protected DkFullAuditedEntity()
        {
        }

        protected DkFullAuditedEntity(TKey id)
        {
            this.Id = id;
        }
    }
}
