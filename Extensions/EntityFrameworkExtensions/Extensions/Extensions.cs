using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFrameworkExtensions.Extensions
{
    public static class Extensions
    {
        public static Expression<Func<T, bool>> WhereOr<T>(params Expression<Func<T, bool>>[] expressions)
        {
            if (expressions == null || expressions.Length == 0)
            {
                throw new InvalidOperationException("Can't compose less then 1 expressions");
            }

            Expression<Func<T, bool>> result = null;

            foreach (var expression in expressions)
            {
                result = result.WhereOr(expression);
            }

            return result;
        }

        public static Expression<Func<T, bool>> WhereOr<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return Compose(first, second, Expression.Or);
        }

        private static Expression<T> Compose<T>(Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            if (first == null && second == null)
            {
                throw new ArgumentNullException("first or second", "Can't compose nullable expressions");
            }

            if (first == null)
            {
                return second;
            }

            if (second == null)
            {
                return first;
            }

            // https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
            // mirror: O:\Projects\C2C\C2C\Documentation\CombiningPredicates
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        // EF generates IN (...) SQL expression for Contains.
        // Downsides here:
        // - EF has to recompile query every time
        // - SQL Server has to recompile query plan for all IN (..) combinations of particular query; which also can lead to purging of already cached query plans
        //
        // We're forcing EF to replace IN with @variables, so allowing query plan caching
        public static IQueryable<TEntity> ContainsAsVariables<TEntity>(this IQueryable<TEntity> query, int[] values, Expression<Func<TEntity, int>> fieldSelector)
        {
            return query.Where(ContainsAsVariablesExpression(values, fieldSelector));
        }

        public static IQueryable<TEntity> NotContainsAsVariables<TEntity>(this IQueryable<TEntity> query, int[] values, Expression<Func<TEntity, int>> fieldSelector)
        {
            return query.Where(ContainsAsVariablesExpression(values, fieldSelector).Not());
        }

        public static IQueryable<TEntity> ContainsAsVariables<TEntity>(this IQueryable<TEntity> query, string[] values, Expression<Func<TEntity, string>> fieldSelector)
        {
            return query.Where(ContainsAsVariablesExpression(values, fieldSelector));
        }

        public static IQueryable<TEntity> NotContainsAsVariables<TEntity>(this IQueryable<TEntity> query, string[] values, Expression<Func<TEntity, string>> fieldSelector)
        {
            return query.Where(ContainsAsVariablesExpression(values, fieldSelector).Not());
        }

        private static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters[0]);
        }

        private static Expression<Func<TEntity, bool>> ContainsAsVariablesExpression<TEntity>(int[] values, Expression<Func<TEntity, int>> fieldSelector)
        {
            // For 1..15   params, generate 15 variables
            // For 16..100 params, generate 100 variables
            // For exact 1000 params, generate 1000 variables
            // Otherwise, use EF hardcoded IN(consts) 
            int variablesCount = values.Length <= 15 ? 15 : values.Length <= 100 ? 100 : values.Length == 1000 ? 1000 : -1;

            // [x] =>
            var sharedParameter = Expression.Parameter(typeof(TEntity));

            // [entity] => [entity].Join1.Join2....Id
            var memberExpression = (MemberExpression)fieldSelector.Body;
            MemberExpression memberAccessExpression = GetMemberAccessExpression(sharedParameter, memberExpression);

            // Generate IN (const1, const2, ...)
            if (variablesCount < 0)
            {
                // [x] => values.Contains([x])
                Expression<Func<int, bool>> containsExpression = x => values.Contains(x);
                var containsMethodInfo = ((MethodCallExpression)containsExpression.Body).Method;
                var valuesExpression = Expression.Constant(values, typeof(int[]));
                var callExpr = Expression.Call(null, containsMethodInfo, valuesExpression, memberAccessExpression);
                return Expression.Lambda<Func<TEntity, bool>>(callExpr, sharedParameter);
            }

            // Generate IN (@var1, @var2, ...)
            var variablesArray = Enumerable.Repeat(-100, variablesCount).ToArray();
            Array.Copy(values, variablesArray, values.Length);

            var whereExpression = variablesArray.Select(value =>
            {
                var currentValue = value;

                Expression<Func<int, bool>> equalsExpression = x => x.Equals(currentValue);
                var methodCallExpression = (MethodCallExpression)equalsExpression.Body;

                var call = Expression.Call(memberAccessExpression, methodCallExpression.Method, methodCallExpression.Arguments);

                return Expression.Lambda<Func<TEntity, bool>>(call, sharedParameter);
            })
                .Aggregate((left, right) => left == null ? right : Expression.Lambda<Func<TEntity, bool>>(Expression.OrElse(left.Body, right.Body), left.Parameters));

            return whereExpression;
        }

        private static Expression<Func<TEntity, bool>> ContainsAsVariablesExpression<TEntity>(string[] values, Expression<Func<TEntity, string>> fieldSelector)
        {
            // For 1..15   params, generate 15 variables
            // For 16..100 params, generate 100 variables
            // For exact 1000 params, generate 1000 variables
            // Otherwise, use EF hardcoded IN(consts) 
            int variablesCount = values.Length <= 15 ? 15 : values.Length <= 100 ? 100 : values.Length == 1000 ? 1000 : -1;

            // [x] =>
            var sharedParameter = Expression.Parameter(typeof(TEntity));

            // [entity] => [entity].Join1.Join2....Id
            var memberExpression = (MemberExpression)fieldSelector.Body;
            MemberExpression memberAccessExpression = GetMemberAccessExpression(sharedParameter, memberExpression);

            // Generate IN (const1, const2, ...)
            if (variablesCount < 0)
            {
                // [x] => values.Contains([x])
                Expression<Func<string, bool>> containsExpression = x => values.Contains(x);
                var containsMethodInfo = ((MethodCallExpression)containsExpression.Body).Method;
                var valuesExpression = Expression.Constant(values, typeof(string[]));
                var callExpr = Expression.Call(null, containsMethodInfo, valuesExpression, memberAccessExpression);
                return Expression.Lambda<Func<TEntity, bool>>(callExpr, sharedParameter);
            }

            // Generate IN (@var1, @var2, ...)
            var variablesArray = Enumerable.Repeat(default(string), variablesCount).ToArray();
            Array.Copy(values, variablesArray, values.Length);

            var whereExpression = variablesArray.Select(value =>
            {
                var currentValue = value;

                Expression<Func<string, bool>> equalsExpression = x => x.Equals(currentValue);
                var methodCallExpression = (MethodCallExpression)equalsExpression.Body;

                var call = Expression.Call(memberAccessExpression, methodCallExpression.Method, methodCallExpression.Arguments);

                return Expression.Lambda<Func<TEntity, bool>>(call, sharedParameter);
            })
                .Aggregate((left, right) => left == null ? right : Expression.Lambda<Func<TEntity, bool>>(Expression.OrElse(left.Body, right.Body), left.Parameters));

            return whereExpression;
        }

        private static MemberExpression GetMemberAccessExpression(ParameterExpression parameterExpression, MemberExpression memberExpression)
        {
            var innerMemberExpression = memberExpression.Expression as MemberExpression;
            if (innerMemberExpression == null)
            {
                return Expression.MakeMemberAccess(parameterExpression, memberExpression.Member);
            }

            var innerMemberType = memberExpression.Member.DeclaringType;
            var innerParameter = Expression.Parameter(innerMemberType);

            var innerMemberAccess = Expression.MakeMemberAccess(innerParameter, memberExpression.Member);

            return Expression.MakeMemberAccess(GetMemberAccessExpression(parameterExpression, innerMemberExpression), innerMemberAccess.Member);
        }
    }
}
