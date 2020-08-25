using System;
using System.Linq.Expressions;
using System.Reflection;

namespace KProcess.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ExtractFrom<T>
    {
        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MemberName(Expression<Func<T, object>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName(Expression<Func<T, Func<object>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1>(Expression<Func<T, Func<T1, object>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1>(Expression<Func<T, Func<T1, int>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1, T2>(Expression<Func<T, Func<T1, T2, object>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1, T2>(Expression<Func<T, Func<T1, T2, int>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1, T2, T3>(Expression<Func<T, Func<T1, T2, T3, object>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1, T2, T3>(Expression<Func<T, Func<T1, T2, T3, int>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1, T2, T3, T4>(Expression<Func<T, Func<T1, T2, T3, T4, object>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1, T2, T3, T4>(Expression<Func<T, Func<T1, T2, T3, T4, int>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1, T2, T3, T4, T5>(Expression<Func<T, Func<T1, T2, T3, T4, T5, object>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member name from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MethodName<T1, T2, T3, T4, T5>(Expression<Func<T, Func<T1, T2, T3, T4, T5, int>>> memberExpression)
        {
            return memberExpression.GetMemberName();
        }

        /// <summary>
        /// Extracts the member path from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to a member access to parse</param>
        /// <returns>THe name of the member</returns>
        public static string MemberPath(Expression<Func<T, object>> memberExpression)
        {
            return memberExpression.GetMemberPath();
        }

        /// <summary>
        /// Extracts the member info from a source object type
        /// </summary>
        /// <param name="memberExpression">The expression to parse</param>
        /// <returns>The instance of the <see cref="MemberInfo"/> related to the expression path</returns>
        public static MemberInfo MemberInfo(Expression<Func<T, object>> memberExpression)
        {
            return memberExpression.GetMemberInfo();
        }
    }
}