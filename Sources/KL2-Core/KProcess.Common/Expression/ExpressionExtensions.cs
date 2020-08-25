using System;
using System.Linq.Expressions;
using System.Reflection;

namespace KProcess.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Extracts the member expression from a Lambda expression
        /// </summary>
        /// <remarks>
        /// Body of the lambda should Member or unary expression.
        /// It doesn't work for method member, it is actually a methodCallExpression within a ConstantExpression
        /// </remarks>
        /// <param name="expression">The lambda expression pointing out a member</param>
        /// <returns>The extracted member expression</returns>
        public static MemberExpression GetMemberExpression(this LambdaExpression expression)
        {
            if (expression.Body is MemberExpression)
            {
                return (MemberExpression)expression.Body;
            }
            else if (expression.Body is UnaryExpression)
            {
                return (MemberExpression)((UnaryExpression)expression.Body).Operand;
            }

            throw new NotSupportedException(string.Format("Unable to find member name of such expression body: {0}", expression.Body.GetType()));
        }

        /// <summary>
        /// Extracts the member info from a Lambda expression
        /// </summary>
        /// <remarks>
        /// Body of the lambda should Member or unary expression
        /// </remarks>
        /// <param name="expression">The lambda expression pointing out a member</param>
        /// <returns>The extracted member info</returns>
        public static MemberInfo GetMemberInfo(this LambdaExpression expression)
        {
            if (expression.Body is MemberExpression)
            {
                return ((MemberExpression)expression.Body).Member;
            }
            else if (expression.Body is UnaryExpression)
            {
                var operand = ((UnaryExpression)expression.Body).Operand;
                if (operand is MethodCallExpression)
                {
                    var memberInfo = (MemberInfo)((ConstantExpression)((MethodCallExpression)operand).Object).Value;
                    return memberInfo;
                }
                else if (operand is MemberExpression)
                {
                    return ((MemberExpression)operand).Member;
                }
            }

            throw new NotSupportedException(string.Format("Unable to find member name of such expression body: {0}", expression.Body.GetType()));
        }

        /// <summary>
        /// Extracts the member name from a Lambda expression
        /// </summary>
        /// <remarks>
        /// Body of the lambda should Member or unary expression
        /// </remarks>
        /// <param name="expression">The lambda expression pointing out a member</param>
        /// <returns>The extracted member name</returns>
        public static string GetMemberName(this LambdaExpression expression)
        {
            return expression.GetMemberInfo().Name;
        }

        /// <summary>
        /// Determines whether the expression has a member: In case expression is somthing like model => model, there is for example no model in this case.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool HasMember(this LambdaExpression expression)
        {
            return expression.Body is MemberExpression 
                || (expression.Body is UnaryExpression && ((UnaryExpression)expression.Body).Operand is MemberExpression);
        }

        /// <summary>
        /// Extracts the path from a lambda expression
        /// </summary>
        /// <param name="expression">THe lambda expression from which the path is extracted</param>
        /// <returns>The string path</returns>
        public static string GetMemberPath(this LambdaExpression expression)
        {
            return GetMemberPath(expression.Body);
        }


        /// <summary>
        /// Extracts the path from a lambda expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static string GetMemberPath(Expression expression, string basePath = "")
        {
            MemberExpression memberExpression = null;
            if (expression is MemberExpression)
            {
                memberExpression = (MemberExpression)expression;
            }
            else if (expression is UnaryExpression)
            {
                memberExpression = (MemberExpression)((UnaryExpression)expression).Operand;
            }
            else
            {
                return basePath;
            }
            
            var memberName = memberExpression.Member.Name;
            return string.IsNullOrWhiteSpace(basePath) 
                ? GetMemberPath(memberExpression.Expression, memberName)
                : GetMemberPath(memberExpression.Expression, string.Format("{0}.{1}", memberName, basePath));
        }
    }
}