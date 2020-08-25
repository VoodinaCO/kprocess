using KProcess.Presentation.Windows.Controls;
using System;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Shell.Configuration.DirectShow
{
    /// <summary>
    /// Parser de configuration DirectShow
    /// </summary>
    internal static class ConfigurationParser
    {

        private const string CLSID_WMV_SPLITTER = "1932C124-77DA-4151-99AA-234FEA09F463";
        private const string CLSID_DMO_DECODER = "82D353DF-90BD-4382-8BC2-3F6192B76E34";
        private const string CLSID_LAV_VIDEO_DECODER = "EE30215D-164F-4A92-A4EB-9D4C13390F9F";

        /// <summary>
        /// Parse la configuration DirectShow
        /// </summary>
        public static FiltersConfiguration Parse()
        {
            try
            {
                FiltersConfigurationSection configSection = FiltersConfigurationSection.GetConfiguration();

                FiltersConfiguration conf = new FiltersConfiguration();

                foreach (ExtensionElement extension in configSection.Extensions)
                {
                    var ext = new ExtensionFiltersSource()
                    {
                        Extension = extension.Extension,
                        MinSpeedRatio = extension.MinSpeedRatio,
                        MaxSpeedRatio = extension.MaxSpeedRatio,
                        Splitter = new FilterSource
                        {
                            Name = extension.Splitter.Name,
                            SourceType = extension.Splitter.SourceType,
                            ExternalCLSID = extension.Splitter.ExternalCLSID,
                        },
                        VideoDecoder = new FilterSource
                        {
                            Name = extension.VideoDecoder.Name,
                            SourceType = extension.VideoDecoder.SourceType,
                            ExternalCLSID = extension.VideoDecoder.ExternalCLSID,
                        },
                        AudioDecoder = new FilterSource
                        {
                            Name = extension.AudioDecoder.Name,
                            SourceType = extension.AudioDecoder.SourceType,
                            ExternalCLSID = extension.AudioDecoder.ExternalCLSID,
                        }
                    };

                    conf[ext.Extension] = ext;
                }

                // Filtres par défaut, minimum requis
                if (conf[".wmv"] == null)
                {
                    conf[".wmv"] = new ExtensionFiltersSource
                    {
                        Extension = ".wmv",
                        Splitter = new FilterSource
                        {
                            Name = "GDCL WMV Splitter",
                            SourceType = FilterSourceTypeEnum.External,
                            ExternalCLSID = CLSID_WMV_SPLITTER,
                        },
                        VideoDecoder = new FilterSource
                        {
                            Name = "LAV Video Decoder",
                            SourceType = FilterSourceTypeEnum.External,
                            ExternalCLSID = CLSID_LAV_VIDEO_DECODER,
                        },
                        AudioDecoder = new FilterSource
                        {
                            Name = "Auto Audio Decoder",
                            SourceType = FilterSourceTypeEnum.Auto,
                        }
                    };
                }

                // Vérification de l'exactitude des valeurs
                if (conf.Select(e => e.Extension).GroupBy(e => e).Any(g => g.Count() > 2))
                {
                    throw new ConfigurationErrorsException("Les extensions configurées doivent être uniques.");
                }

                foreach (var e in conf)
                {
                    if (e.Splitter.SourceType == FilterSourceTypeEnum.Auto)
                        throw new ConfigurationErrorsException("Le splitter ne peut pas être Auto");

                    if ((e.MinSpeedRatio.HasValue && !e.MaxSpeedRatio.HasValue) ||
                        (!e.MinSpeedRatio.HasValue && e.MaxSpeedRatio.HasValue))
                        throw new ConfigurationErrorsException("Les vitesses de lecture doivent être toutes les deux définies");

                    if (e.MinSpeedRatio.HasValue && e.MaxSpeedRatio.HasValue && e.MinSpeedRatio.Value > e.MaxSpeedRatio.Value)
                        throw new ConfigurationErrorsException("Les vitesses de lecture sont incorrectes");

                    var sources = new FilterSource[]
                    {
                        e.Splitter, 
                        e.AudioDecoder,
                        e.VideoDecoder
                    };

                    foreach (var source in sources)
                    {
                        if (source.SourceType == FilterSourceTypeEnum.External && string.IsNullOrEmpty(source.ExternalCLSID))
                            throw new ConfigurationErrorsException("Une source externe doit être accompagnée d'un CLSID externe");

                        if (source.SourceType == FilterSourceTypeEnum.Auto && !string.IsNullOrEmpty(source.ExternalCLSID))
                            throw new ConfigurationErrorsException("Une source auto ne doit pas être accompagnée d'un CLSID externe");
                    }
                }

                return conf;
            }
            catch (ConfigurationErrorsException ex)
            {
                TraceManager.TraceError(ex, "Erreur de configuration");
                MessageBox.Show(Resources.LocalizationResources.Error_Configuration_Filters, Resources.LocalizationResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            catch (Exception e)
            {
                TraceManager.TraceError(e, "Erreur lors de l'initialisation de la configuration des filtres");
                MessageBox.Show(Resources.LocalizationResources.Exception_GenericMessage, Resources.LocalizationResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
