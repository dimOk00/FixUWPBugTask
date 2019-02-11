using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Data
{
    public class DataSchema
    {
        public List<DataSchemaItem> DataItems { get; set; }

        public DataSchema()
        {
            DataItems = new List<DataSchemaItem>();
        }
    }

    public class DataSchemaItem
    {
        public bool IsIdentifier { get; set; }
        public string Property { get; set; }
        public string PropertyLabel { get; set; }
        public bool IsMultiSelect { get; set; }
        public int ArrayIndex { get; set; } = -1;
        public DataType DataType { get; set; }
        public List<DataSourceItem> DataSource { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }

    public class DataSourceItem
    {
        public string Label { get; set; }
        public object Value { get; set; }
    }

    public enum DataType
    {
        FromSource,
        Range,
        Boolean,
    }
}
