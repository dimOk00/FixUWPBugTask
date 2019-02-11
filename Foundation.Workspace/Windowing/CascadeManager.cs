using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation.Core;
using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using Foundation.Workspace.Menu;
using GhostCore;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Foundation.Workspace.Windowing
{
    public class CascadeManager
    {
        private Workspace _workspace;
        private Menu.Menu _menu;
        private Point _lastMenuPosition;
        private RootConfigurationViewModel _rootConfig;
        private int _posMul = 0;

        public int CascadeOffsetX { get; set; }
        public int CascadeOffsetY { get; set; }
        public int DistanceFromMenu { get; set; }
        public HostType HostType { get; internal set; }

        public CascadeManager(Workspace workspace)
        {
            //TODO get from config
            CascadeOffsetX = 30;
            CascadeOffsetY = 20;
            DistanceFromMenu = 70;

            _workspace = workspace;
            _rootConfig = ServiceLocator.Instance.Resolve<RootConfigurationViewModel>();
        }


        public void SetMenu(Menu.Menu menu)
        {
            _menu = menu;
        }

        public (double x, double y) GetSpawnCoordinates()
        {

            if (HostType == HostType.Tablet)
                return (0, 0);

            var curMenuPos = new Point(Canvas.GetLeft(_menu), Canvas.GetTop(_menu));
            if (_lastMenuPosition != curMenuPos)
            {
                _posMul = 0;
                _lastMenuPosition = curMenuPos;
            }

            var defW = _rootConfig.SelectedSession.Root.WindowSettings.DefaultWidth;
            var defH = _rootConfig.SelectedSession.Root.WindowSettings.DefaultHeight;

            bool hasSpaceOnRight = true;
            bool hasSpaceOnBottom = true;

            if (_workspace.ActualWidth - (_lastMenuPosition.X + _menu.ActualWidth + DistanceFromMenu + CascadeOffsetX * _posMul) < defW)
                hasSpaceOnRight = false;

            if (_workspace.ActualHeight - (_lastMenuPosition.Y + DistanceFromMenu + CascadeOffsetY * _posMul) < defH)
                hasSpaceOnBottom = false;

            double x = 0;
            double y = 0;

            if (hasSpaceOnRight)
            {
                x = _lastMenuPosition.X + _menu.ActualWidth + DistanceFromMenu + CascadeOffsetX * _posMul;
            }
            else
            {
                x = _lastMenuPosition.X - (DistanceFromMenu + defW) + CascadeOffsetX * _posMul;
            }

            if (hasSpaceOnBottom)
            {
                y = _lastMenuPosition.Y + DistanceFromMenu + CascadeOffsetY * _posMul;
            }
            else
            {
                y = _lastMenuPosition.Y - (DistanceFromMenu + defH) + CascadeOffsetY * _posMul;
            }

            _posMul++;
            return (x, y);

        }
    }
}
