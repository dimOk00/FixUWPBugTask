using Foundation.Core.Data;
using System;
using System.Linq;

namespace Foundation.Core.ViewModels
{
    public class ButtonItemViewModel : InteractableItemViewModel
    {
        public override string ItemTitle => "Button Item";
        public Uri ImagePath
        {
            get { return ModelAs<ButtonItem>().ImagePath; }
            set { ModelAs<ButtonItem>().ImagePath = value; OnPropertyChanged(nameof(ImagePath)); }
        }

        public bool UseImage
        {
            get { return ModelAs<ButtonItem>().UseImage; }
            set { ModelAs<ButtonItem>().UseImage = value; OnPropertyChanged(nameof(UseImage)); }
        }
        public string StyleKey
        {
            get { return ModelAs<ButtonItem>().StyleKey; }
            set { ModelAs<ButtonItem>().StyleKey = value; OnPropertyChanged(nameof(StyleKey)); }
        }
        public ButtonItemViewModel(HostedLayoutItem model) : base(model)
        {
        }

        public override HostedLayoutItemViewModel Clone()
        {
            var oldContent = ModelAs<ButtonItem>();

            var newContent = new ButtonItem()
            {
                Height = oldContent.Height,
                Name = oldContent.Name,
                Shape = oldContent.Shape.ToList(),
                Width = oldContent.Width,
                X = oldContent.X,
                Y = oldContent.Y,
                Action = oldContent.Action,
                ContentAreaTarget = null,
                HighlightColor = oldContent.HighlightColor,
                IsNonStandardHitArea = oldContent.IsNonStandardHitArea,
                ItemTarget = oldContent.ItemTarget,
                PageTarget = oldContent.PageTarget,
                ImagePath = oldContent.ImagePath,
                StyleKey = oldContent.StyleKey,
                UseImage = oldContent.UseImage
            };

            var newVm = new ButtonItemViewModel(newContent);

            return newVm;
        }

    }
}
