using System.IO;
using System.Web;

namespace KProcess.KL2.WebAdmin
{
    /// <summary>
    /// Summary description for UploadFile
    /// </summary>
    public class UploadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            /*if (false)
            {
                var file = context.Request.Files[0];
                string targetFolder = HttpContext.Current.Server.MapPath("Files");
                //creating a folder for saving a file
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    fileName = HttpContext.Current.Request.Form.GetValues("intRefIdentifier")[0] + "." + HttpContext.Current.Request.Form.GetValues("itemId")[0] + "." + fileName.Split('.')[1];
                    file.SaveAs(Path.Combine(targetFolder, fileName));
                }

                context.Response.Write("Success");
            }*/
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}