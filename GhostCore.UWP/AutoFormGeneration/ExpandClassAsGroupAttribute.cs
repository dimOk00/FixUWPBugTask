﻿using System;

namespace GhostCore.UWP.AutoFormGeneration
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ExpandClassAsGroupAttribute : Attribute
    {
        public string GroupLabel { get; set; }
    }
}
