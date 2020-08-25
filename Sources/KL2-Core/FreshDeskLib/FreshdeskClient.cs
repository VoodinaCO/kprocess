using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FreshDeskLib
{
    public class FreshdeskClient
    {
        static FreshdeskClient _instance;
        public static FreshdeskClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FreshdeskClient();
                return _instance;
            }
        }

        HttpWebRequest request;
        bool _isInitialized;

        public string API_USERNAME { get; set; }
        public string API_KEY { get; set; }
        public string API_FRESHDESK_TICKETS { get; set; }
        public string API_FRESHDESK_OUTBOUND_EMAIL { get; set; }

        public void Initialize(string api_username, string api_key, string api_tickets, string api_outbound_email)
        {
            API_USERNAME = api_username;
            API_KEY = api_key;
            API_FRESHDESK_TICKETS = api_tickets;
            API_FRESHDESK_OUTBOUND_EMAIL = api_outbound_email;

            _isInitialized = true;
        }

        string PrepareAuthInfo() =>
            $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{API_USERNAME}:{API_KEY}"))}";

        async Task<bool> PostAsync(string apiPath, SerializableObject obj)
        {
            var formData = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToJson())
                .ToDictionary(_ => _.Key, _ => _.Value.ToString().Replace("\r\n", ""));

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            var boundary = $"----------------------------{DateTime.Now.Ticks.ToString("x")}";
            request = (HttpWebRequest)WebRequest.Create(apiPath);
            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.Method = "POST";
            request.KeepAlive = true;
            request.Headers["Authorization"] = PrepareAuthInfo();

            //Get the stream that holds request data by calling the GetRequestStream method. 
            using (Stream dataStream = await request.GetRequestStreamAsync())
            {
                //FormData
                foreach (var pair in formData)
                {
                    await WriteBoundaryBytesAsync(dataStream, boundary, false);
                    if (pair.Key == "cc_emails")
                    {
                        var cc_emails = JsonConvert.DeserializeObject<string[]>(pair.Value);
                        foreach(string _ in cc_emails)
                        {
                            await WriteContentDispositionFormDataHeaderAsync(dataStream, $"{pair.Key}[]");
                            await WriteStringAsync(dataStream, _);
                            await WriteCrlfAsync(dataStream);
                        }
                        continue;
                    }
                    await WriteContentDispositionFormDataHeaderAsync(dataStream, pair.Key);

                    await WriteStringAsync(dataStream, pair.Value);
                    await WriteCrlfAsync(dataStream);
                }
                //Now we can send Attachments
                if (obj is IAttachments attachments && attachments.Attachments != null)
                {
                    foreach (var attachment in attachments.Attachments)
                    {
                        await WriteBoundaryBytesAsync(dataStream, boundary, false);

                        await WriteContentDispositionFileHeaderAsync(dataStream, "attachments[]",
                            attachment.Key, "application/octet-stream");
                        await dataStream.WriteAsync(attachment.Value, 0, attachment.Value.Length);
                        await WriteCrlfAsync(dataStream);
                    }
                }

                await WriteBoundaryBytesAsync(dataStream, boundary, true);
            }
            var sb = new StringBuilder();
            try
            {
                sb.AppendLine("Submitting Request");
                WebResponse response = await request.GetResponseAsync();
                // Get the stream containing content returned by the server.
                //Send the request to the server by calling GetResponse. 
                var dataStream = response.GetResponseStream();
                if (dataStream == null)
                    return false;
                // Open the stream using a StreamReader for easy access. 
                StreamReader reader = new StreamReader(dataStream);
                // Read the content. 
                string Response = await reader.ReadToEndAsync();
                //return status code
                sb.AppendLine($"Status Code: {(int)((HttpWebResponse)response).StatusCode} {((HttpWebResponse)response).StatusCode}");
                //return location header
                sb.AppendLine($"Location: {response.Headers["Location"]}");

                reader.Dispose();
                dataStream.Dispose();

                //return the response 
                await Console.Out.WriteLineAsync(Response);
            }
            catch (WebException ex)
            {
                sb.AppendLine("API Error: Your request is not successful. If you are not able to debug this error properly, mail us at support@freshdesk.com with the following X-Request-Id");
                sb.AppendLine($"X-Request-Id: {ex.Response.Headers["X-Request-Id"]}");
                sb.AppendLine($"Error Status Code : {(int)((HttpWebResponse)ex.Response).StatusCode} {((HttpWebResponse)ex.Response).StatusCode}");
                var stream = ex.Response.GetResponseStream();
                if (stream == null)
                    return false;
                var reader = new StreamReader(stream);
                sb.AppendLine("Error Response: ");
                sb.AppendLine(await reader.ReadToEndAsync());

                reader.Dispose();
                stream.Dispose();

                throw new Exception(sb.ToString());
            }
            catch (Exception ex)
            {
                sb.AppendLine("ERROR");
                sb.AppendLine(ex.Message);
                throw new Exception(sb.ToString());
            }

            return true;
        }

        static Task WriteCrlfAsync(Stream requestStream)
        {
            byte[] crLf = Encoding.UTF8.GetBytes("\r\n");
            return requestStream.WriteAsync(crLf, 0, crLf.Length);
        }

        static Task WriteBoundaryBytesAsync(Stream requestStream, string b, bool isFinalBoundary)
        {
            string boundary = isFinalBoundary ? $"--{b}--" : $"--{b}\r\n";
            byte[] d = Encoding.UTF8.GetBytes(boundary);
            return requestStream.WriteAsync(d, 0, d.Length);
        }

        static Task WriteContentDispositionFormDataHeaderAsync(Stream requestStream, string name)
        {
            string data = $"Content-Disposition: form-data; name=\"{name}\"\r\n\r\n";
            byte[] b = Encoding.UTF8.GetBytes(data);
            return requestStream.WriteAsync(b, 0, b.Length);
        }

        static Task WriteContentDispositionFileHeaderAsync(Stream requestStream, string name, string fileName, string contentType)
        {
            string data = $"Content-Disposition: form-data; name=\"{name}\"; filename=\"{fileName}\"\r\n";
            data += $"Content-Type: {contentType}\r\n\r\n";
            byte[] b = Encoding.UTF8.GetBytes(data);
            return requestStream.WriteAsync(b, 0, b.Length);
        }

        static Task WriteStringAsync(Stream requestStream, string data)
        {
            byte[] b = Encoding.UTF8.GetBytes(data);
            return requestStream.WriteAsync(b, 0, b.Length);
        }

        /// <summary>
        /// Envoie un ticket.
        /// </summary>
        /// <param name="ticket">Le ticket à envoyer.</param>
        /// <returns>
        ///   <c>true</c> si l'envoi a réussi.
        /// </returns>
        public Task<bool> SendTicketAsync(Ticket ticket)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Le client FreshDesk n'a pas été initialisé.");
            if (string.IsNullOrEmpty(API_KEY))
                throw new InvalidOperationException("API_KEY ne peut être null.");
            if (string.IsNullOrEmpty(API_FRESHDESK_TICKETS))
                throw new InvalidOperationException("API_FRESHDESK_TICKETS ne peut être null.");

            if (string.IsNullOrEmpty(ticket.Email))
                throw new InvalidOperationException("Le ticket doir avoir un expéditeur.");
            if (string.IsNullOrEmpty(ticket.Subject))
                throw new InvalidOperationException("Le ticket doir avoir un expéditeur.");

            return PostAsync(API_FRESHDESK_TICKETS, ticket);
        }
        public Task<bool> SendOutboundMailAsync(OutboundMail mail)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Le client FreshDesk n'a pas été initialisé.");
            if (string.IsNullOrEmpty(API_KEY))
                throw new InvalidOperationException("API_KEY ne peut être null.");
            if (string.IsNullOrEmpty(API_FRESHDESK_OUTBOUND_EMAIL))
                throw new InvalidOperationException("API_FRESHDESK_OUTBOUND_EMAIL ne peut être null.");

            if (string.IsNullOrEmpty(mail.Email))
                throw new InvalidOperationException("Le ticket doir avoir un expéditeur.");
            if (string.IsNullOrEmpty(mail.Subject))
                throw new InvalidOperationException("Le ticket doir avoir un expéditeur.");

            return PostAsync(API_FRESHDESK_OUTBOUND_EMAIL, mail);
        }
    }
}
