using System.Collections.Generic;

namespace Zack.EFCore.Batch
{
    public class SelectParsingResult
    {
        public string SelectSql { get; internal set; }
        public IReadOnlyDictionary<string, object> Parameters { get; internal set; }
        public string TableName
        {
            get;
            internal set;
        }
        public string TableAlias
        {
            get;
            internal set;
        }
        public IEnumerable<string> ProjectionSQL
        {
            get;
            internal set;
        }

        public string PredicateSQL
        {
            get;
            internal set;
        }

        public string AliasSeparator
        {
            get;
            internal set;
        }
    }
}
