using KProcess.Ksmed.Server.Activation.KeygenApp.Freshdesk.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Server.Activation.KeygenApp
{
    class FreshdeskApiConnector
    {
        private HttpWebRequest request = null;
        private const string API_KEY = "kuOJdimBk4Y93MC15gT:X";
        private const string API_FRESHDESK_COMPANIES = "https://kprocess.freshdesk.com/api/v2/companies";
        private const string API_FRESHDESK_CUSTOMERS = "https://kprocess.freshdesk.com/api/v2/contacts";
        private const string API_FRESHDESK_OUTBOUND_EMAIL = "https://kprocess.freshdesk.com/api/v2/tickets/outbound_email";

        public async Task<List<User>> GetCustomerListAsync()
        {
            JsonDeserializer deserializer = new JsonDeserializer();
            int page = 1;
            var list = new List<User>();
            string responseBody = string.Empty;
            do
            {
                responseBody = await this.GetResponseAsync(API_FRESHDESK_CUSTOMERS, $"?per_page=99&page={page}");

                list.AddRange(deserializer.GetUserListFromJson(responseBody));
                page++;
            } while (responseBody!="[]");

            return list;
        }

        public async Task<List<Company>> GetCompanyListAsync()
        {
            JsonDeserializer deserializer = new JsonDeserializer();
            int page = 1;
            var list = new List<Company>();
            string responseBody = string.Empty;
            do
            {
                responseBody = await this.GetResponseAsync(API_FRESHDESK_COMPANIES, $"?per_page=99&page={page}");

                list.AddRange(deserializer.GetCompanyListFromJson(responseBody));
                page++;
            } while (responseBody != "[]");

            return list;
        }

        public async Task<Company> GetCompanyByIdAsync(string companyId)
        {
            var responseBody = await this.GetResponseAsync(API_FRESHDESK_COMPANIES + "/" + companyId);

            JsonDeserializer deserializer = new JsonDeserializer();
            return deserializer.GetCompanyFromJson(responseBody);
        }

        public bool CreateOutboundMail(OutboundMail mail)
        {
            return this.Post(API_FRESHDESK_OUTBOUND_EMAIL, mail.ToJson());
        }

        public bool UpdateCompany(Company company)
        {
            return this.Put(API_FRESHDESK_COMPANIES + "/" + company.Id, company.ToJson());             
        }

        public bool UpdateUser(User user)
        {
            return this.Put(API_FRESHDESK_CUSTOMERS + "/" + user.Id, user.ToJson());
        }

        private async Task<string> GetResponseAsync(string apiPath, string filter="")
        {
            string responseBody = String.Empty;

            this.request = (HttpWebRequest)WebRequest.Create(apiPath + filter);
            this.request.Method = "GET";
            this.request.ContentType = "application/json";
            this.request.Headers["Authorization"] = PrepareAuthInfo();

            try
            {
                Console.WriteLine("Submitting Request");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseBody = await reader.ReadToEndAsync();
                    reader.Close();
                    dataStream.Close();
                }


            }
            catch (WebException ex)
            {
                Console.WriteLine("API Error: Your request is not successful. If you are not able to debug this error properly, mail us at support@freshdesk.com with the follwing X-Request-Id");
                Console.WriteLine("X-Request-Id: {0}", ex.Response.Headers["X-Request-Id"]);
                Console.WriteLine("Error Status Code : {1} {0}", ((HttpWebResponse)ex.Response).StatusCode, (int)((HttpWebResponse)ex.Response).StatusCode);
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.Write("Error Response: ");
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(ex.Message);
            }

            return responseBody;
        }

        private bool Put(string apiPath, string json)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            this.request = (HttpWebRequest)WebRequest.Create(apiPath);
            this.request.ContentType = "application/json";
            this.request.Method = "PUT";            
            this.request.ContentLength = byteArray.Length;
            this.request.Headers["Authorization"] = PrepareAuthInfo();

            //Get the stream that holds request data by calling the GetRequestStream method. 
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream. 
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object. 
            dataStream.Close();
            try
            {
                Console.WriteLine("Submitting Request");
                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string Response = reader.ReadToEnd();
                Console.WriteLine("Status Code: {1} {0}", ((HttpWebResponse)response).StatusCode, (int)((HttpWebResponse)response).StatusCode);
                Console.Out.WriteLine(Response);
            }
            catch (WebException ex)
            {
                Console.WriteLine("API Error: Your request is not successful. If you are not able to debug this error properly, mail us at support@freshdesk.com with the follwing X-Request-Id");
                Console.WriteLine("X-Request-Id: {0}", ex.Response.Headers["X-Request-Id"]);
                Console.WriteLine("Error Status Code : {1} {0}", ((HttpWebResponse)ex.Response).StatusCode, (int)((HttpWebResponse)ex.Response).StatusCode);
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.Write("Error Response: ");
                    Console.WriteLine(reader.ReadToEnd());
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;

        }

        private bool Post(string apiPath, string json)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            this.request = (HttpWebRequest)WebRequest.Create(apiPath);
            this.request.ContentType = "application/json; multipart/form-data";
            this.request.Method = "POST";
            this.request.ContentLength = byteArray.Length;
            this.request.Headers["Authorization"] = PrepareAuthInfo();

            //Get the stream that holds request data by calling the GetRequestStream method. 
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream. 
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object. 
            dataStream.Close();
            try
            {
                Console.WriteLine("Submitting Request");
                WebResponse response = request.GetResponse();
                // Get the stream containing content returned by the server.
                //Send the request to the server by calling GetResponse. 
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access. 
                StreamReader reader = new StreamReader(dataStream);
                // Read the content. 
                string Response = reader.ReadToEnd();
                //return status code
                Console.WriteLine("Status Code: {1} {0}", ((HttpWebResponse)response).StatusCode, (int)((HttpWebResponse)response).StatusCode);
                //return location header
                Console.WriteLine("Location: {0}", response.Headers["Location"]);
                //return the response 
                Console.Out.WriteLine(Response);
            }
            catch (WebException ex)
            {
                Console.WriteLine("API Error: Your request is not successful. If you are not able to debug this error properly, mail us at support@freshdesk.com with the follwing X-Request-Id");
                Console.WriteLine("X-Request-Id: {0}", ex.Response.Headers["X-Request-Id"]);
                Console.WriteLine("Error Status Code : {1} {0}", ((HttpWebResponse)ex.Response).StatusCode, (int)((HttpWebResponse)ex.Response).StatusCode);
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.Write("Error Response: ");
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(ex.Message);
            }

            return true;

        }

        private string PrepareAuthInfo()
        {
            var apiKey64 = Convert.ToBase64String(Encoding.Default.GetBytes(API_KEY));
            return "Basic " + apiKey64;
        }
    }
}

