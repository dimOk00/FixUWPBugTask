using Foundation.Core.ControlData;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml.Media.Animation;
using EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode;
using EasingType = Microsoft.Toolkit.Uwp.UI.Animations.EasingType;
using FEasingMode = Foundation.Core.ControlData.EasingMode;
using FEasingType = Foundation.Core.ControlData.EasingType;

namespace Foundation.Workspace
{
    public class WorkspaceBackgroundManager
    {
        private Workspace _workspace;
        private BlurParameter _on;
        private BlurParameter _off;

        public bool EnableBlurring { get; set; } = true;

        public WorkspaceBackgroundManager(Workspace workspace)
        {
            _workspace = workspace;

            _workspace.TemplateApplied += workspace_TemplateApplied;
            _workspace.WindowDataSource.CollectionChanged += WindowDataSource_CollectionChanged;
        }

        private void workspace_TemplateApplied(object sender, System.EventArgs e)
        {
            _workspace.TemplateApplied -= workspace_TemplateApplied;
            _on = _workspace.AppData.Root.WorkspaceSettings.BlurParameterOn;
            _off = _workspace.AppData.Root.WorkspaceSettings.BlurParameterOff;
        }

        private void WindowDataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!EnableBlurring)
                return;

            if (_workspace.WindowDataSource.Count != 0)
            {
                _workspace.PART_BackgroundHost.Blur(
                    value: _on.BlurRadius,
                    duration: _on.TransitionDuration,
                    delay: _on.DelayOn,
                    easingType: (EasingType)_on.EasingType,
                    easingMode: (EasingMode)_on.EasingMode).Start();
            }
            else
            {
                _workspace.PART_BackgroundHost.Blur(
                    value: _off.BlurRadius,
                    duration: _off.TransitionDuration,
                    delay: _off.DelayOn,
                    easingType: (EasingType)_off.EasingType,
                    easingMode: (EasingMode)_off.EasingMode).Start();
            }
        }
    }
}
