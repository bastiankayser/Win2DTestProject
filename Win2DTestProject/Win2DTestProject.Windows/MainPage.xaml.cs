using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Numerics;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Matrix3x2 = System.Numerics.Matrix3x2;
using Vector2 = System.Numerics.Vector2;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2DTestProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private bool isTankSelected = false;

        private Vector2 maxSize = new Vector2(1000, 900);
        private Vector2 minSize = new Vector2(100, 100);


        //private Transform2DEffect rotationEffect = new Transform2DEffect();
        //private Transform2DEffect translationEffect = new Transform2DEffect();
        private Projectile projectilePrototype = new Projectile();
        private List<Tank> Tanks = new List<Tank>();
        private List<Projectile> Projectiles = new List<Projectile>();
        public Color[] FractionColors = { Colors.Red, Colors.Aqua, Colors.White };
        public Vector2 FiringTargetPos { get; set; }
        public bool FireCommand { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void CanvasControl_OnDraw(ICanvasAnimatedControl canvasAnimatedControl, CanvasAnimatedDrawEventArgs args)
        {

            //if (startPointSet && endPointSet)
            //{
            //    args.DrawingSession.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), Colors.Red);

            //}
            //args.DrawingSession.DrawImage(translationEffect);
            foreach (var tank in Tanks)
            {
                args.DrawingSession.DrawImage(tank.translationEffect);
                if (tank.selected)
                {
                    args.DrawingSession.DrawCircle(tank.CurrentPosition, tank.size + 5.0f, Colors.Crimson);

                    //debug info
                    args.DrawingSession.DrawText(tank.CurrentPosition.ToString(), 100, 100, Colors.Red);

                    args.DrawingSession.DrawText("TargetPos:" + tank.TargetPosition.ToString(), 100, 150, Colors.Red);
                    args.DrawingSession.DrawText("FiringTarget:" + tank.FiringTarget.ToString(), 100, 200, Colors.Red);

                    args.DrawingSession.DrawText(tank.desiredRot.ToString(), 100, 250, Colors.Red);
                    args.DrawingSession.DrawText(tank.currentRot.ToString(), 100, 300, Colors.Red);
                    args.DrawingSession.DrawText((tank.currentRot - tank.desiredRot).ToString(), 100, 350, Colors.Red);

                }
            }

            foreach (var projectile in Projectiles)
            {
                args.DrawingSession.DrawCircle(projectile.pos.X, projectile.pos.Y, 5.0f, new CanvasSolidColorBrush(canvasAnimatedControl, Colors.Yellow));
                args.DrawingSession.DrawCircle(projectile.Target, 15.0f, Colors.Crimson, 5.0f);
            }


        }
        private void CanvasControl_OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {

            //for each tank, create translation and rotation matrix
            foreach (var tank in Tanks)
            {
                if (tank.selected && this.FireCommand && !tank.firing)
                {
                    //calc fire direction and rotate, mark the tank as firing
                    tank.firing = true;
                    //tank.direction = Vector2.Normalize(FiringTargetPos - tank.CurrentPosition);
                    tank.FiringTarget = FiringTargetPos;
                    //tank.desiredRot = (float)Math.Atan2(tank.FiringTarget.Y, tank.FiringTarget.X);
                    FireCommand = false;
                }
                else if (!tank.firing)
                {
                    tank.desiredRot = (float)Math.Atan2(tank.direction.Y, tank.direction.X);

                }

                //update rotation until done
                if (Math.Abs(tank.desiredRot - tank.currentRot) > 0.01f)
                {
                    tank.rotating = true;
                    //var rot = Math.Atan2(Math.Sin(tank.desiredRot - tank.currentRot), Math.Cos(tank.desiredRot - tank.currentRot));
                    tank.currentRot = tank.currentRot - 0.05f * (tank.currentRot - tank.desiredRot);
                    //tank.currentRot += (float)rot * 0.05f;
                }
                else
                {
                    tank.rotating = false;
                    //update position if not already on target
                    if (Vector2.Distance(tank.CurrentPosition, tank.TargetPosition) > 1)
                    {
                        tank.CurrentPosition = tank.CurrentPosition + tank.direction;
                    }
                    else
                    {
                        tank.CurrentPosition.X = tank.TargetPosition.X;
                        tank.CurrentPosition.Y = tank.TargetPosition.Y;

                    }
                    if (tank.CurrentPosition.X < minSize.X || tank.CurrentPosition.X > maxSize.X)
                    {
                        tank.direction.X *= -1;
                    }
                    if (tank.CurrentPosition.Y < minSize.Y || tank.CurrentPosition.Y > maxSize.Y)
                    {
                        tank.direction.Y *= -1;
                    }


                }
                if (tank.firing && !tank.rotating)
                {
                    tank.firing = false;
                    //create and fire projectile
                    var projectile = new Projectile();
                    projectile.pos = new Vector2(tank.CurrentPosition.X, tank.CurrentPosition.Y);
                    projectile.Target = new Vector2(tank.FiringTarget.X, tank.FiringTarget.Y);
                    this.Projectiles.Add(projectile);
                }

                var translationMatrix = Matrix3x2.CreateTranslation(tank.CurrentPosition);
                tank.translationEffect.TransformMatrix = translationMatrix;
                var rotationMatrix = Matrix3x2.CreateRotation(tank.currentRot);
                tank.rotationEffect.TransformMatrix = rotationMatrix;
            }

            List<Projectile> toRemove = new List<Projectile>();
            foreach (var projectile in Projectiles)
            {
                if (projectile.pos.X > maxSize.X || projectile.pos.X < minSize.X || projectile.pos.Y > maxSize.Y || projectile.pos.Y < minSize.Y)
                {
                    toRemove.Add(projectile);
                    continue;
                }
                if (Vector2.Distance(projectile.pos, projectile.Target) < 1f)
                {
                    toRemove.Add(projectile);
                    continue;
                }
                projectile.pos = projectile.pos + projectile.direction * 3.0f;

            }

            foreach (var projectile in toRemove)
            {
                Projectiles.Remove(projectile);
            }

        }

        private void MainPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.CanvasControl.RemoveFromVisualTree();
            this.CanvasControl = null;
        }

        private void CanvasControl_OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            //init tanks
            InitTanks();
            foreach (var tank in Tanks)
            {

                CanvasCommandList ccl = new CanvasCommandList(sender);
                using (CanvasDrawingSession cds = ccl.CreateDrawingSession())
                {
                    //draw each tank

                    //tank base
                    cds.DrawRectangle(new Rect(-tank.size / 2, -tank.size / 2, tank.size * 2, tank.size), FractionColors[tank.fraction]);
                    //cannon
                    cds.DrawRectangle(new Rect(-tank.size / 2, -tank.size / 2 + (tank.size / 1.5f), tank.size * 3, tank.size / 2), FractionColors[tank.fraction]);
                    //"wheels"

                }
                //set the effect source
                tank.rotationEffect.Source = ccl;
                tank.translationEffect.Source = tank.rotationEffect;
            }

            CanvasCommandList pccl = new CanvasCommandList(sender);
            using (CanvasDrawingSession pds = pccl.CreateDrawingSession())
            {
                pds.DrawCircle(0, 0, 5.0f, new CanvasSolidColorBrush(pds, Colors.Yellow));
            }
            projectilePrototype.translationEffect = new Transform2DEffect();
            projectilePrototype.translationEffect.Source = pccl;

        }


        private void InitTanks()
        {
            // create 10 for each fraktion tanks with random init positions and rotations
            Random r = new Random();
            for (int i = 0; i < 30; i++)
            {
                var tank = new Tank();
                Vector2 startPos = new Vector2((float)r.NextDouble() * (maxSize.X - minSize.X) + minSize.X, (float)r.NextDouble() * (maxSize.Y - maxSize.Y) + minSize.Y);
                tank.CurrentPosition = startPos;
                tank.direction = Vector2.Normalize(new Vector2((float)r.NextDouble() - .5f, (float)r.NextDouble() - 0.5f));
                tank.currentRot = 0.0f;//(float)Math.Asin(tank.direction.X);
                tank.desiredRot = 0.0f;
                tank.damage = 0f;
                tank.fraction = i % 3;
                tank.size = 20f;
                tank.destroyed = false;
                tank.selected = false;
                tank.hit = false;
                tank.translationEffect = new Transform2DEffect();
                tank.rotationEffect = new Transform2DEffect();
                Tanks.Add(tank);
            }


        }

        private void CanvasControl_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var position = e.GetCurrentPoint(CanvasControl).Position;
            var properties = e.GetCurrentPoint(CanvasControl).Properties;
            var isLeft = properties.IsLeftButtonPressed;
            var isRight = properties.IsRightButtonPressed;

            var action = CanvasControl.RunOnGameLoopThreadAsync(() =>
            {
                HandlePointerPressed(position, isLeft, isRight);
            });
        }

        private void HandlePointerPressed(Point position, bool isLeft, bool isRight)
        {

            var pointerPos = position.ToVector2();
            if (isLeft)
            {
                bool newSelection = false;
                Tank tempTank = null;
                //check if something get selected
                foreach (var tank in Tanks)
                {

                    if (tank.selected)
                    {
                        tempTank = tank;
                    }
                    if (!newSelection && Vector2.Distance(pointerPos, tank.CurrentPosition) < tank.size)
                    {
                        tank.selected = true;
                        isTankSelected = true;
                        newSelection = true;

                    }
                    else
                    {
                        tank.selected = false;
                    }

                }
                if (!newSelection && tempTank != null)
                {
                    tempTank.TargetPosition = pointerPos;
                    tempTank.selected = true;
                }
            }
            else if (isRight)
            {
                this.FiringTargetPos = pointerPos;
                this.FireCommand = true;
            }

        }




        private void CanvasControl_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {



        }

        private void CanvasControl_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {

        }


        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            //bind to keyboard events
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            var pressedKey = args.VirtualKey;
            var pressedArrow = "";
            switch (pressedKey)
            {
                case VirtualKey.Space:
                    pressedArrow = "Fire"; break;
                case VirtualKey.GoHome:
                    pressedArrow = "selfdestruct"; break;


            }
            if (String.IsNullOrEmpty(pressedArrow))
                return;

            args.Handled = true;
            var action = CanvasControl.RunOnGameLoopThreadAsync(() => this.ProcessArrowKeyPressed(pressedArrow));


        }

        private void ProcessArrowKeyPressed(string pressedArrow)
        {
            if (pressedArrow.Equals("Fire"))
            {

            }
            else if (pressedArrow.Equals("selfdestruct"))
            {

            }
        }
    }
}
