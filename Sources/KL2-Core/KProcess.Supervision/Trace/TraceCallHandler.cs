using Microsoft.Practices.Unity.InterceptionExtension;

namespace KProcess.Supervision.Trace
{
    /// <summary>
    /// Représente un intercepteurs de méthode Unity se charger de tracer l'appel.
    /// </summary>
    public class TraceCallHandler : CallHandlerBase
    {
        private System.Diagnostics.Stopwatch _stopwatch;

        /// <summary>
        /// Méthode de traitement avant appel
        /// </summary>
        /// <param name="pInput">Description de l'appel intercepté</param>
        /// <returns>null pour poursuivre le traitement (défaut), tout autre valeur intercepte l'appel</returns>
        protected override IMethodReturn PreProcessHandler(IMethodInvocation pInput)
        {
            string className = pInput.Target.GetType().BaseType.Name;
            string methodName = pInput.MethodBase.Name;
            string generic = pInput.MethodBase.DeclaringType.IsGenericType ? string.Format("<{0}>", pInput.MethodBase.DeclaringType.GetGenericArguments().ToStringList()) : string.Empty;
            string arguments = pInput.Arguments.ToStringList();

            pInput.Target.TraceDebug(string.Format("=> {0}{1}.{2}({3})", className, generic, methodName, arguments));

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();

            return base.PreProcessHandler(pInput);
        }

        /// <summary>
        /// Méthode de traitement après appel
        /// </summary>
        /// <param name="pInput">Description de l'appel intercepté</param>
        /// <param name="pReturn">Description du résultat de l'appel</param>
        /// <returns>Résultat final de l'appel</returns>
        protected override IMethodReturn PostProcessHandler(IMethodInvocation pInput, IMethodReturn pReturn)
        {
            _stopwatch.Stop();

            string className = pInput.Target.GetType().BaseType.Name;
            string methodName = pInput.MethodBase.Name;
            string generic = pInput.MethodBase.DeclaringType.IsGenericType ? string.Format("<{0}>", pInput.MethodBase.DeclaringType.GetGenericArguments().ToStringList()) : string.Empty;

            var res = base.PostProcessHandler(pInput, pReturn);

            pInput.Target.TraceDebug(string.Format("<= {0}{1}.{2}() : {3} in {4} s", className, generic, methodName, res.ReturnValue, _stopwatch.Elapsed.TotalSeconds));

            return res;
        }
    }
}