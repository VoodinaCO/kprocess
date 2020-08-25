using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KProcess.Common
{
    /// <summary>
    /// Définit le comportement d'un Getter/Setter de propriété.
    /// </summary>
    public interface IPropertySelector
    {
        /// <summary>
        /// Obtient la valeur de la propriété.
        /// </summary>
        /// <param name="instance">L'instance.</param>
        /// <returns>La valeur</returns>
        object GetValue(object instance);

        /// <summary>
        /// Définit la valeur de la propriété.
        /// </summary>
        /// <param name="instance">L'instance.</param>
        /// <param name="value">La valeur.</param>
        void SetValue(object instance, object value);
    }

    /// <summary>
    /// Représente un Getter/Setter de propriété.
    /// </summary>
    /// <typeparam name="T">Le type qui héberge la propriété.</typeparam>
    /// <typeparam name="R">Le type de la valeur de la propriété.</typeparam>
    public class PropertySelector<T, R> : IPropertySelector
    {
        internal Func<T, R> selector;
        internal Action<T, R> setter;

        /// <summary>
        /// Obtient la valeur de la propriété.
        /// </summary>
        /// <param name="source">L'instance.</param>
        /// <returns>La valeur</returns>
        public R GetValue(T source)
        {
            return selector(source);
        }

        /// <summary>
        /// Définit la valeur de la propriété.
        /// </summary>
        /// <param name="source">L'instance.</param>
        /// <param name="value">La valeur.</param>
        public void SetValue(T source, R value)
        {
            setter(source, value);
        }

        #region IPropertySelector Members

        /// <summary>
        /// Obtient la valeur de la propriété.
        /// </summary>
        /// <param name="instance">L'instance.</param>
        /// <returns>La valeur</returns>
        object IPropertySelector.GetValue(object instance)
        {
            return GetValue((T)instance);
        }

        /// <summary>
        /// Définit la valeur de la propriété.
        /// </summary>
        /// <param name="instance">L'instance.</param>
        /// <param name="value">La valeur.</param>
        void IPropertySelector.SetValue(object instance, object value)
        {
            SetValue((T)instance, (R)value);
        }

        #endregion

        /// <summary>
        /// Représente un visiteur de propriété.
        /// </summary>
        public class PropertyVisitor : ExpressionVisitor
        {
            /// <summary>
            /// Créee un visiteur à partir d'une expression.
            /// </summary>
            /// <param name="source">La source du visiteur.</param>
            /// <returns>Le sélecteur.</returns>
            public PropertySelector<T, R> Compile(Expression<Func<T, R>> source)
            {
                this.source = source;

                parameters = new List<ParameterExpression>();

                Visit(source);

                var valueParameter = Expression.Parameter(typeof(R), "value");
                parameters.Add(valueParameter);
                currentNode = Expression.Assign(currentNode, valueParameter);
                var setter = Expression.Lambda(typeof(Action<T, R>), currentNode, parameters);

                var result = new PropertySelector<T, R>() { selector = source.Compile(), setter = (Action<T, R>)setter.Compile() };

                return result;
            }

            private LambdaExpression source;
            private Expression currentNode;
            private List<ParameterExpression> parameters;

            /// <inheritdoc />
            protected override System.Linq.Expressions.Expression VisitMemberAccess(System.Linq.Expressions.MemberExpression m)
            {
                var result = base.VisitMemberAccess(m);

                currentNode = Expression.MakeMemberAccess(currentNode, m.Member);

                return result;
            }

            /// <inheritdoc />
            protected override Expression VisitParameter(ParameterExpression p)
            {
                var result = base.VisitParameter(p);

                var existingParam = (parameters.Find(param => param.Name == p.Name));
                if (existingParam == null)
                {
                    existingParam = Expression.Parameter(p.Type, p.Name);
                    parameters.Add(existingParam);
                }
                currentNode = existingParam;

                return result;
            }

            /// <inheritdoc />
            protected override Expression Visit(Expression exp)
            {
                if ((exp.NodeType != ExpressionType.Parameter) &&
                    (exp.NodeType != ExpressionType.Lambda) &&
                    ((exp.NodeType != ExpressionType.MemberAccess) && ((exp as MemberExpression).Member.MemberType == System.Reflection.MemberTypes.Property)))
                    throw new ArgumentException("Only Properties are allowed with PropertyPath");
                return base.Visit(exp);
            }
        }
    }

    /// <summary>
    /// Contient une fabrique de Getter/Setter de propriété.
    /// </summary>
    /// <typeparam name="T">Le type hébergeant la propriété.</typeparam>
    public class PropertySelector<T>
    {
        /// <summary>
        /// Crée un Getter/Setter de propriété
        /// </summary>
        /// <typeparam name="R">Le type de la valeur de la propriété.</typeparam>
        /// <param name="source">L'expression permettant d'accéder à la propriété..</param>
        /// <returns></returns>
        public static PropertySelector<T, R> Create<R>(Expression<Func<T, R>> source)
        {
            var v = new PropertySelector<T, R>.PropertyVisitor();
            return v.Compile(source);
        }
    }
}
