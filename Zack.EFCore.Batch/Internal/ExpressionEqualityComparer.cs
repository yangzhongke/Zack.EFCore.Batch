using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zack.EFCore.Batch.Internal
{
    class ExpressionEqualityComparer : IEqualityComparer<Expression>,
        IComparer<Expression>
    {
        public int Compare(Expression x, Expression y)
        {
            return Equals(x, y) ? 0 : 1;
        }

        public bool Equals(Expression x, Expression y)
        {
            if(x==null&&y==null)
            {
                return true;
            }
            else if(x==null||y==null)
            {
                return false;
            }
            else if(x.NodeType!=y.NodeType)
            {
                return false;
            }
            else
            {
                return x.ToString() == y.ToString();
            }
        }

        public int GetHashCode(Expression obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
