using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Server.Activation.KeygenApp.Freshdesk.Models
{

    public class Company : SerializableObject
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("domains")]
        public List<string> Domains { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        [JsonProperty("custom_fields")]
        public CustomFields Other { get; set; }


        public string GetNextLicenceKeyIdentifier()
        {
            string returnLicence = string.Empty;
            long nextLicenceNumber = 1;

            if (this.Other.LicenceNumber.HasValue && this.Other.LicenceNumber >= 1)
            {
                nextLicenceNumber = this.Other.LicenceNumber.Value;
                nextLicenceNumber++;
            }
            this.Other.LicenceNumber = nextLicenceNumber;

            switch (this.Other.LicenceId?.Length)
            {
                case 2:
                    returnLicence = this.Other.LicenceId + nextLicenceNumber.ToString("0000");
                    break;
                case 3:
                    returnLicence = this.Other.LicenceId + nextLicenceNumber.ToString("000");
                    break;
                case 4:
                    returnLicence = this.Other.LicenceId + nextLicenceNumber.ToString("00");
                    break;
            }

            return returnLicence;
        }



        public class CustomFields
        {
            [JsonProperty("identifiant_licence_socit")]
            public string LicenceId { get; set; }
            [JsonProperty("nombre_de_licences")]
            public long? LicenceNumber { get; set; }
            [JsonConverter(typeof(CustomDateTimeConverter))]
            [JsonProperty("date_anniversaire_maintenance")]
            public DateTime? MaintenanceAnniversaryDate { get; set; }
            [JsonProperty("version")]
            public string VersionKL2 { get; set; }
        }
    }


}
