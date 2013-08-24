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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using System.Windows;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;

namespace TouchFramework
{
    /// <summary>
    /// Wraps any FrameworkElement object with a controlling interface which stores touch information and 
    /// processes actions based on the touches present.
    /// </summary>
    public class MTSmoothContainer : MTContainer, IDisposable
    {
        object sync = new object();

        public LinearFilter2d TranslateFilter = new LinearFilter2d();
        public LinearFilter2d CenterFilter = new LinearFilter2d();
        public LinearFilter RotateFilter = new LinearFilter();
        public LinearFilter ScaleFilter = new LinearFilter();
        public LinearFilter2d DampingFilter = new LinearFilter2d();
        public LinearFilter AngularDampingFilter = new LinearFilter();

        public bool centerInit = false;
        public bool IsRotating = false;
        public bool IsScrollView = false;

        public string userIP = "";

        private double FirstAngle;
        RotateTransform rotateTrans = new RotateTransform();

        delegate void InvokeDelegate();

        /// <summary>
        /// Constructor for MTElementC                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        ontainer.
        /// </summary>
        /// <param name="createFrom">The FrameworkElement this container is going to store touches for and manipulate.</param>
        public MTSmoothContainer(FrameworkElement createFrom, Panel cont, ElementProperties props)
            : base(createFrom, cont, props)
        {
            this.ScaleFilter.Reset(1.0f, 1.0f);

            this.Delay = 100;
            this.DampingDelay = 1200;            
        }

        public override void Reset()
        {
            centerInit = false;
            base.Reset();
        }

        /// <summary>
        /// Delay in Milliseconds which handles the length of time the filters run for.
        /// A lower delay values makes movement faster, a higher value slows everything down.
        /// The default of 100 is usually fine here.
        /// </summary>
        public int Delay
        {
            set
            {
                this.TranslateFilter.Delay = value;
                this.RotateFilter.Delay = value;
                this.ScaleFilter.Delay = value;
                this.CenterFilter.Delay = value;
            }
        }

        /// <summary>
        /// Delay in Milliseconds which handles the length of time the damping filters run for.
        /// Damping filters are used for the inertia functionality.  A lower value results in the element
        /// moving a shorter distance when flicked, a higher value increases the distance.
        /// </summary>
        public int DampingDelay
        {
            set
            {
                this.DampingFilter.Delay = value;
                this.AngularDampingFilter.Delay = value;
            }
        }

        /// <summary>
        /// Performs a rendertransform applying the scale to the working object.
        /// </summary>
        /// <param name="scaleFactor">Value to multiply the object's width and height by</param>
        /// <param name="centerPoint">Point in screen space for the center of the scale operation</param>
        public override void Scale(float scaleFactor, PointF centerPoint)
        {
            if (!Supports(TouchAction.Resize)) return;
            SetCenterTarget(centerPoint);
            if (scaleFactor != 1.0f && scaleFactor != 0.0f)
            {
                this.ScaleFilter.Target *= scaleFactor;
            }
        }

        /// <summary>
        /// Performs a rendertransform moving the object from it's current position (after all previous tranforms).
        /// </summary>
        /// <param name="offsetX">Number of pixels to move the object on the x axis.</param>
        /// <param name="offsetY">Number of pixels to move the object on the y axis.</param>
        public override void Move(float offsetX, float offsetY)
        {
            if (!Supports(TouchAction.Move)) return;
            if (IsScrollView == true) return;
            
            PointF target = TranslateFilter.Target;
            target.X += offsetX;
            target.Y += offsetY;
            TranslateFilter.Target = target;

            PointF cen = CenterFilter.Target;
            cen.X += offsetX;
            cen.Y += offsetY;
            CenterFilter.Target = cen;
        }

        /// <summary>
        /// 지정된 좌표로 물체를 이동시킵니다.
        /// </summary>
        /// <param name="offsetX">Number of pixels to move the object on the x axis.</param>
        /// <param name="offsetY">Number of pixels to move the object on the y axis.</param>
        public override void SetPosition(float offsetX, float offsetY, double angle, double scale)
        {
            RotateTransform objRotateTrans = new RotateTransform();
            TranslateTransform objTranslateTrans = new TranslateTransform();
            ScaleTransform objScaleTrans = new ScaleTransform();
            TransformGroup objTransGroup = new TransformGroup();

            objTransGroup.Children.Add(objRotateTrans);
            objTransGroup.Children.Add(objScaleTrans);
            objTransGroup.Children.Add(objTranslateTrans);

            objTranslateTrans.X = offsetX - (WorkingObject.Width / 2);
            objTranslateTrans.Y = offsetY - (WorkingObject.Height / 2);

            objRotateTrans.Angle = angle;
            objScaleTrans.ScaleX = scale;
            objScaleTrans.ScaleY = scale;

            transforms = new TransformGroup();
            transforms = objTransGroup;

            //WorkingObject.RenderTransformOrigin = new System.Windows.Point(0.867, 0.818);
            WorkingObject.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            WorkingObject.RenderTransform = transforms;

            // Store the current transform ready for use in the next frame
            oldTranform = WorkingObject.RenderTransform.Value;

            // Reset our transform group (each time we will fill it with new tranforms)
            clearTransformsNext = true;
        }

        /// <summary>
        /// Performs a rendertransform rotating the object around the center point provided from it's current rotation (after all previous transforms).
        /// </summary>
        /// <param name="angle">The angle to rotate by.</param>
        /// <param name="centerPoint">Point in screen space for the center of the scale operation.</param>
        public override void Rotate(float angle, PointF centerPoint)
        {
            if (!Supports(TouchAction.Rotate)) return;
            SetCenterTarget(centerPoint);
            //Resetting the center filter every time to the current center may or may not be correct, Need to test on table.
            //this.CenterFilter.Reset(centerPoint, centerPoint);
            if (angle < 170 && angle > -170)
            {
                this.RotateFilter.Target += angle;
            }
        }

        /// <summary>
        /// 파일팍스 전용 회전
        /// </summary>
        /// <param name="angle">The angle to rotate by.</param>
        public override void RotateFileBox(double angle, PointF globalPt, PointF CenterPoint)
        {

//             PointF DistanceVector = new PointF((globalPt.X - CenterPoint.X), (CenterPoint.Y - globalPt.Y));
// 
//             double CurrentAngle = Math.Atan2(DistanceVector.Y, DistanceVector.X) * (180 / Math.PI);
// 
//             if (!IsRotating)
//             {
//                 // 처음 각도
//                 FirstAngle = CurrentAngle + rotateTrans.Angle;
//                 IsRotating = true;
//             }
//             // 현재 각도
//             double AngleDiff = (FirstAngle - CurrentAngle + 360) % 360;
//             rotateTrans.Angle = AngleDiff;
// 
//             WorkingObject.RenderTransformOrigin = new System.Windows.Point(0.867, 0.818);
//             Console.WriteLine(WorkingObject.RenderTransformOrigin);
// 
//             WorkingObject.RenderTransform = rotateTrans;
// 
//             // Store the current transform ready for use in the next frame
//             oldTranform = WorkingObject.RenderTransform.Value;
// 
//             // Reset our transform group (each time we will fill it with new tranforms)
//             clearTransformsNext = true;
        }

        /// <summary>
        /// Combines a scale, rotate and move operation for simplicity
        /// </summary>
        /// <param name="angle">The angle to rotate by.</param>
        /// <param name="scaleFactor">Value to multiply the object's width and height by.</param>
        /// <param name="offsetX">Number of pixels to move the object on the x axis.</param>
        /// <param name="offsetY">Number of pixels to move the object on the y axis.</param>
        /// <param name="centerPoint">Point in screen space for the center of the scale/rotate operation.</param>
        public override void ScaleRotateMove(float angle, float scaleFactor, float offsetX, float offsetY, PointF centerPoint)
        {
            if (IsScrollView == true) return;
            
            PointF target = TranslateFilter.Target;
            target.X += offsetX;
            target.Y += offsetY;
            TranslateFilter.Target = target;
            SetCenterTarget(centerPoint);
        
            if (scaleFactor != 1.0f && scaleFactor != 0.0f)
            {
                this.ScaleFilter.Target *= scaleFactor;
            }

            if (angle < 170 && angle > -170)
            {
                this.RotateFilter.Target += angle;
            }
        }

        void smoothActions()
        {
            lock (sync)
            {
                if (Supports(TouchAction.Move)) this.TranslateFilter.Step();
                if (Supports(TouchAction.Rotate)) this.RotateFilter.Step();
                if (Supports(TouchAction.Resize)) this.ScaleFilter.Step();
                if (Supports(TouchAction.Move) || Supports(TouchAction.Resize)) this.CenterFilter.Step();

                // If we've just been touched, stop all damping
                // NOTE: You can disable this to test spinning move.
                if (this.ObjectTouches.JustTouched) { this.DampingFilter.Stop(); }
                if (this.ObjectTouches.TwoOrMoreTouch) { this.AngularDampingFilter.Stop(); }
                
                // If we support flicking then dampen the movement
                if (Supports(TouchAction.Flick))
                {
                    if (!this.DampingFilter.IsFiltering && this.ObjectTouches.Lifted)
                    {
                        PointF point = this.TranslateFilter.LastVelocityFromSet;
                        this.DampingFilter.Reset(point, new PointF(0, 0));
                    }
                    this.DampingFilter.StepIfFiltering();
                }
                if (Supports(TouchAction.Spin))
                {
                    if (!this.AngularDampingFilter.IsFiltering && this.ObjectTouches.OneOrMoreLifted)
                    {
                        float angle = this.RotateFilter.LastVelocityFromSet;
                        this.AngularDampingFilter.Reset(angle, 0);
                    }
                    this.AngularDampingFilter.StepIfFiltering();
                }

                WorkingObject.Dispatcher.BeginInvoke((InvokeDelegate)delegate() { this.updatePosition(); });
            }
        }

        public void updatePosition()
        {
            PointF centerPoint = this.CenterFilter.Position;

            double dCenX = centerPoint.X - StartX;
            double dCenY = centerPoint.Y - StartY;
            
            float scaleFactor = 1.0f;
            scaleFactor /= ScaleFilter.PreviousPosition;
            scaleFactor *= ScaleFilter.Position;
                        
            ScaleTransform st = new ScaleTransform(scaleFactor, scaleFactor, dCenX, dCenY);
            TranslateTransform tt = new TranslateTransform(this.TranslateFilter.Velocity.X, this.TranslateFilter.Velocity.Y);
            RotateTransform rt = new RotateTransform(RotateFilter.Velocity, dCenX, dCenY);

            double dPosX = 0, dPosY = 0;
            float dRotPos = 0f;
            
            if (this.DampingFilter.IsFiltering)
            {
                dPosX = this.DampingFilter.Position.X;
                dPosY = this.DampingFilter.Position.Y;

                dCenX = (centerPoint.X + this.DampingFilter.CumulativePosition.X) - StartX;
                dCenY = (centerPoint.Y + this.DampingFilter.CumulativePosition.Y) - StartY;

                // If we are supposed to check the bounds with the container, check using the corners
                if (this.Supports(TouchAction.BoundsCheck))
                {
                    var points = getCorners();
                    if (checkPointEdge(points)) this.DampingFilter.Stop();
                }
            }

            if (this.AngularDampingFilter.IsFiltering)
            {
                dRotPos = this.AngularDampingFilter.Position;
            }

            TranslateTransform dt = new TranslateTransform(dPosX, dPosY);
            RotateTransform drt = new RotateTransform(dRotPos, dCenX, dCenY);           
            addTransforms(st, tt, dt, rt, drt);

            this.ApplyTransforms();
        }

        void SetCenterTarget(PointF target)
        {
            CheckInitCentre();
            CenterFilter.Target = target;
        }

        void CheckInitCentre()
        {
            if (centerInit) return;
            PointF cen = this.GetElementCenter();
            this.CenterFilter.Reset(cen, cen);
            centerInit = true;
        }

        protected override void  Cleanup()
        {
        }

        public override void Tick()
        {
            smoothActions();
            base.Tick();
        }
    }
}

