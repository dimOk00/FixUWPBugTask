using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace GhostCore.UWP.Controls
{
    [TemplatePart(Name = "PART_ContentControl", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_ControlButton", Type = typeof(Button))]
    public class Chip : Control
    {
        #region Events

        public event EventHandler ControlButtonPressed;

        #endregion

        #region Template Parts

        internal ContentControl PART_ContentControl { get; set; }
        internal Button PART_ControlButton { get; set; }

        #endregion

        #region Properties

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }



        public Style ControlButtonStyle
        {
            get { return (Style)GetValue(ControlButtonStyleProperty); }
            set { SetValue(ControlButtonStyleProperty, value); }
        }


        public ICommand ControlButtonCommand
        {
            get { return (ICommand)GetValue(ControlButtonCommandProperty); }
            set { SetValue(ControlButtonCommandProperty, value); }
        }

        #endregion

        #region Dependecy Properties

        public static readonly DependencyProperty ControlButtonStyleProperty =
            DependencyProperty.Register("ControlButtonStyle", typeof(Style), typeof(Chip), new PropertyMetadata(null));

        public static readonly DependencyProperty ControlButtonCommandProperty =
            DependencyProperty.Register("ControlButtonCommand", typeof(ICommand), typeof(Chip), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(Chip), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(Chip), new PropertyMetadata(null));

        #endregion

        public Chip()
        {
            DefaultStyleKey = typeof(Chip);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ContentControl = GetTemplateChild(nameof(PART_ContentControl)) as ContentControl;
            PART_ControlButton = GetTemplateChild(nameof(PART_ControlButton)) as Button;

            PART_ControlButton.Click += PART_ControlButton_Click;
        }

        private void PART_ControlButton_Click(object sender, RoutedEventArgs e)
        {
            OnControlButtonPressed();
        }

        protected void OnControlButtonPressed()
        {
            if (ControlButtonPressed == null)
                return;

            ControlButtonPressed(this, EventArgs.Empty);
        }

    }
}
