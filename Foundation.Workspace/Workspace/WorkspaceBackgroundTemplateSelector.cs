using Foundation.Core.ControlData;
using Foundation.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Workspace
{
    public class WorkspaceBackgroundTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }
        public DataTemplate SlideshowTemplate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is WorkspaceBackgroundViewModel dataObject)
            {
                switch (dataObject.Type)
                {
                    case Core.BackgroundType.Image:
                        return ImageTemplate;
                    case Core.BackgroundType.Video:
                        return VideoTemplate;
                    case Core.BackgroundType.Slideshow:
                        return SlideshowTemplate;
                    default:
                        break;
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
