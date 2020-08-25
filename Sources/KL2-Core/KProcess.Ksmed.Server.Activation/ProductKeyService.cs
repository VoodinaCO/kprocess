using KProcess.Ksmed.Security.Activation;

namespace KProcess.Ksmed.Server.Activation
{
    public class ProductKeyService
    {
        public ProductLicenseInfo GenerateAndActivateProductKey(short productID, short productFeatures, short trialDays, string clientID, string username, string company, string userEmail, string machineHash)
        {
            string privateXmlKey = Helpers.GetResourceString("KProcess.Ksmed.Server.Activation.Resources.PrivateKey.xml");

            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);
            string productKey = keyPublisher.GenerateProductKey(productID, productFeatures, trialDays, clientID, username, company, userEmail);

            return new ProductActivation(privateXmlKey)
                .ActivateProduct(productKey, machineHash);
        }

        public ProductLicenseInfo GenerateAndActivateWebProductKey(short productID, short trialDays, short numberOfUsers, string username, string company, string userEmail, string machineHash)
        {
            string privateXmlKey = Helpers.GetResourceString("KProcess.Ksmed.Server.Activation.Resources.PrivateKey.xml");

            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);
            string productKey = keyPublisher.GenerateWebProductKey(productID, trialDays, numberOfUsers, username, company, userEmail);

            return new ProductActivation(privateXmlKey)
                .ActivateWebProduct(productKey, machineHash);
        }
    }
}
