using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KProcess.Ksmed.Models
{
    partial class Anomaly
    {
        public bool IsHorsVisite { get; set; }
        public string ActionLabel { get; set; }
        public string AnomalyTypeAndName { get; set; }
    }

    public static class Anomalies
    {
        public static readonly string other = "Autre";

        public static List<IAnomalyKindItem> GetPossibleAnomalies(AnomalyType anomalyType)
        {
            var result = new List<IAnomalyKindItem>();
            int i = 1;
            switch (anomalyType)
            {
                case AnomalyType.Security:
                    result.Add(new AnomalyKindItemTitle(string.Empty));
                    result.Add(new AnomalyKindItem(i++, "Outils inadéquats"));
                    result.Add(new AnomalyKindItem(i++, "Dispositif de sécurité brisé"));
                    result.Add(new AnomalyKindItem(i++, "Dispositif de sécurité exclus"));
                    result.Add(new AnomalyKindItem(i++, "Dispositif de sécurité enlevé"));
                    result.Add(new AnomalyKindItemTitle(string.Empty));
                    result.Add(new AnomalyKindItem(i++, "Pièce électrique brisée"));
                    result.Add(new AnomalyKindItem(i++, "Pièce électrique non protégée"));
                    result.Add(new AnomalyKindItem(i++, "Pièce mobile non protégée"));
                    result.Add(new AnomalyKindItem(i++, "Espace restreint"));
                    result.Add(new AnomalyKindItemTitle(string.Empty));
                    result.Add(new AnomalyKindItem(i++, "EPI non disponible"));
                    result.Add(new AnomalyKindItem(i++, "EPI en mauvaise condition"));
                    result.Add(new AnomalyKindItem(i++, "Procédures non disponibles"));
                    result.Add(new AnomalyKindItem(i++, "Procédures incorrectes"));
                    result.Add(new AnomalyKindItemTitle(string.Empty));
                    result.Add(new AnomalyKindItem(i++, "Fuite matière dangereuse"));
                    result.Add(new AnomalyKindItem(i++, "Formation inadéquate"));
                    result.Add(new AnomalyKindItem(i++, "Comportement non sécuritaire"));
                    result.Add(new AnomalyKindItem(i++, other));
                    break;
                case AnomalyType.Maintenance:
                case AnomalyType.Operator:
                    var title1 = new AnomalyKindItemTitle("Anomalie");
                    var title2 = new AnomalyKindItemTitle("Contamination");
                    var title3 = new AnomalyKindItemTitle("Difficulté d'accès pour");
                    result.Add(title1);
                    result.Add(new AnomalyKindItem(i++, "Fuite", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Usé", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Brisé - Déformé", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Jeu dans la pièce", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Manquant", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Paramètre réglage non OK", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Bloqué", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Vibration excessive", title1.Label));
                    result.Add(new AnomalyKindItemEmpty());
                    result.Add(new AnomalyKindItem(i++, "Bruit excessif", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Température excessive", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Mauvais fonctionnement", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Equipement redondant", title1.Label));
                    result.Add(new AnomalyKindItem(i++, "Aucun standard", title1.Label));
                    result.Add(new AnomalyKindItem(i++, other, title1.Label));
                    result.Add(new AnomalyKindItemEmpty());
                    result.Add(new AnomalyKindItemEmpty());
                    result.Add(title2);
                    result.Add(new AnomalyKindItem(i++, "Lubrification", title2.Label));
                    result.Add(new AnomalyKindItem(i++, "Eau/Liquide", title2.Label));
                    result.Add(new AnomalyKindItem(i++, "Produit", title2.Label));
                    result.Add(new AnomalyKindItem(i++, "Rejet de production", title2.Label));
                    result.Add(new AnomalyKindItem(i++, "Gaz", title2.Label));
                    result.Add(new AnomalyKindItem(i++, "Saleté", title2.Label));
                    result.Add(new AnomalyKindItem(i++, "Corrosion", title2.Label));
                    result.Add(new AnomalyKindItem(i++, other, title2.Label));
                    result.Add(title3);
                    result.Add(new AnomalyKindItem(i++, "Nettoyage", title3.Label));
                    result.Add(new AnomalyKindItem(i++, "Inspection", title3.Label));
                    result.Add(new AnomalyKindItem(i++, "Lubrification", title3.Label));
                    result.Add(new AnomalyKindItem(i++, "Remplacement", title3.Label));
                    result.Add(new AnomalyKindItem(i++, "Serrage", title3.Label));
                    result.Add(new AnomalyKindItem(i++, other, title3.Label));
                    break;
            }
            return result;
        }

        public static List<AnomalyType> GetAnomalyTypes()
        {
            return new List<AnomalyType>
            {
                AnomalyType.Security,
                AnomalyType.Maintenance,
                AnomalyType.Operator
            };
        }

        public static string AnomalyTypeToString(this AnomalyType anomalyType)
        {
            switch (anomalyType)
            {
                case AnomalyType.Security:
                    return "Sécurité";
                case AnomalyType.Maintenance:
                    return "Maintenance";
                case AnomalyType.Operator:
                    return "Opérateur";
                default:
                    return string.Empty;
            }
        }

        public static string AnomalyTypeToStringReport(this AnomalyType anomalyType)
        {
            switch (anomalyType)
            {
                case AnomalyType.Security:
                    return "Vert";
                case AnomalyType.Maintenance:
                    return "Rouge";
                case AnomalyType.Operator:
                    return "Bleue";
                default:
                    return string.Empty;
            }
        }
    }

    public interface IAnomalyKindItem
    {
        string Label { get; set; }
        string Category { get; set; }
    }
    [Serializable]
    public class AnomalyKindItem : IAnomalyKindItem
    {
        public int Number { get; set; }
        public string Label { get; set; }
        public string Category { get; set; }

        public AnomalyKindItem() : this(0, null, null)
        {
        }
        public AnomalyKindItem(int number, string label, string category = null)
        {
            Number = number;
            Label = label;
            Category = category;
        }
    }

    public class AnomalyKindEditableItem : IAnomalyKindItem, INotifyPropertyChanged
    {
        public int Number { get; set; }
        public string Label { get; set; }
        public string Category { get; set; }

        string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public AnomalyKindEditableItem() : this(0, null, null)
        {
        }
        public AnomalyKindEditableItem(int number, string label, string category = null)
        {
            Number = number;
            Label = label;
            Category = category;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    [Serializable]
    public class AnomalyKindItemTitle : IAnomalyKindItem
    {
        public string Label { get; set; }
        public string Category { get; set; }

        public AnomalyKindItemTitle(string label)
        {
            Label = label;
        }
    }
    [Serializable]
    public class AnomalyKindItemEmpty : IAnomalyKindItem
    {
        public string Label { get; set; }
        public string Category { get; set; }
    }

}
