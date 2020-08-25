using KProcess.Presentation.Windows;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran de gestion des compétences d'action.
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IAdminSkillsViewModel))]
    public partial class AdminSkillsView : UserControl, IView
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AdminActionCategoriesView"/>.
        /// </summary>
        public AdminSkillsView()
        {
            InitializeComponent();
        }
    }

}

