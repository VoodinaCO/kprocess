using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Server.Activation.KeygenApp.Freshdesk.Models
{
    public class User : SerializableObject
    {

        [JsonProperty("custom_fields")]
        public CustomFields Other { get; set; }
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("company_id")]
        public long? CompanyId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        //[JsonProperty("language")]
        //public string Language { get; set; }
        //public string active { get; set; }
        //public string address { get; set; }
        //public string description { get; set; }
        [JsonProperty("email")]
        public string EmailAddress { get; set; }
        //public string job_title { get; set; }
        //public string mobile { get; set; }
        //public string phone { get; set; }
        //public string time_zone { get; set; }
        //public string twitter_id { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj != null
                && obj is User
                && ((User)obj).Id == this.Id)
            {
                return true;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }



        public class CustomFields
        {
            [JsonProperty("dure_licence")]
            public long? LicenceDuration { get; set; }
            [JsonProperty("identifiant_client")]
            public string CustomerId { get; set; }
            //public string site { get; set; }
            [JsonProperty("langue")]
            public string Language { get; set; }
            //public string ok_teamviewer { get; set; }
            // public string webex { get; set; }
            //public string lien_vers_enduser { get; set; }
            [JsonProperty("version_logiciel")]
            public string VersionKL2_Old { get; set; }
            [JsonProperty("version_kl2")]
            public string VersionKL2 { get; set; }
            [JsonConverter(typeof(CustomDateTimeConverter))]
            [JsonProperty("date_premire_installation")]
            public DateTime? FirstInstallDate { get; set; }
            [JsonConverter(typeof(CustomDateTimeConverter))]
            [JsonProperty("date_renouvellement_licence")]
            public DateTime? LicenceDateRenewal { get; set; }
            [JsonProperty("nom_cl_licence")]
            public string LicenceKeyName { get; set; }
            [JsonProperty("socit_cl_licence")]
            public string LicenceKeyCompany { get; set; }
            [JsonProperty("mail_cl_licence")]
            public string LicenceKeyMail { get; set; }
            [JsonProperty("id_machine_cl_licence")]
            public string LicenceKeyMachineId { get; set; }
            [JsonProperty("id_machine_cl_licence_bis")]
            public string LicenceKeyMachineIdBis { get; set; }
            [JsonProperty("lien_vers_enduser")]
            public string EndUserLink { get; set; }
            [JsonProperty("updated")]
            public bool? Updated { get; set; }
        }

    }


}
