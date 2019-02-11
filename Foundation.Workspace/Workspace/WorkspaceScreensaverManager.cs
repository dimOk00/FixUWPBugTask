using GhostCore.UWP.Input;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Foundation.Workspace
{
    public class WorkspaceScreensaverManager
    {
        private Workspace _workspace;

        public WorkspaceScreensaverManager(Workspace workspace)
        {
            _workspace = workspace;
            _workspace.TemplateApplied += workspace_TemplateApplied;
        }

        private void workspace_TemplateApplied(object sender, EventArgs e)
        {
            _workspace.TemplateApplied -= workspace_TemplateApplied;

            if (!IdleDetector.Instance.IsInitialized)
            {
                IdleDetector.Instance.Initialize(Window.Current.CoreWindow);
                IdleDetector.Instance.SetTimeout((int)(_workspace.AppData.Root.ScreensaverSettings.ActivationTime / 60));
            }

            IdleDetector.Instance.Idled += Instance_Idled;
            IdleDetector.Instance.Start();
        }

        private void Instance_Idled(object sender, EventArgs e)
        {
            _workspace.PART_ScreensaverHost.Visibility = Visibility.Visible;
            _workspace.PART_ScreensaverHost.Tapped += PART_ScreensaverHost_Tapped;

            IdleDetector.Instance.Stop();
        }

        private void PART_ScreensaverHost_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _workspace.PART_ScreensaverHost.Visibility = Visibility.Collapsed;
            _workspace.PART_ScreensaverHost.Tapped -= PART_ScreensaverHost_Tapped;
            IdleDetector.Instance.Start();
        }
    }
}
