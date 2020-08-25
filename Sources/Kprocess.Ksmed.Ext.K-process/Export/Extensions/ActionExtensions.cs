using KProcess.Globalization;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using System;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    public static class ActionExtensions
    {
        public static string GetIES(this KAction action)
        {
            string label = null;

            // Amélioration
            if (action.Reduced != null)
            {
                // Amélioration I/E/S

                if (ActionsTimingsMoveManagement.IsActionInternal(action))
                    label = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal");
                else if (ActionsTimingsMoveManagement.IsActionExternal(action))
                    label = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_External");
                else if (ActionsTimingsMoveManagement.IsActionDeleted(action))
                    label = LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_Deleted");
                else
                    throw new ArgumentOutOfRangeException();
            }

            return label;
        }
    }
}
