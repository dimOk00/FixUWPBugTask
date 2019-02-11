using Foundation.Core.Data;
using GhostCore.UWP.AutoFormGeneration;
using System;
using System.Collections.Generic;

namespace Foundation.Core.ViewModels
{
    public class PDFContentItemViewModel : ContentItemViewModel
    {
        [BrowseFormItem(Label = "PDF Path")]
        public Uri Path
        {
            get { return ModelAs<PDFContentItem>().Path; }
            set { ModelAs<PDFContentItem>().Path = value; OnPropertyChanged(nameof(Path)); }
        }
        public PDFContentItemViewModel(ContentItem model)
           : base(model)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is PDFContentItemViewModel model &&
                   EqualityComparer<Uri>.Default.Equals(Path, model.Path);
        }

        public override int GetHashCode()
        {
            return 467214278 + EqualityComparer<Uri>.Default.GetHashCode(Path);
        }
    }

}
