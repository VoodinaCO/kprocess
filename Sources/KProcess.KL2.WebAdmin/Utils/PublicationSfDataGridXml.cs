using KProcess.KL2.WebAdmin.Models.DTO;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace KProcess.KL2.WebAdmin.Utils
{
    public class PublicationSfDataGridXml
    {
        /// <summary>
        /// Each publication has a list of columns that needs to displayed to enduser
        /// Retrieve this list from XML stored in Db
        /// </summary>
        /// <param name="publication"></param>
        /// <returns></returns>
        public static IEnumerable<SfDataGridGridColumn> GetVisibleActionColumns(Publication publication, PublishModeEnum publishModeFilter)
        {
            SfDataGrid settings;
            XmlSerializer serializer = new XmlSerializer(typeof(SfDataGrid));

            byte[] disposition = null;
            if (publishModeFilter == PublishModeEnum.Formation)
                disposition = publication.Formation_Disposition;

            if (publishModeFilter == PublishModeEnum.Inspection)
                disposition = publication.Inspection_Disposition;
            
            if (publishModeFilter == PublishModeEnum.Audit)
                disposition = publication.Audit_Disposition;

            if (disposition == null)
                return new List<SfDataGridGridColumn>();

            using (MemoryStream ms = new MemoryStream(disposition))
            {
                var xml = Encoding.UTF8.GetString(ms.ToArray());
                xml = xml.Replace("i:type=\"GridTextColumn\"", "").Replace("i:type=\"GridNumericColumn\"", "")
                    .Replace("i:type=\"GridTemplateColumn\"", "").Replace("i:type=\"GridCheckBoxColumn\"", "");

                using (TextReader reader = new StringReader(xml))
                {
                    settings = (SfDataGrid)serializer.Deserialize(reader);
                }
            }
            return settings.Columns.Where(u => !u.IsHidden);
        }


    }
}