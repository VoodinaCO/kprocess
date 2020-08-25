using KProcess.Ksmed.Server.Activation.KeygenApp.Freshdesk.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Server.Activation.KeygenApp
{
    class JsonDeserializer
    {
        public List<User> GetUserListFromJson(string json)
        {
            var userList = new List<User>();
            if (json.IsNotNullNorEmpty())
            {
                JArray jarray = JArray.Parse(json);
                var jsonUserList = jarray.ToList();


                foreach (JToken jsonUser in jsonUserList)
                {
                    User user = JsonConvert.DeserializeObject<User>(jsonUser.ToString());
                    userList.Add(user);
                }
            }

            return userList;
        }

        public List<Company> GetCompanyListFromJson(string json)
        {
            var companyList = new List<Company>();
            if (json.IsNotNullNorEmpty())
            {
                JArray jarray = JArray.Parse(json);
                var jsonCompanyList = jarray.ToList();


                foreach (JToken jsonCompany in jsonCompanyList)
                {
                    Company company = JsonConvert.DeserializeObject<Company>(jsonCompany.ToString());
                    companyList.Add(company);
                }
            }

            return companyList;
        }

        public Company GetCompanyFromJson(string json)
        {
            var company = new Company();
            if (json.IsNotNullNorEmpty())
            {
                //JToken jsonCompany = JToken.Parse(json);
                company = JsonConvert.DeserializeObject<Company>(json);
            }

            return company;
        }
    }
}
