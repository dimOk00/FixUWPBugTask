using System;
using GhostCore.MVVM;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class SubMenuItemViewModel : ViewModelBase<SubMenuItem>
    {
        private bool _isCurrentSubMenuItem;

        public SubMenuItemViewModel(SubMenuItem subMenuItem) : base(subMenuItem)
        {
        }

        public static int FontSize { get; set; }

        public bool IsCurrentSubMenuItem
        {
            get => _isCurrentSubMenuItem;
            set
            {
                if (_isCurrentSubMenuItem != value)
                {
                    _isCurrentSubMenuItem = value;
                    OnPropertyChanged(nameof(IsCurrentSubMenuItem));
                }
            }
        }

        public string Title => Model.Title;
        public Uri Media => Model.Media;
    }
}
