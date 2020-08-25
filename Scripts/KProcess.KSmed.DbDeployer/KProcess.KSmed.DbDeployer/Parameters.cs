using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.KSmed.DbDeployer
{
    public class Parameters
    {
        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string DatabaseName { get; set; }
        /// <summary>
        /// Gets or sets the file path where .mdf files must be stored.
        /// </summary>
        public string DataPath { get; set; }
        /// <summary>
        /// Gets or sets the file path where database log must be stored.
        /// </summary>
        public string LogPath { get; set; }
        /// <summary>
        /// Gets or sets the customs parameters.
        /// </summary>
        public Dictionary<string, string> CustomsParameters { get; set; }
        /// <summary>
        /// Gets or sets the SQL file path.
        /// </summary>
        public string SqlFilePath { get; set; }

        public string ServerName { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        public Parameters()
        {
            this.CustomsParameters = new Dictionary<string, string>();
        }
    }
}
