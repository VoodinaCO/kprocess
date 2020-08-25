using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.Ksmed.Business.Referentials
{
    /// <summary>
    /// Fournit des méthodes aidant à la gestion des référentiels.
    /// </summary>
    public static class ReferentialsHelper
    {
        /// <summary>
        /// Obtient tous les référentiels standard utilisés par le projet.
        /// </summary>
        /// <param name="p">Le projet.</param>
        /// <returns>les référentiels standard utilisés par le projet</returns>
        public static IEnumerable<IActionReferential> GetAllReferentialsStandardUsed(Project p)
        {
            IEnumerable<KAction> allActions = p.Scenarios.SelectMany(a => a.Actions);
            return GetAllReferentialsStandardUsed(allActions).Union(
                p.Process.Videos.Select(v => v.DefaultResource).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null));
        }

        /// <summary>
        /// Obtient tous les référentiels process du projet.
        /// </summary>
        /// <param name="p">Le projet.</param>
        /// <returns>les référentiels process du projet</returns>
        public static IEnumerable<IActionReferentialProcess> GetAllReferentialsProject(Project p)
        {
            return
                p.Process.ActionCategories.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Operators.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Equipments.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Refs1.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Refs2.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Refs3.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Refs4.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Refs5.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Refs6.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Refs7.OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                p.Process.Videos.Select(v => v.DefaultResource).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                )))))))));
        }

        /// <summary>
        /// Obtient tous les référentiels standard utilisés par les actions.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <returns>les référentiels projet utilisés par les actions</returns>
        public static IEnumerable<IActionReferential> GetAllReferentialsStandardUsed(IEnumerable<KAction> actions)
        {
            return
                actions.Select(a => a.Category).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.Select(a => a.Resource).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.SelectMany(a => a.Ref1.Select(c => c.Ref1)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.SelectMany(a => a.Ref2.Select(c => c.Ref2)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.SelectMany(a => a.Ref3.Select(c => c.Ref3)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.SelectMany(a => a.Ref4.Select(c => c.Ref4)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.SelectMany(a => a.Ref5.Select(c => c.Ref5)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.SelectMany(a => a.Ref6.Select(p => p.Ref6)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.SelectMany(a => a.Ref7.Select(t => t.Ref7)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null).Union(
                actions.Where(a => a.Video != null).Select(a => a.Video.DefaultResource).OfType<IActionReferentialProcess>().Where(r => r.ProcessId == null))
                ))))))));
        }

        /// <summary>
        /// Obtient tous les référentiels process utilisés par les actions.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <returns>les référentiels process utilisés par les actions</returns>
        public static IEnumerable<IActionReferentialProcess> GetAllReferentialsProject(IEnumerable<KAction> actions)
        {
            return
                actions.Select(a => a.Category).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.Select(a => a.Resource).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.SelectMany(a => a.Ref1.Select(c => c.Ref1)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.SelectMany(a => a.Ref2.Select(c => c.Ref2)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.SelectMany(a => a.Ref3.Select(c => c.Ref3)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.SelectMany(a => a.Ref4.Select(c => c.Ref4)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.SelectMany(a => a.Ref5.Select(c => c.Ref5)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.SelectMany(a => a.Ref6.Select(p => p.Ref6)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.SelectMany(a => a.Ref7.Select(t => t.Ref7)).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null).Union(
                actions.Where(a => a.Video != null).Select(a => a.Video.DefaultResource).OfType<IActionReferentialProcess>().Where(r => r.ProcessId != null))
                ))))))));
        }

        /// <summary>
        /// Met à jour les références vers le référentiel spécifié pour les actions.
        /// </summary>
        /// <param name="actions">Les actions/</param>
        /// <param name="oldReferential">L'ancien référentiel.</param>
        /// <param name="newReferential">Le nouveau référential à utiliser.</param>
        public static void UpdateReferentialReferences(IEnumerable<KAction> actions, IActionReferential oldReferential, IActionReferential newReferential)
        {
            foreach (KAction action in actions)
            {
                if (action.Category == oldReferential)
                {
                    action.Category = (ActionCategory)newReferential;
                    continue;
                }
                if (action.Resource == oldReferential)
                {
                    action.Resource = (Resource)newReferential;
                    continue;
                }
                if (action.Ref1.Any(c => c.Ref1 == oldReferential))
                {
                    foreach (Ref1Action actionLink in action.Ref1.Where(c => c.Ref1 == oldReferential))
                        actionLink.Ref1 = (Ref1)newReferential;
                    continue;
                }
                if (action.Ref2.Any(c => c.Ref2 == oldReferential))
                {
                    foreach (Ref2Action actionLink in action.Ref2.Where(c => c.Ref2 == oldReferential))
                        actionLink.Ref2 = (Ref2)newReferential;
                    continue;
                }
                if (action.Ref3.Any(c => c.Ref3 == oldReferential))
                {
                    foreach (Ref3Action actionLink in action.Ref3.Where(c => c.Ref3 == oldReferential))
                        actionLink.Ref3 = (Ref3)newReferential;
                    continue;
                }
                if (action.Ref4.Any(c => c.Ref4 == oldReferential))
                {
                    foreach (Ref4Action actionLink in action.Ref4.Where(c => c.Ref4 == oldReferential))
                        actionLink.Ref4 = (Ref4)newReferential;
                    continue;
                }
                if (action.Ref5.Any(c => c.Ref5 == oldReferential))
                {
                    foreach (Ref5Action actionLink in action.Ref5.Where(c => c.Ref5 == oldReferential))
                        actionLink.Ref5 = (Ref5)newReferential;
                    continue;
                }
                if (action.Ref6.Any(c => c.Ref6 == oldReferential))
                {
                    foreach (Ref6Action actionLink in action.Ref6.Where(c => c.Ref6 == oldReferential))
                        actionLink.Ref6 = (Ref6)newReferential;
                    continue;
                }
                if (action.Ref7.Any(c => c.Ref7 == oldReferential))
                {
                    foreach (Ref7Action actionLink in action.Ref7.Where(c => c.Ref7 == oldReferential))
                        actionLink.Ref7 = (Ref7)newReferential;
                    continue;
                }
            }
        }

        /// <summary>
        /// Met à jour les références vers le référentiel spécifié pour les ressources par défaut des videos.
        /// </summary>
        /// <param name="videos">Les videos/</param>
        /// <param name="oldReferential">L'ancien référentiel.</param>
        /// <param name="newReferential">Le nouveau référential à utiliser.</param>
        public static void UpdateReferentialReferences(IEnumerable<Video> videos, IActionReferential oldReferential, IActionReferential newReferential)
        {
            foreach (Video video in videos.Where(v => v.DefaultResource == ((Resource)oldReferential)))
                video.DefaultResource = (Resource)newReferential;
        }

        /// <summary>
        /// Met à jour les références vers le référentiel spécifié pour le projet.
        /// Cela inclue les référentiels sur <see cref="Project"/>, les ressources par défaut pour les vidéos et les référentiels des actions des scénarios.
        /// </summary>
        /// <param name="project">Le projet./</param>
        /// <param name="oldReferential">L'ancien référentiel.</param>
        /// <param name="newReferential">Le nouveau référential à utiliser.</param>
        public static void UpdateReferentialReferences(Project project, IActionReferential oldReferential, IActionReferential newReferential)
        {
            // Les actions
            UpdateReferentialReferences(project.Scenarios.SelectMany(s => s.Actions), oldReferential, newReferential);

            // Les resources par défaut des vidéos du projet
            if (oldReferential is Resource)
                UpdateReferentialReferences(project.Process.Videos, oldReferential, newReferential);

            // Les référentiels du projet
            if (oldReferential is ActionCategory && (oldReferential as ActionCategory).ProcessId != null)
            {
                project.Process.ActionCategories.Remove((ActionCategory)oldReferential);
                if (newReferential is ActionCategory && (newReferential as ActionCategory).ProcessId != null)
                    project.Process.ActionCategories.Add((ActionCategory)newReferential);
            }
            else if (oldReferential is Equipment && (oldReferential as Equipment).ProcessId != null)
            {
                project.Process.Equipments.Remove((Equipment)oldReferential);
                if (newReferential is Equipment && (newReferential as Equipment).ProcessId != null)
                    project.Process.Equipments.Add((Equipment)newReferential);
            }
            else if (oldReferential is Operator && (oldReferential as Operator).ProcessId != null)
            {
                project.Process.Operators.Remove((Operator)oldReferential);
                if (newReferential is Operator && (newReferential as Operator).ProcessId != null)
                    project.Process.Operators.Add((Operator)newReferential);
            }
            else if (oldReferential is Ref1)
            {
                if ((oldReferential as Ref1).ProcessId != null)
                    project.Process.Refs1.Remove((Ref1)oldReferential);
                if (newReferential is Ref1 && (newReferential as Ref1).ProcessId != null)
                    project.Process.Refs1.Add((Ref1)newReferential);
            }
            else if (oldReferential is Ref2)
            {
                if ((oldReferential as Ref2).ProcessId != null)
                    project.Process.Refs2.Remove((Ref2)oldReferential);
                if (newReferential is Ref2 && (newReferential as Ref2).ProcessId != null)
                    project.Process.Refs2.Add((Ref2)newReferential);
            }
            else if (oldReferential is Ref3)
            {
                if ((oldReferential as Ref3).ProcessId != null)
                    project.Process.Refs3.Remove((Ref3)oldReferential);
                if (newReferential is Ref3 && (newReferential as Ref3).ProcessId != null)
                    project.Process.Refs3.Add((Ref3)newReferential);
            }
            else if (oldReferential is Ref4)
            {
                if ((oldReferential as Ref4).ProcessId != null)
                    project.Process.Refs4.Remove((Ref4)oldReferential);
                if (newReferential is Ref4 && (newReferential as Ref4).ProcessId != null)
                    project.Process.Refs4.Add((Ref4)newReferential);
            }
            else if (oldReferential is Ref5)
            {
                if ((oldReferential as Ref5).ProcessId != null)
                    project.Process.Refs5.Remove((Ref5)oldReferential);
                if (newReferential is Ref5 && (newReferential as Ref5).ProcessId != null)
                    project.Process.Refs5.Add((Ref5)newReferential);
            }
            else if (oldReferential is Ref6)
            {
                if ((oldReferential as Ref6).ProcessId != null)
                    project.Process.Refs6.Remove((Ref6)oldReferential);
                if (newReferential is Ref6 && (newReferential as Ref6).ProcessId != null)
                    project.Process.Refs6.Add((Ref6)newReferential);
            }
            else if (oldReferential is Ref7)
            {
                if ((oldReferential as Ref7).ProcessId != null)
                    project.Process.Refs7.Remove((Ref7)oldReferential);
                if (newReferential is Ref7 && (newReferential as Ref7).ProcessId != null)
                    project.Process.Refs7.Add((Ref7)newReferential);
            }
        }

        static readonly Type _categoryType = typeof(ActionCategory);
        static readonly Type _operatorType = typeof(Operator);
        static readonly Type _equipmentType = typeof(Equipment);
        static readonly Type _ref1Type = typeof(Ref1);
        static readonly Type _ref2Type = typeof(Ref2);
        static readonly Type _ref3Type = typeof(Ref3);
        static readonly Type _ref4Type = typeof(Ref4);
        static readonly Type _ref5Type = typeof(Ref5);
        static readonly Type _ref6Type = typeof(Ref6);
        static readonly Type _ref7Type = typeof(Ref7);

        /// <summary>
        /// Obtient l'identifiant du référentiel en fonction de son type.
        /// </summary>
        /// <typeparam name="TReferential">Le type du référentiel.</typeparam>
        /// <param name="instance">L'instance du référentiel.</param>
        /// <returns>L'identifiant du référentiel.</returns>
        public static ProcessReferentialIdentifier GetIdentifier<TReferential>(TReferential instance)
            where TReferential : class, IActionReferential
        {
            Assertion.NotNull(instance, nameof(instance));

            if (instance is ActionCategory)
                return ProcessReferentialIdentifier.Category;
            else if (instance is Operator)
                return ProcessReferentialIdentifier.Operator;
            else if (instance is Equipment)
                return ProcessReferentialIdentifier.Equipment;
            else if (instance is Ref1)
                return ProcessReferentialIdentifier.Ref1;
            else if (instance is Ref2)
                return ProcessReferentialIdentifier.Ref2;
            else if (instance is Ref3)
                return ProcessReferentialIdentifier.Ref3;
            else if (instance is Ref4)
                return ProcessReferentialIdentifier.Ref4;
            else if (instance is Ref5)
                return ProcessReferentialIdentifier.Ref5;
            else if (instance is Ref6)
                return ProcessReferentialIdentifier.Ref6;
            else if (instance is Ref7)
                return ProcessReferentialIdentifier.Ref7;
            else
                throw new ArgumentOutOfRangeException(nameof(instance));
        }

        /// <summary>
        /// Obtient l'identifiant du référentiel en fonction de son type.
        /// </summary>
        /// <typeparam name="TReferential">Le type du référentiel.</typeparam>
        /// <returns>L'identifiant du référentiel.</returns>
        public static ProcessReferentialIdentifier GetIdentifier<TReferential>()
            where TReferential : IActionReferential =>
            GetIdentifier(typeof(TReferential));

        /// <summary>
        /// Obtient l'identifiant du référentiel en fonction de son type.
        /// </summary>
        /// <param name="referentialType">Le type du référentiel.</param>
        /// <returns>L'identifiant du référentiel.</returns>
        public static ProcessReferentialIdentifier GetIdentifier(Type referentialType)
        {
            if (referentialType.IsAssignableFrom(_categoryType))
                return ProcessReferentialIdentifier.Category;
            if (referentialType.IsAssignableFrom(_operatorType))
                return ProcessReferentialIdentifier.Operator;
            if (referentialType.IsAssignableFrom(_equipmentType))
                return ProcessReferentialIdentifier.Equipment;
            if (referentialType.IsAssignableFrom(_ref1Type))
                return ProcessReferentialIdentifier.Ref1;
            if (referentialType.IsAssignableFrom(_ref2Type))
                return ProcessReferentialIdentifier.Ref2;
            if (referentialType.IsAssignableFrom(_ref3Type))
                return ProcessReferentialIdentifier.Ref3;
            if (referentialType.IsAssignableFrom(_ref4Type))
                return ProcessReferentialIdentifier.Ref4;
            if (referentialType.IsAssignableFrom(_ref5Type))
                return ProcessReferentialIdentifier.Ref5;
            if (referentialType.IsAssignableFrom(_ref6Type))
                return ProcessReferentialIdentifier.Ref6;
            if (referentialType.IsAssignableFrom(_ref7Type))
                return ProcessReferentialIdentifier.Ref7;
            
            throw new ArgumentOutOfRangeException(nameof(referentialType));
        }
    }
}
