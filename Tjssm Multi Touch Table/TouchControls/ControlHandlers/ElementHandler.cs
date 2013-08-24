/*
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

using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using TouchFramework.Helpers;
using TouchFramework.Events;
using TouchFramework.ControlHandlers.Contacts;

namespace TouchFramework.ControlHandlers
{
    /// <summary>
    /// Provides default behaviour which is applied to all multi-touch element.
    /// Derive from this class to create custom behaviors for different types of element.
    /// Note: Currently the static GetHandler method needs to be modified to support additional added handlers.
    /// </summary>
    public class ElementHandler
    {
        public static ElementHandler GetHandler(FrameworkElement source, Panel cont)
        {
            ElementHandler handler = null;
            if (source is SmartArea)
            {
                handler = new SmartAreaHandler() as ElementHandler;
            }
            else if (source is phoneMenu)
            {
                handler = new PhoneMenuHandler() as ElementHandler;
            }
            else if (source is FileBox3)
            {
                handler = new FileBoxHandler() as ElementHandler;
            }
            else if (source is ImageView)
            {
                handler = new ImageViewHandler() as ElementHandler;
            }
            else if (source is PptViewer)
            {
                handler = new PptViewHandler() as ElementHandler;
            }
            else if (source is DocViewer)
            {
                handler = new DocViewHandler() as ElementHandler;
            }
            else if (source is ContactsBox)
            {
                handler = new ContactsBoxHandler() as ElementHandler;
            }
            else if (source is ContactsObject)
            {
                handler = new ContactsObjectHandler() as ElementHandler;
            }
            else if (source is TextBox)
            {
                handler = new TextBoxHandler() as ElementHandler;
            }
            else if (source is Button)
            {
                handler = new ButtonHandler() as ElementHandler;
            }
            else if (source is CheckBox)
            {
                handler = new CheckBoxHandler() as ElementHandler;
            }
            else if (source is Slider)
            {
                handler = new SliderHandler() as ElementHandler;
            }
            else if (source is ListBox)
            {
                handler = new ListBoxHandler() as ElementHandler;
            }
            else if (source is RssList)
            {
                handler = new RssListHandler() as ElementHandler;
            }
            else if (source is VideoControl)
            {
                handler = new VideoControlHandler() as ElementHandler;
            }
            else
            {
                handler = new ButtonHandler() as ElementHandler;
            }
            handler.Source = source;
            handler.Container = cont;
            return handler;
        }

        public FrameworkElement Source = null;
        public Panel Container = null;
        public bool isListBox = false;
        public bool isDocViewer = false;

        /// <summary>
        /// Raises the tap event as a routed event on the element.
        /// </summary>
        /// <param name="p">Relative point in element space</param>
        public virtual void Tap(PointF p)
        {
            this.Source.RaiseEvent(new RoutedEventArgs(MTEvents.TapEvent, Source));
        }

        /// <summary>
        /// Checks if the element has a scroll viewer.  It it does, uses the scroll viewer
        /// to scroll the content by the number of pixels moved.
        /// X and Y are inverted so that the scroll works like the iPhone (i.e. the touched
        /// item sticks to your finger).  Works better without scrollbars shown.
        /// </summary>
        /// <param name="x">Change in the horizontal object relative movement center in pixels</param>
        /// <param name="y">Change in the vertical object relative movement center in pixels</param>
        public virtual void Scroll(float x, float y)
        {
            if (Source is FileBox3)
            {
                FileBox3 s = Source as FileBox3;

                if (isListBox == true)
                {
                    // This sets it so that we scroll by pixels rather than by lines
                    if (ScrollViewer.GetCanContentScroll(s.itemListBox)) ScrollViewer.SetCanContentScroll(s.itemListBox, false);

                    // Find the scrollviewer and scroll it if there is one
                    ScrollViewer scroll = s.itemListBox.FindChild<ScrollViewer>() as ScrollViewer;
                    if (scroll != null)
                    {
                        scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + (x * -1));
                        scroll.ScrollToVerticalOffset(scroll.VerticalOffset + (y * -1));
                    }

                    s.itemListBox.RaiseEvent(new RoutedEventArgs(MTEvents.ScrollEvent, Source));
                }
            }
            else if (Source is DocViewer)
            {
                DocViewer s = Source as DocViewer;
                
                if (isDocViewer == true)
                {
                    // This sets it so that we scroll by pixels rather than by lines
                    if (ScrollViewer.GetCanContentScroll(s.documentviewWord)) ScrollViewer.SetCanContentScroll(s.documentviewWord, false);

                    // Find the scrollviewer and scroll it if there is one
                    ScrollViewer scroll = s.documentviewWord.FindChild<ScrollViewer>() as ScrollViewer;
                    if (scroll != null)
                    {
                        scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + (x * -1));
                        scroll.ScrollToVerticalOffset(scroll.VerticalOffset + (y * -1));
                    }

                    s.documentviewWord.RaiseEvent(new RoutedEventArgs(MTEvents.ScrollEvent, Source));
                }
            }
            else if (Source is ContactsBox)
            {
                ContactsBox s = Source as ContactsBox;

                if (isListBox == true)
                {
                    // This sets it so that we scroll by pixels rather than by lines
                    if (ScrollViewer.GetCanContentScroll(s.ContactslistBox)) ScrollViewer.SetCanContentScroll(s.ContactslistBox, false);

                    // Find the scrollviewer and scroll it if there is one
                    ScrollViewer scroll = s.ContactslistBox.FindChild<ScrollViewer>() as ScrollViewer;
                    if (scroll != null)
                    {
                        scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + (x * -1));
                        scroll.ScrollToVerticalOffset(scroll.VerticalOffset + (y * -1));
                    }

                    s.ContactslistBox.RaiseEvent(new RoutedEventArgs(MTEvents.ScrollEvent, Source));
                }
            }
            else
            {// This sets it so that we scroll by pixels rather than by lines
                if (ScrollViewer.GetCanContentScroll(Source)) ScrollViewer.SetCanContentScroll(Source, false);

                // Find the scrollviewer and scroll it if there is one
                ScrollViewer scroll = Source.FindChild<ScrollViewer>() as ScrollViewer;
                if (scroll != null)
                {
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + (x * -1));
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset + (y * -1));
                }

                Source.RaiseEvent(new RoutedEventArgs(MTEvents.ScrollEvent, Source));
            }
        }

        /// <summary>
        /// Raises the Drag event as a routed event on the element.
        /// Override this to implement custom drag functionality.
        /// </summary>
        /// <param name="x">Horizontal coordinate in screen space of the center of movement.</param>
        /// <param name="y">Vertical coordinate in screen space of the center of movement.</param>
        public virtual void Drag(float x, float y)
        {
            Source.RaiseEvent(new RoutedEventArgs(MTEvents.DragEvent, Source));
        }

        /// <summary>
        /// Raises the Drag event as a routed event on the element.
        /// Override this to implement custom CenterNotify behaviour.
        /// </summary>
        /// <param name="global">Center of movement relative to the whole canvas</param>
        /// <param name="relative">Center of movement relative to the object</param>
        public virtual void Drag(PointF global, PointF relative)
        {
            //this.Drag(global.X, global.Y);
            this.Drag(relative.X, relative.Y);
        }

        /// <summary>
        /// Raises the Slide event as a routed event on the element.
        /// Overeride this to implement custom slide behaviour.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void Slide(float x, float y)
        {
            Source.RaiseEvent(new RoutedEventArgs(MTEvents.SlideEvent, Source));
        }

        /// <summary>
        /// Raises the TouchDown event as a routed event on the element.
        /// Override this to implement custom TouchDown behaviour.
        /// </summary>
        /// <param name="p">Point in element relative space of the touch center.</param>
        public virtual void TouchDown(PointF p)
        {
            Source.RaiseEvent(new RoutedEventArgs(MTEvents.TouchDownEvent, Source));
        }

        /// <summary>
        /// Raises the TouchUp event as a routed event on the element.
        /// Override this to implement custom TouchUp behaviour.
        /// </summary>
        /// <param name="p">Last valid point in element relative space of the touch center.</param>
        public virtual void TouchUp(PointF p)
        {
            Source.RaiseEvent(new RoutedEventArgs(MTEvents.TouchUpEvent, Source));
        }
    }
}
