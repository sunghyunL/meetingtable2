using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TouchFramework.ControlHandlers
{
    internal struct MouseObject
    {
        public int Key;
        public Point point;
    }

    class MouseAdorner : Adorner
    {
        private Dictionary<int, MouseObject> MousePosition;
        private Pen Stroke1 = new Pen(Brushes.SkyBlue, 4);
        private Pen Stroke2 = new Pen(Brushes.DeepSkyBlue, 4);

        public MouseAdorner(UIElement element)
            : base(element)
        {
            MousePosition = new Dictionary<int, MouseObject>();
            AdornerLayer Layer = AdornerLayer.GetAdornerLayer(element);
            Layer.Add(this);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            foreach (MouseObject mo in MousePosition.Values)
            {
                if (mo.Key == 0)
                {
                    drawingContext.DrawEllipse(Brushes.Transparent, Stroke1, new Point(mo.point.X, mo.point.Y), 10.0, 10.0);
                }
                else
                {
                    drawingContext.DrawEllipse(Brushes.Transparent, Stroke2, new Point(mo.point.X, mo.point.Y), 10.0, 10.0);
                }
            }

            this.IsHitTestVisible = false;
            base.OnRender(drawingContext);
        }

        public void AddObject(MouseObject mo)
        {
            MousePosition[mo.Key] = mo;
            this.InvalidateVisual();
        }

        public void RemoveObject(int Key)
        {
            if (MousePosition.ContainsKey(Key))
            {
                MousePosition.Remove(Key);
                this.InvalidateVisual();
            }
        }
    }
}

