using Foundation.Core.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Clients.MableArch.Controls.ApartmentFinder
{
    public class ApartmentFinderItem : ContentItem
    {
        public Uri SchemaDefinition { get; set; }
        public Uri StaticFilePath { get; set; }
        public string DataSinkName { get; set; }
        public DataSourceType DataSourceType { get; set; }
    }

    public enum DataSourceType
    {
        StaticFile,
        DataSink
    }
}