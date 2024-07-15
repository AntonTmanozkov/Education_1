using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.UI.Common
{
    public static class TimeSpanTools
    {
        public static int GetSeconds(TimeSpan span)
        {
            return (int)Math.Ceiling(span.TotalSeconds - span.Hours * 60 * 60 - span.Minutes * 60);
        }
    }
}
