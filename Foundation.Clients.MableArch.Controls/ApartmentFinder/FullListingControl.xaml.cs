using Foundation.Core.ViewModels;
using Foundation.Data;
using GhostCore;
using GhostCore.MVVM.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Foundation.Clients.MableArch.Controls.ApartmentFinder
{
    public sealed partial class FullListingControl : UserControl
    {
        private EventBus _menuSelectedItemBus;
        internal DataSchema Schema
        {
            get { return (DataSchema)GetValue(SchemaProperty); }
            set { SetValue(SchemaProperty, value); }
        }

        public ApartmentFinderViewModel ViewModel
        {
            get { return (ApartmentFinderViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty SchemaProperty =
            DependencyProperty.Register("Schema", typeof(DataSchema), typeof(FullListingControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ApartmentFinderViewModel), typeof(FullListingControl), new PropertyMetadata(null, OnViewModelChanged));

        public FullListingControl()
        {
            _menuSelectedItemBus = EventBusManager.Instance.GetOrCreateBus(nameof(ContentItemViewModel.MenuSelectedItemBus));
            InitializeComponent();
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as FullListingControl;
            that.OnViewModelChanged(e.NewValue as ApartmentFinderViewModel);
        }

        private async void OnViewModelChanged(ApartmentFinderViewModel vm)
        {
            var schemaJson = await FileIO.ReadTextAsync(await StorageFile.GetFileFromPathAsync(vm.SchemaDefinition.LocalPath));
            Schema = JsonConvert.DeserializeObject<DataSchema>(schemaJson);

            foreach (var item in Schema.DataItems)
            {
                if (item.IsIdentifier)
                    continue;

                var renderer = new DataSchemaItemRenderer();
                renderer.Schema = item;
                renderer.ValueChanged += Renderer_ValueChanged;

                pnlFilterPanel.Children.Add(renderer);
            }
        }

        private void Renderer_ValueChanged(object sender, object e)
        {
            // DO SOMETHING
        }

        private void btnApt1_Click(object sender, RoutedEventArgs e)
        {
            var q = GetContentItem(100);
            _menuSelectedItemBus.Publish(q, this);
        }

        private void btnApt2_Click(object sender, RoutedEventArgs e)
        {
            var q = GetContentItem(101);
            _menuSelectedItemBus.Publish(q, this);
        }

        private void btnApt3_Click(object sender, RoutedEventArgs e)
        {
            var q = GetContentItem(102);
            _menuSelectedItemBus.Publish(q, this);
        }
        private void btnApt5_Click(object sender, RoutedEventArgs e)
        {
            var q = GetContentItem(103);
            _menuSelectedItemBus.Publish(q, this);
        }

        private void btnPenthouse_Click(object sender, RoutedEventArgs e)
        {
            var q = GetContentItem(104);
            _menuSelectedItemBus.Publish(q, this);
        }

        private ContentItemViewModel GetContentItem(int id)
        {
            var rcvm = ServiceLocator.Instance.Resolve<RootConfigurationViewModel>();
            var ses = rcvm.SelectedSession;
            if (ses == null)
                ses = rcvm.Sessions[0];

            var floorplaRoot = ses.Root.Children.FirstOrDefault(x => x.Name == "FLOORPLAN");
            return floorplaRoot.Children.FirstOrDefault(x => x.Id == id);
        }
    }
}
