using System.Collections.Generic;

namespace Zack.EFCore.Batch
{
    public class SelectParsingResult
    {
        /// <summary>
        /// parameters of query
        /// </summary>
        public IReadOnlyDictionary<string, object> Parameters { get; internal set; }
       
        
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
    }
}
