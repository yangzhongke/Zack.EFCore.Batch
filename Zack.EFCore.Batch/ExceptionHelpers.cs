using System.Globalization;

namespace Zack.EFCore.Batch_NET7
{
    public static class ExceptionHelpers
    {
        public static Exception CreateBatchNotSupportException_InEF7()
        {
            if(CultureInfo.CurrentCulture.Name.StartsWith("zh-"))
            {
                return new NotImplementedException("因为 EFCore 7 中已经有了内置的'批量删除和更新'的功能, 这个库将不会在EFCore7及更高版本中支持'批量删除和更新'的功能，但是批量插入BulkInsert功能仍然支持。请访问 https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates ");
            }
            else
            {
                return new NotImplementedException("Since EFCore 7 has built-in support of 'batch update and delete', this library doesn't support EFCore 7 and higher anymore, but BulkInsert is still supported. Please see https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates ");
            }            
        }
    }
}
