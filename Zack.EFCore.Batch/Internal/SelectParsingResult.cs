using System.Collections.Generic;

namespace Zack.EFCore.Batch.Internal
{
    public class SelectParsingResult
    {
        /// <summary>
        /// parameters of query
        /// </summary>
        public IDictionary<string, object> Parameters { get; internal set; }
       

        public string Schema
        {
            get;
            internal set;
        }

        public string TableName
        {
            get;
            internal set;
        }

        /// <summary>
        /// columns of select
        /// </summary>
        public IEnumerable<string> ProjectionSQL
        {
            get;
            internal set;
        }

        /// <summary>
        /// where clause
        /// </summary>

        public string PredicateSQL
        {
            get;
            internal set;
        }

        public string FullSQL
        {
            get;
            internal set;
        }

        public IZackQuerySqlGenerator QuerySqlGenerator { get; set; }
    }
}
