using Foundation.Core.Data;
using GhostCore.MVVM;
using Newtonsoft.Json;
using System;

namespace Foundation.Core.ViewModels
{
    public abstract class HostedLayoutItemViewModel : ViewModelBase<HostedLayoutItem>
    {
        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; OnPropertyChanged(nameof(Name)); }
        }

        public double X
        {
            get { return Model.X; }
            set { Model.X = value; OnPropertyChanged(nameof(X)); }
        }

        public double Y
        {
            get { return Model.Y; }
            set { Model.Y = value; OnPropertyChanged(nameof(Y)); }
        }

        public double Width
        {
            get { return Model.Width; }
            set { Model.Width = value; OnPropertyChanged(nameof(Width)); }
        }

        public double Height
        {
            get { return Model.Height; }
            set { Model.Height = value; OnPropertyChanged(nameof(Height)); }
        }

        public HostedLayoutItemViewModel(HostedLayoutItem model)
            : base(model)
        {

        }

        public abstract HostedLayoutItemViewModel Clone();
    }
}
