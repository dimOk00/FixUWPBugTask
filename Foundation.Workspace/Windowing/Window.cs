using Foundation.Controls;
using Foundation.Core.Data;
using Foundation.Core.Events.Windows;
using GhostCore;
using GhostCore.Math;
using GhostCore.MVVM;
using GhostCore.MVVM.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Foundation.Workspace.Windowing
{
    [TemplatePart(Name = nameof(PART_CloseButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_FullscreenButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_LockButton), Type = typeof(ToggleButton))]
    [TemplatePart(Name = nameof(PART_Transform), Type = typeof(CompositeTransform))]
    [TemplatePart(Name = nameof(PART_ControlTransform), Type = typeof(CompositeTransform))]
    public sealed class WorkspaceWindow : Control
    {
        #region Dependency Properties

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(WorkspaceWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(WorkspaceWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(WorkspaceWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty IsLockedProperty =
           DependencyProperty.Register("IsLocked", typeof(bool), typeof(WorkspaceWindow), new PropertyMetadata(false, OnIsLockedChanged));

        public static readonly DependencyProperty IsFullscreenProperty =
            DependencyProperty.Register("IsFullscreen", typeof(bool), typeof(WorkspaceWindow), new PropertyMetadata(false, OnIsFullscreenChanged));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(RelayCommand), typeof(WorkspaceWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty RestoreCommandProperty =
            DependencyProperty.Register("RestoreCommand", typeof(RelayCommand), typeof(WorkspaceWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty ShadowColorProperty =
           DependencyProperty.Register("ShadowColor", typeof(Color), typeof(WorkspaceWindow), new PropertyMetadata(Colors.Black));

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(WorkspaceWindow), new PropertyMetadata(0.5));
        #endregion

        #region Fields

        private EventBus _windowOperationsBus;
        private TransformData _normalTransformData;
        private double _normalWidth;
        private double _normalHeight;

        #endregion

        #region Properties
        public RelayCommand CloseCommand
        {
            get { return (RelayCommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }
        public RelayCommand RestoreCommand
        {
            get { return (RelayCommand)GetValue(RestoreCommandProperty); }
            set { SetValue(RestoreCommandProperty, value); }
        }
        public bool IsFullscreen
        {
            get { return (bool)GetValue(IsFullscreenProperty); }
            set { SetValue(IsFullscreenProperty, value); }
        }
        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }


        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }

        #endregion

        #region Template Parts

        public Button PART_CloseButton { get; private set; }
        public Button PART_FullscreenButton { get; private set; }
        public Button PART_ScreenshotButton { get; private set; }
        public Panel PART_ContentContainer { get; private set; }
        public ToggleButton PART_LockButton { get; private set; }
        public CompositeTransform PART_Transform { get; private set; }
        public CompositeTransform PART_ControlTransform { get; private set; }
        public AnnotationControl PART_AnnotationControl { get; private set; }

        #endregion

        #region Init

        public WorkspaceWindow()
        {
            DefaultStyleKey = typeof(WorkspaceWindow);
            _normalTransformData = new TransformData();
            _windowOperationsBus = EventBusManager.Instance.GetOrCreateBus("WindowOperationBus");

            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            RestoreCommand = new RelayCommand(ExecuteRestoreCommand);
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_CloseButton = GetTemplateChild(nameof(PART_CloseButton)) as Button;
            PART_FullscreenButton = GetTemplateChild(nameof(PART_FullscreenButton)) as Button;
            PART_LockButton = GetTemplateChild(nameof(PART_LockButton)) as ToggleButton;
            PART_Transform = GetTemplateChild(nameof(PART_Transform)) as CompositeTransform;
            PART_ControlTransform = GetTemplateChild(nameof(PART_ControlTransform)) as CompositeTransform;
            PART_ScreenshotButton = GetTemplateChild(nameof(PART_ScreenshotButton)) as Button;
            PART_ContentContainer = GetTemplateChild(nameof(PART_ContentContainer)) as Panel;
            PART_AnnotationControl = GetTemplateChild(nameof(PART_AnnotationControl)) as AnnotationControl;

            AttachHandlers();

            var wndOp = CreateOpData(WindowOperation.Opened);
            _windowOperationsBus.Publish(wndOp, this);
        }

        #endregion

        #region Dependency Property Changes

        private static void OnIsLockedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as WorkspaceWindow;
            WindowOperationEventData evtData = null;
            if ((bool)e.NewValue)
                evtData = that.CreateOpData(WindowOperation.Lock);
            else
                evtData = that.CreateOpData(WindowOperation.Unlock);

            that._windowOperationsBus.Publish(evtData, that);
        }
        private static void OnIsFullscreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as WorkspaceWindow;
            WindowOperationEventData evtData = null;
            if ((bool)e.NewValue)
                evtData = that.CreateOpData(WindowOperation.Fullscreen);
            else
                evtData = that.CreateOpData(WindowOperation.Restore);

            that._windowOperationsBus.Publish(evtData, that);
        }


        #endregion

        #region Template Part Handlers

        private async void PART_ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO create separate class for screenshots

            PART_AnnotationControl.HideToolbar();
            var globalSettings = ServiceLocator.Instance.Resolve<GlobalSettings>();

            var screenshotPath = Path.Combine(globalSettings.RootFolder, "Screenshots");

            var screenshotsFolder = await StorageFolder.GetFolderFromPathAsync(screenshotPath);
            var screenshotFile = await screenshotsFolder.CreateFileAsync($"{DateTime.Now.Ticks}.jpg", CreationCollisionOption.GenerateUniqueName);

            var rtBitmap = new RenderTargetBitmap();
            await rtBitmap.RenderAsync(PART_ContentContainer); // TODO ask about this
            var pData = await rtBitmap.GetPixelsAsync();

            using (var fileStream = await screenshotFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, fileStream);

                var displayInformation = DisplayInformation.GetForCurrentView();
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)rtBitmap.PixelWidth,
                    (uint)rtBitmap.PixelHeight,
                    displayInformation.LogicalDpi,
                    displayInformation.LogicalDpi,
                    pData.ToArray());

                await encoder.FlushAsync();
            }
            PART_AnnotationControl.ShowToolbar();
        }

        private void PART_FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            IsFullscreen = !IsFullscreen;
        }
        private void PART_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCloseCommand();
        }
        private void PART_LockButton_Unchecked(object sender, RoutedEventArgs e)
        {
            IsLocked = false;
        }

        private void PART_LockButton_Checked(object sender, RoutedEventArgs e)
        {
            IsLocked = true;
        }

        #endregion

        #region Commands

        private void ExecuteRestoreCommand(object obj = null)
        {
            IsFullscreen = false;
        }

        private void ExecuteCloseCommand(object obj = null)
        {
            var wndOp = CreateOpData(WindowOperation.Close);
            _windowOperationsBus.Publish(wndOp, this);
        }

        #endregion

        #region Logic
        public void PrepareFullscreen()
        {
            _normalTransformData.TranslateX = PART_Transform.TranslateX;
            _normalTransformData.TranslateY = PART_Transform.TranslateY;
            _normalTransformData.ScaleX = PART_Transform.ScaleX;
            _normalTransformData.ScaleY = PART_Transform.ScaleY;
            _normalTransformData.Rotation = PART_Transform.Rotation;

            _normalWidth = ActualWidth;
            _normalHeight = ActualHeight;

            PART_Transform.TranslateX = 0;
            PART_Transform.TranslateY = 0;
            PART_Transform.ScaleX = 1;
            PART_Transform.ScaleY = 1;
            PART_Transform.Rotation = 0;
        }

        public void ChangeRestoreData(double width, double height)
        {
            _normalWidth = width;
            _normalHeight = height;
        }

        public void PrepareRestore()
        {
            PART_Transform.TranslateX = _normalTransformData.TranslateX;
            PART_Transform.TranslateY = _normalTransformData.TranslateY;
            PART_Transform.ScaleX = _normalTransformData.ScaleX;
            PART_Transform.ScaleY = _normalTransformData.ScaleY;
            PART_Transform.Rotation = _normalTransformData.Rotation;
        }

        public void SetPosition(double x, double y)
        {
            PART_Transform.TranslateX = x;
            PART_Transform.TranslateY = y;
        }

        public (double w, double h) GetRestoreData()
        {
            return (_normalWidth, _normalHeight);
        }

        #endregion

        #region Handler Management

        private void AttachHandlers()
        {
            if (PART_CloseButton != null) PART_CloseButton.Click += PART_CloseButton_Click;
            if (PART_FullscreenButton != null) PART_FullscreenButton.Click += PART_FullscreenButton_Click;
            if (PART_LockButton != null) PART_LockButton.Checked += PART_LockButton_Checked;
            if (PART_LockButton != null) PART_LockButton.Unchecked += PART_LockButton_Unchecked;
            if (PART_ScreenshotButton != null) PART_ScreenshotButton.Click += PART_ScreenshotButton_Click;
        }

        private void DetachHandlers()
        {
            if (PART_CloseButton != null) PART_CloseButton.Click -= PART_CloseButton_Click;
            if (PART_FullscreenButton != null) PART_FullscreenButton.Click -= PART_FullscreenButton_Click;
            if (PART_LockButton != null) PART_LockButton.Checked -= PART_LockButton_Checked;
            if (PART_LockButton != null) PART_LockButton.Unchecked -= PART_LockButton_Unchecked;
            if (PART_ScreenshotButton != null) PART_ScreenshotButton.Click -= PART_ScreenshotButton_Click;
        }

        #endregion

        #region Helpers

        private WindowOperationEventData CreateOpData(WindowOperation op)
        {
            return new WindowOperationEventData()
            {
                Operation = op,
                WindowObject = this,
                DataObject = Content
            };
        }

        public CompositeTransform GetTransform()
        {
            return PART_Transform;
        }
        public CompositeTransform GetControlTransform()
        {
            return PART_ControlTransform;
        }

        #endregion
    }
}
