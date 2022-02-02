using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Emmy.Data.Extensions
{
    public class ExceptBuilder
    {
        private readonly List<(Type Type, PropertyInfo PropertyInfo)> _excepts = new();

        public void Except<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> exceptExpr)
        {
            var prop = GetPropertyInfo(exceptExpr);
            _excepts.Add((typeof(TEntity), prop));
        }

        internal bool IsExcepted(Type type, PropertyInfo propertyInfo)
        {
            return _excepts.Any(x => x.Type == type && x.PropertyInfo.Name == propertyInfo.Name);
        }

        private static PropertyInfo GetPropertyInfo<T, P>(Expression<Func<T, P>> property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (property.Body is MemberExpression memberExp)
            {
                return (PropertyInfo) memberExp.Member;
            }

            throw new ArgumentException("The expression doesn't indicate a valid property");
        }
    }
}