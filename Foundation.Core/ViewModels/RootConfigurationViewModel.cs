using GhostCore.Collections;
using GhostCore.MVVM;
using System.Collections.Generic;
using Foundation.Core.Data;
using System;

namespace Foundation.Core.ViewModels
{
    public class RootConfigurationViewModel : ViewModelBase<RootConfiguration>
    {
        private static Array _hostTypeSource = Enum.GetValues(typeof(HostType));
        private SessionViewModel _selectedSession;

        public ModelUpdatingCollection<string> EnvironmentVariables { get; set; }
        public ViewModelCollection<SessionViewModel, Session> Sessions { get; set; }

        public HostType HostType
        {
            get { return Model.HostType; }
            set { Model.HostType = value; OnPropertyChanged(nameof(HostType)); }
        }
        public SessionViewModel SelectedSession
        {
            get { return _selectedSession; }
            set { _selectedSession = value; OnPropertyChanged(nameof(SelectedSession)); }
        }

        public Array HostTypeSource
        {
            get { return _hostTypeSource; }
            set { _hostTypeSource = value; OnPropertyChanged(nameof(HostTypeSource)); }
        }

        public RootConfigurationViewModel()
            : base(new RootConfiguration())
        {
            if (Model.Sessions.Count == 0)
            {
                Model.Sessions.Add(new Session()
                {
                    Name = "Default Session",
                    ApplicationSettings = new ApplicationSettings()
                });
            }

            var lst = new List<SessionViewModel>();
            foreach (var item in Model.Sessions)
            {
                var vm = new SessionViewModel(item);
                vm.Parent = this;
                lst.Add(vm);
            }

            Sessions = new ViewModelCollection<SessionViewModel, Session>(Model.Sessions, lst);
            EnvironmentVariables = new ModelUpdatingCollection<string>(Model.EnvironmentVariables, Model.EnvironmentVariables);

            _selectedSession = Sessions[0];
        }

        public RootConfigurationViewModel(RootConfiguration model)
            : base(model)
        {
            if (model.Sessions == null)
                throw new ArgumentException("No Sessions found. Please check your json");

            var lst = new List<SessionViewModel>();
            foreach (var item in model.Sessions)
            {
                var vm = new SessionViewModel(item);
                vm.Parent = this;
                lst.Add(vm);
            }

            Sessions = new ViewModelCollection<SessionViewModel, Session>(model.Sessions, lst);
            EnvironmentVariables = new ModelUpdatingCollection<string>(model.EnvironmentVariables, model.EnvironmentVariables);

            _selectedSession = Sessions[0];
        }
    }

}
