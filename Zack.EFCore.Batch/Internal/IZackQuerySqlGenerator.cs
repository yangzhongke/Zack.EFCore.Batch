using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;

namespace Zack.EFCore.Batch.Internal
{
    public interface IZackQuerySqlGenerator
    {
		public bool IsForBatchEF { get; set; }

		public IEnumerable<string> ProjectionSQL
		{
			get;
		}

		/// <summary>
		/// the where clause
		/// </summary>
		public string PredicateSQL
		{
			get;
		}

		public IRelationalCommand GetCommand(SelectExpression selectExpression);
	}
}
