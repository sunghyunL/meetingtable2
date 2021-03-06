﻿/*
TouchFramework connects touch tracking from a tracking engine to WPF controls 
allow scaling, rotation, movement and other multi-touch behaviours.

Copyright 2009 - Mindstorm Limited (reg. 05071596)

Author - Simon Lerpiniere

This file is part of TouchFramework.

TouchFramework is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

TouchFramework is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser Public License for more details.

You should have received a copy of the GNU Lesser Public License
along with TouchFramework.  If not, see <http://www.gnu.org/licenses/>.

If you have any questions regarding this library, or would like to purchase 
a commercial licence, please contact Mindstorm via www.mindstorm.com.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// Implements custom behaviour to handle the slide of a thumb on a slider.
    /// </summary>
    public class SliderHandler : ElementHandler
    {
        public override void Slide(float x, float y)
        {
            Slider c = this.Source as Slider;
            if (c == null) return;

            double diff = c.Maximum - c.Minimum;
            double m = c.Orientation == Orientation.Horizontal ? x : y;
            double sz = c.Orientation == Orientation.Horizontal ? c.RenderSize.Width : c.RenderSize.Height;

            double movePercent = ((double)m / sz) * 100;
            double valMove = (diff / 100) * movePercent;

            c.Value += valMove;

            base.Slide(x, y);
        }
    }
}
