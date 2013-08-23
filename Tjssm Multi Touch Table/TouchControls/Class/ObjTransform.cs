using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TouchFramework.ControlHandlers.Class
{
    class ObjTransform
    {
        #region Variable
        private Window Window;
        private FrameworkElement Element;
        private FrameworkElement EventElement;

        public TransformGroup TransGroup = new TransformGroup();
        public RotateTransform RotateTrans = new RotateTransform();
        public ScaleTransform ScaleTrans = new ScaleTransform();
        public TranslateTransform TranslateTrans = new TranslateTransform();

        private bool IsDragging = false;
        private bool IsScaling = false;
        private bool IsRotating = false;

        private double FirstLength;
        private double FirstAngle;
        private Point FirstCoordinate;

        private Point FirstPoint;
        private bool IsTransformation = false;
        private int OldZindex;
        private static MouseAdorner Adorner = null;
        private static MouseObject FirstPosition, SecondPosition;

        public bool CanBeDragged = false;
        public bool CanBeScaled = false;
        public bool CanBeRotated = false;
        public bool ShowPosition = false;
        #endregion

        public ObjTransform(FrameworkElement eventElement, FrameworkElement element, Window window)
        {
            Window = window;
            Element = element;
            EventElement = eventElement;

            EventElement.PreviewMouseDown += Element_PreviewMouseDown;
            EventElement.PreviewMouseUp += Element_PreviewMouseUp;
            EventElement.MouseLeave += Element_MouseLeave;

            TransGroup.Children.Add(RotateTrans);
            TransGroup.Children.Add(ScaleTrans);
            TransGroup.Children.Add(TranslateTrans);
        }

        #region Sacle & Rotate
        /// <summary>
        /// 객체의 크기 변경 및 회전
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransformationeEvent(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                Point CurrentPoint = e.GetPosition(Window);
                Point DistanceVector = new Point((CurrentPoint.X - FirstPoint.X), (FirstPoint.Y - CurrentPoint.Y));

                double CurrentLength = Math.Sqrt(DistanceVector.X * DistanceVector.X + DistanceVector.Y * DistanceVector.Y);
                double CurrentAngle = Math.Atan2(DistanceVector.Y, DistanceVector.X) * 180 / Math.PI;

                // 포인터 위치 표시
                if (ShowPosition)
                {
                    ChangePosition(SecondPosition, CurrentPoint);
                }

                // 크기 변경
                if (CanBeScaled)
                {
                    if (!IsScaling)
                    {
                        // 처음 비율
                        FirstLength = CurrentLength / ScaleTrans.ScaleX;
                        IsScaling = true;
                    }

                    if (FirstLength > 0)
                    {
                        // 현재 비율
                        double Scale = CurrentLength / FirstLength;
                        ScaleTrans.ScaleX = Scale;
                        ScaleTrans.ScaleY = Scale;
                    }
                }

                // 회전
                if (CanBeRotated)
                {
                    if (!IsRotating)
                    {
                        // 처음 각도
                        FirstAngle = CurrentAngle + RotateTrans.Angle;
                        IsRotating = true;
                    }

                    // 현재 각도
                    double AngleDiff = (FirstAngle - CurrentAngle + 360) % 360;
                    RotateTrans.Angle = AngleDiff;
                }

                Drag(FirstPoint);
            }
        }
        #endregion

        #region Drag
        /// <summary>
        /// 객체의 좌표를 변경합니다.
        /// </summary>
        /// <param name="point"></param>
        private void Drag(Point point)
        {
            if (!IsDragging)
            {
                // 윈도우상의 처음 좌표값의 역변환
                FirstCoordinate = Element.TransformToVisual(Window).Inverse.Transform(FirstPoint);
                IsDragging = true;
            }

            // 객체를 기준으로 역변환 값을 윈도우상의 좌표값으로 변환합니다.
            Point Translate = Element.TranslatePoint(FirstCoordinate, Window);

            // 현재 좌표값과의 차이
            Point Difference = new Point(point.X - Translate.X, point.Y - Translate.Y);

            // 좌표를 변경합니다.
            TranslateTrans.X += Difference.X;
            TranslateTrans.Y += Difference.Y;

            Element.RenderTransform = TransGroup;
        }
        #endregion

        #region ChangeMousePosition
        /// <summary>
        /// 마우스의 포인터 점을 변경합니다.
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="point"></param>
        private void ChangePosition(MouseObject ob, Point point)
        {
            if (Adorner == null)
            {
                Adorner = new MouseAdorner((UIElement)Window.Content);

                FirstPosition.Key = 0;
                SecondPosition.Key = 1;
            }

            ob.point = point;
            Adorner.AddObject(ob);
        }
        #endregion

        #region MouseEvent
        /// <summary>
        /// 마우스 이벤트들
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Element_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 크기 변경이나 회전
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed && (CanBeScaled || CanBeRotated))
            {
                IsTransformation = !IsTransformation;

                Window.PreviewMouseMove -= DragEvent;

                if (IsTransformation)
                {
                    FirstPoint = e.GetPosition(Window);
                    Window.PreviewMouseMove += TransformationeEvent;
                }
            }
            // 드래그
            else
            {
                if (CanBeDragged && e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    if (!IsTransformation)
                    {
                        FirstPoint = e.GetPosition(Window);
                        Window.PreviewMouseMove += DragEvent;

                        OldZindex = Canvas.GetZIndex(Element);
                        Canvas.SetZIndex(Element, 20);
                    }
                }
            }

            // 포인터 위치 표시
            if (ShowPosition)
            {
                if (!IsTransformation)
                {
                    ChangePosition(FirstPosition, e.GetPosition(Window));
                }
            }
        }

        private void Element_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                if (!IsTransformation)
                {
                    Canvas.SetZIndex(Element, OldZindex);
                }

                if (CanBeDragged)
                {
                    Window.PreviewMouseMove -= DragEvent;
                    IsDragging = false;
                }

                if (CanBeScaled || CanBeRotated)
                {
                    if (!IsTransformation)
                    {
                        Window.PreviewMouseMove -= TransformationeEvent;
                    }
                    IsRotating = IsScaling = false;
                }

                if (ShowPosition)
                {
                    if (!IsTransformation)
                    {
                        Adorner.RemoveObject(FirstPosition.Key);
                        Adorner.RemoveObject(SecondPosition.Key);
                    }
                    else
                    {
                        Adorner.RemoveObject(SecondPosition.Key);
                    }
                }
            }
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                Window.PreviewMouseMove -= DragEvent;
                Window.PreviewMouseMove -= TransformationeEvent;
                Canvas.SetZIndex(Element, OldZindex);

                IsTransformation = IsDragging = IsRotating = IsScaling = false;

                if (ShowPosition)
                {
                    if (Adorner != null)
                    {
                        Adorner.RemoveObject(FirstPosition.Key);
                        Adorner.RemoveObject(SecondPosition.Key);
                    }
                }
            }
        }

        private void DragEvent(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                Point CurrentPoint = e.GetPosition(Window);

                // 포인터 위치 표시
                if (ShowPosition)
                {
                    ChangePosition(FirstPosition, CurrentPoint);
                }

                Drag(CurrentPoint);
            }
        }
        #endregion
    }
}