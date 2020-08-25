using KProcess.Ksmed.Business;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;

namespace KProcess.KL2.Business.Impl.Shared
{
    /// <summary>
    /// Permet d'effectuer des vérifications en débug
    /// </summary>
    internal static class ServicesDiagnosticsDebug
    {

        /// <summary>
        /// Vérifie que les entités spécifiées ne soient pas présentes dans le contexte.
        /// </summary>
        /// <param name="context">Les contexte ef</param>
        /// <param name="entities">Les entités.</param>
        [Conditional("DEBUG")]
        public static void CheckNotInContext(KsmedEntities context, IEnumerable<object> entities)
        {
            // S'assurer que pour les référentiels mergés, il n'y a pas de trace d'eux dans le contexte

            foreach (var entity in entities)
                Debug.Assert(!context.ObjectStateManager.TryGetObjectStateEntry(entity, out ObjectStateEntry entry));

        }


        /// <summary>
        /// Vérifie que les entités spécifiées soient dans l'état spécifié dans le contexte..
        /// </summary>
        /// <param name="context">Les contexte ef</param>
        /// <param name="expectedState">L'état attendu</param>
        /// <param name="entities">Les entités.</param>
        [Conditional("DEBUG")]
        public static void CheckObjectStateManagerState(KsmedEntities context, EntityState expectedState, params object[] entities)
        {
            foreach (var entity in entities)
            {
                Debug.Assert(context.ObjectStateManager.GetObjectStateEntry(entity).State == expectedState);
            }
        }


        /// <summary>
        /// Vérifie que les entités spécifiées soient dans l'état spécifié dans le contexte..
        /// </summary>
        /// <param name="context">Les contexte ef</param>
        /// <param name="expectedState">L'état attendu</param>
        /// <param name="entities">Les entités.</param>
        [Conditional("DEBUG")]
        public static void CheckObjectStateManagerState(KsmedEntities context, EntityState expectedState, IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                Debug.Assert(context.ObjectStateManager.GetObjectStateEntry(entity).State == expectedState);
            }
        }

        /// <summary>
        /// Vérifie l'état des référentiels, en base.
        /// </summary>
        [Conditional("DEBUG")]
        public static async void CheckReferentialsState()
        {
            // Règles
            // 1. Il n'y a pas de référentiel projet utilisé dans un projet différent de celui où il est déclaré
            // 2. Il n'y a pas de vidéos en POV sans ressource associée

            using (var context = ContextFactory.GetNewContext())
            {

                Referentials referentials = new Referentials
                {
                    Categories = await context.ActionCategories.ToArrayAsync(),
                    Resources = await context.Resources.ToArrayAsync(),
                    Ref1s = await context.Refs1.ToArrayAsync(),
                    Ref2s = await context.Refs2.ToArrayAsync(),
                    Ref3s = await context.Refs3.ToArrayAsync(),
                    Ref4s = await context.Refs4.ToArrayAsync(),
                    Ref5s = await context.Refs5.ToArrayAsync(),
                    Ref6s = await context.Refs6.ToArrayAsync(),
                    Ref7s = await context.Refs7.ToArrayAsync()
                };

                // 1. Il n'y a pas de référentiel projet utilisé dans deux projets différents
                foreach (var category in referentials.Categories.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.CategoryId == category.Id && a.Scenario.Project.ProcessId != category.ProcessId), category.Id.ToString());

                foreach (var resource in referentials.Resources.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                {
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.ResourceId == resource.Id && a.Scenario.Project.ProcessId != resource.ProcessId));
                    Debug.Assert(!await context.Videos.AnyAsync(v => v.DefaultResourceId == resource.Id && v.ProcessId != resource.ProcessId));
                }

                foreach (var ref1 in referentials.Ref1s.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.Ref1.Any(ar => ar.RefId == ref1.Id) && a.Scenario.Project.ProcessId != a.Scenario.Project.ProcessId));

                foreach (var ref2 in referentials.Ref2s.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.Ref2.Any(ar => ar.RefId == ref2.Id) && a.Scenario.Project.ProcessId != a.Scenario.Project.ProcessId));

                foreach (var ref3 in referentials.Ref3s.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.Ref3.Any(ar => ar.RefId == ref3.Id) && a.Scenario.Project.ProcessId != a.Scenario.Project.ProcessId));

                foreach (var ref4 in referentials.Ref4s.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.Ref4.Any(ar => ar.RefId == ref4.Id) && a.Scenario.Project.ProcessId != a.Scenario.Project.ProcessId));

                foreach (var ref5 in referentials.Ref5s.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.Ref5.Any(ar => ar.RefId == ref5.Id) && a.Scenario.Project.ProcessId != a.Scenario.Project.ProcessId));

                foreach (var ref6 in referentials.Ref6s.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.Ref6.Any(ar => ar.RefId == ref6.Id) && a.Scenario.Project.ProcessId != a.Scenario.Project.ProcessId));

                foreach (var ref7 in referentials.Ref7s.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                    Debug.Assert(!await context.KActions.AnyAsync(a => a.Ref7.Any(ar => ar.RefId == ref7.Id) && a.Scenario.Project.ProcessId != a.Scenario.Project.ProcessId));

                // 2. Il n'y a pas de vidéos avec une vue sans ressource associée
                Debug.Assert(!await context.Videos.AnyAsync(v => v.ResourceView != null && v.DefaultResourceId == null));
            }
        }



    }
}
