using KProcess.Globalization;

namespace KProcess.Ksmed.Ext.Kprocess
{
    class ResourceExport : IResourceProviderExport
    {
        public const string Id = "SmartExport";

        private ILocalizedResourceProvider _provider;

        public ILocalizedResourceProvider GetProvider()
        {
            if (_provider == null)
                _provider = new ResourceFileProvider(Id, typeof(Resources.LocalizationResources));
            return _provider;
        }
    }
}
