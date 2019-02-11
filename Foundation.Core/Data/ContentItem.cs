using Foundation.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Core.Data
{
    public class ContentItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayLabel { get; set; }
        public bool IsHidden { get; set; }
        public bool IsSingleton { get; set; }
        public ContentItemType Type { get; set; }
        public WindowSettings WindowSettings { get; set; }
        public HostType DisplayTarget { get; set; }

        [EventSerializerIgnore]
        public List<ContentItem> Children { get; set; }

        public ContentItem()
        {
            Children = new List<ContentItem>();
            WindowSettings = new WindowSettings();
        }

        public override bool Equals(object obj)
        {
            return obj is ContentItem item && Id == item.Id && item.Id + Id != 0;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
    }

    public abstract class MediaContentItem : ContentItem
    {
        public Uri Path { get; set; }
        public bool IsSlideshow { get; set; }
        public TimeSpan SlideInterval { get; set; }
    }

    public class ImageContentItem : MediaContentItem
    {
        
    }

    public class PDFContentItem : MediaContentItem
    {
    }

    public class VideoContentItem : MediaContentItem
    {
    }

    public class MultimediaContentItem : MediaContentItem
    {
        public bool AutoInferFromPath { get; set; } = true;
    }

    public class GalleryContentItem : MediaContentItem
    {
        public bool AutoInferFromPath { get; set; } = true;
    }

    public class WebContentItem : ContentItem
    {
        public Uri URL { get; set; }
    }

    public enum ContentItemType
    {
        MenuItem = 0,
        Image = 1,
        Video = 2,
        PDF = 3,
        MultimediaControl = 4,
        Gallery = 5,
        HostedLayout = 6,
        ProjectSpecific = 7,
        WebView = 8
    }
}
