using Foundation.Core.ViewModels;
using GhostCore.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Foundation.Controls.HostedLayout
{
    public sealed partial class ContentAreaTargetRenderer : UserControl
    {
        public ContentAreaViewModel ViewModel
        {
            get { return (ContentAreaViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public RelayCommand ClosePopupCommand { get; set; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ContentAreaViewModel), typeof(ContentAreaTargetRenderer), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as ContentAreaTargetRenderer;
            that.DataContext = e.NewValue;
            (e.NewValue as ContentAreaViewModel).PropertyChanged += ContentAreaTargetRenderer_PropertyChanged;
        }

        private static void ContentAreaTargetRenderer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ContentAreaViewModel.Content))
            {

            }
        }

        public ContentAreaTargetRenderer()
        {
            ClosePopupCommand = new RelayCommand(OnClosePopupCommand);
            InitializeComponent();
        }

        private void OnClosePopupCommand(object obj)
        {
            ViewModel.Content = null;
        }
    }
}
