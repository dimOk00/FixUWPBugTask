namespace Foundation.Core.Data
{
    public abstract class HostedLayoutItem
    {
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public enum HLItemType
    {
        ContentArea,
        InteractableItem,
        ToggleItem,
        ButtonItem
    }
}
