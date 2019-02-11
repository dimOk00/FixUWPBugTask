using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Core.ControlData
{
    public class WorkspaceBackground
    {
        public Uri BackgroundSource { get; set; }
        public BackgroundType Type { get; set; }
        public bool EnableBlurring { get; internal set; }

        public BlurParameter BlurParameterOn { get; set; }
        public BlurParameter BlurParameterOff { get; set; }

        public WorkspaceBackground()
        {
            BlurParameterOn = new BlurParameter()
            {
                BlurRadius = 15,
                TransitionDuration = 500,
                DelayOn = 0,
                EasingMode = EasingMode.EaseInOut,
                EasingType = EasingType.Quadratic
            };

            BlurParameterOff = new BlurParameter()
            {
                BlurRadius = 0,
                TransitionDuration = 290,
                DelayOn = 0,
                EasingMode = EasingMode.EaseInOut,
                EasingType = EasingType.Quadratic
            };
        }
    }

    public class BlurParameter
    {
        public double TransitionDuration { get; set; }
        public double BlurRadius { get; set; }
        public double DelayOn { get; set; }
        public EasingMode EasingMode { get; set; }
        public EasingType EasingType { get; set; }
    }

    public enum EasingType : byte
    {
        Default = 0,
        Linear = 1,
        Cubic = 2,
        Back = 3,
        Bounce = 4,
        Elastic = 5,
        Circle = 6,
        Quadratic = 7,
        Quartic = 8,
        Quintic = 9,
        Sine = 10
    }

    public enum EasingMode : byte
    {
        EaseOut = 0,
        EaseIn = 1,
        EaseInOut = 2
    }
}
