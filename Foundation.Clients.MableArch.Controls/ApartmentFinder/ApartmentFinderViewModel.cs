using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using GhostCore.UWP.AutoFormGeneration;
using System;

namespace Foundation.Clients.MableArch.Controls.ApartmentFinder
{
    public class ApartmentFinderViewModel : ContentItemViewModel
    {
        [BrowseFormItem(Label = "Schema Definition")]
        public Uri SchemaDefinition
        {
            get { return ModelAs<ApartmentFinderItem>().SchemaDefinition; }
            set { ModelAs<ApartmentFinderItem>().SchemaDefinition = value; OnPropertyChanged(nameof(SchemaDefinition)); }
        }

        [BrowseFormItem(Label = "Static File Path")]
        public Uri StaticFilePath
        {
            get { return ModelAs<ApartmentFinderItem>().StaticFilePath; }
            set { ModelAs<ApartmentFinderItem>().StaticFilePath = value; OnPropertyChanged(nameof(StaticFilePath)); }
        }

        public string DataSinkName
        {
            get { return ModelAs<ApartmentFinderItem>().DataSinkName; }
            set { ModelAs<ApartmentFinderItem>().DataSinkName = value; OnPropertyChanged(nameof(DataSinkName)); }
        }

        public DataSourceType DataSourceType
        {
            get { return ModelAs<ApartmentFinderItem>().DataSourceType; }
            set { ModelAs<ApartmentFinderItem>().DataSourceType = value; OnPropertyChanged(nameof(DataSourceType)); }
        }

        public ApartmentFinderViewModel(ContentItem model)
            : base(model)
        {
        }
    }
}
