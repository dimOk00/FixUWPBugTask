using Foundation.Clients.MableArch.Controls;
using Foundation.Core;
using Foundation.Core.ControlData;
using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using Foundation.Workspace;
using GhostCore;
using GhostCore.MVVM;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Clients.MableArch.Components.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private WorkspaceBackgroundViewModel _workspaceBackground;
        private RootConfigurationViewModel _rootConfig;

        public RootConfigurationViewModel RootConfiguration
        {
            get { return _rootConfig; }
            set { _rootConfig = value; OnPropertyChanged(nameof(RootConfiguration)); }
        }

        public MainPageViewModel()
        {
            var foundationConfig = new FoundationConfiguration
            {
                ContentItemViewModelFactory = new CustomContentItemViewModelFactoryFactory()
            };

            ServiceLocator.Instance.Register(foundationConfig);

            var config = ServiceLocator.Instance.Resolve<RootConfiguration>();
            _rootConfig = new RootConfigurationViewModel(config);
            _workspaceBackground = _rootConfig.SelectedSession.Root.WorkspaceSettings;

            ServiceLocator.Instance.Register(RootConfiguration);
        }
    }
}
