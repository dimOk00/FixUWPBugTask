namespace Foundation.Core.Data
{
    public class WindowSettings
    {
        public double? MinWindowScale { get; set; }
        public double? MaxWindowScale { get; set; }
        public double? DefaultWidth { get; set; }
        public double? DefaultHeight { get; set; }
        public bool ShrinkToContent { get; set; } = true;
        public bool HasDropShadow { get; set; } = true;
        public double DropShadowOpacity { get; set; }
        public int DropShadowColor { get; set; }
        public bool InheritProperties { get; set; } = true;
        public int BackgroundColor { get; set; } = -3355444;
    }
}
