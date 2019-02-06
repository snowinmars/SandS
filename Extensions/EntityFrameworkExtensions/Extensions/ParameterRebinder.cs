using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkExtensions.Extensions
{
    // https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
    // mirror: O:\Projects\C2C\C2C\Documentation\CombiningPredicates
    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            if (map == null)
            {
                this.map = new Dictionary<ParameterExpression, ParameterExpression>();
            }
            else
            {
                this.map = map;
            }
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(parameter, out replacement))
            {
                parameter = replacement;
            }

            return base.VisitParameter(parameter);
        }
    }
}
