using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;
using KProcess.Ksmed.Presentation.Core.Behaviors;

namespace KProcess.Ksmed.Presentation.ViewModels.Restitution
{
    public class RestitutionValueModeViewModel
    {
        internal static IEnumerable<RestitutionValueModeViewModel> GetViewModels()
        {
            yield return new RestitutionValueModeViewModel(RestitutionValueMode.Absolute);
            yield return new RestitutionValueModeViewModel(RestitutionValueMode.Relative);
            yield return new RestitutionValueModeViewModel(RestitutionValueMode.Occurences);
        }

        public RestitutionValueModeViewModel(RestitutionValueMode mode)
        {
            this.Value = mode;

            string key;

            switch (mode)
            {
                case RestitutionValueMode.Absolute:
                    key = "View_Restitution_AbsoluteValues";
                    break;

                case RestitutionValueMode.Relative:
                    key = "View_Restitution_RelativeValues";
                    break;

                case RestitutionValueMode.Occurences:
                    key = "View_Restitution_OccurenceValues";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("mode");
            }

            this.Label = LocalizationManagerExt.GetSafeDesignerString(key);
        }

        public RestitutionValueMode Value { get; private set; }

        public string Label { get; private set; }
    }
}
