using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flat;
using FlatPhysics;
using Flat.Graphics;
using Flat.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace FlatPhysX
{
    public class simulation2 : Game
    {
        #region feilds
        private GraphicsDeviceManager _graphics;

        public Camera cam;
        private Screen screen;
        private Shapes shapes;
        private Sprites sprites;
        private SpriteFont consolas18;

        private string bodyCountstr = string.Empty;
        private int totalSampleCount = 0;
        private double totalWorldStepTime = 0;
        private int totalBodies = 0;
        private string worldStepTimeStr = string.Empty;

        private FlatKeyboard Kinput;
        private FlatMouse Minput;

        private FlatWorld world = new(9.81f);

        private Stopwatch sampleTimer = new();

        private Stopwatch watch = new();

        private List<Color> colors;
        private List<Color> outlines;

        #endregion feilds


        public simulation2()
        {
            this._graphics = new GraphicsDeviceManager(this);
            this._graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.IsFixedTimeStep = true;

            const double fps = 60d;
            this.TargetElapsedTime = TimeSpan.FromTicks((long)Math.Round((double)TimeSpan.TicksPerSecond / fps));
        }

        protected override void Initialize()
        {

            FlatUtil.SetRelativeBackBufferSize(this._graphics, 0.85f);
            shapes = new Shapes(this);
            screen = new Screen(this, 1280, 768);
            cam = new Camera(screen);
            sprites = new Sprites(this);
            cam.Zoom = 20;

            this.cam.GetExtents(out float left, out float right, out float bottom, out float top);

            float padding = MathF.Abs(right - left) * 0.10f;

            this.colors = new List<Color>();
            this.outlines = new List<Color>();

            if (!FlatBody.CreateBoxBody(20f, 2f, 1f, true, 0.5f, out FlatBody ledge1, out string err))
            {
                throw new Exception(err);
            }
            ledge1.RotateTo(-2 * MathF.PI / 20);
            ledge1.MoveTo(new FlatVector(-10, 10));
            this.world.AddBody(ledge1);
            colors.Add(Color.Blue);
            outlines.Add(Color.White);

            if (!FlatBody.CreateBoxBody(15, 2f, 1f, true, 0.5f, out FlatBody ledge2, out string err1))
            {
                throw new Exception(err1);
            }
            ledge2.RotateTo(2 * MathF.PI / 20);
            ledge2.MoveTo(new FlatVector(10, 10));
            this.world.AddBody(ledge2);
            colors.Add(Color.Red);
            outlines.Add(Color.White);

            if (!FlatBody.CreateBoxBody(right - left - padding * 2, 3f, 1f, true, 0.5f, out FlatBody ground, out string err2))
            {
                throw new Exception(err2);
            }
            this.world.AddBody(ground);
            colors.Add(Color.DarkGreen);
            outlines.Add(Color.White);



            watch = new Stopwatch();
            sampleTimer.Start();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            consolas18 = Content.Load<SpriteFont>("Consolas 18");
        }


        protected override void Update(GameTime gameTime)
        {
            Kinput = FlatKeyboard.Instance;
            Minput = FlatMouse.Instance;

            Kinput.Update();
            Minput.Update();
            //get input
            if (Minput.IsLeftMouseButtonPressed())
            {
                float width = RandomHelper.RandomSingle(2f, 3f);
                float height = RandomHelper.RandomSingle(2f, 3f);

                FlatVector mousePos = FlatConv.Mono2flat(Minput.GetMouseWorldPosition(this, screen, cam));

                if (!FlatBody.CreateBoxBody(width, height, 2f, false, 0.6f, out FlatBody body, out string err))
                {
                    throw new Exception(err);
                }
                body.MoveTo(mousePos);
                this.world.AddBody(body);
                colors.Add(RandomHelper.RandomColor());
                outlines.Add(Color.White);

            }
            if (Minput.IsRightMouseButtonPressed())
            {
                float rad = RandomHelper.RandomSingle(1f, 1.25f);

                FlatVector mousePos = FlatConv.Mono2flat(Minput.GetMouseWorldPosition(this, screen, cam));

                if (!FlatBody.CreateCircleBody(rad, 2f, false, 0.6f, out FlatBody body, out string err))
                {
                    throw new Exception(err);
                }
                body.MoveTo(mousePos);
                this.world.AddBody(body);
                colors.Add(RandomHelper.RandomColor());
                outlines.Add(Color.White);
            }

            if (Kinput.IsKeyAvailable)
            {
                if (Kinput.IsKeyClicked(Keys.OemTilde))
                {
                    Console.WriteLine($"BodyCount : {world.BodyCount}");
                    Console.WriteLine($"watch.Elapsed.TotalMillisecond : {Math.Round(watch.Elapsed.TotalMilliseconds, 4)}");
                    Console.WriteLine($"TransformCount : {FlatWorld.TransformCount}");
                    Console.WriteLine($"NoTransformCount : {FlatWorld.NoTransformCount}");
                    Console.WriteLine();
                }
                if (Kinput.IsKeyClicked(Keys.Escape))
                {
                    this.Exit();
                }
                if (Kinput.IsKeyDown(Keys.A))
                {
                    cam.IncZoom();
                }
                if (Kinput.IsKeyDown(Keys.Z))
                {
                    cam.DecZoom();
                }
            }

            //log stats
            if (sampleTimer.Elapsed.TotalSeconds > 1d)
            {
                bodyCountstr = "body count :" + Math.Round(this.totalBodies / (double)this.totalSampleCount, 4).ToString();
                worldStepTimeStr = "world step time :" + Math.Round(this.totalWorldStepTime / (double)this.totalSampleCount, 4).ToString();

                totalBodies = 0;
                totalWorldStepTime = 0;
                totalSampleCount = 0;
                sampleTimer.Restart();
            }

            FlatWorld.TransformCount = 0;
            FlatWorld.NoTransformCount = 0;

            this.watch.Restart();
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 30);
            watch.Stop();

            totalWorldStepTime += watch.Elapsed.TotalMilliseconds;
            totalBodies += world.BodyCount;
            totalSampleCount++;

            cam.GetExtents(out _, out _, out float buttom, out _);
            for (int i = 0; i < world.BodyCount; i++)
            {
                if (!world.GetBody(i, out FlatBody body))
                {
                    throw new ArgumentOutOfRangeException($"no body found at index{i}");
                }
                if (body.IsStatic)
                {
                    continue;
                }
                FlatAABB box = body.GetAABB();
                if (box.Max.Y < buttom)
                {
                    this.world.RemoveBody(body);
                    colors.RemoveAt(i);
                    outlines.RemoveAt(i);
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            screen.Set();
            GraphicsDevice.Clear(Color.CornflowerBlue);



            shapes.Begin(cam);
            for (int i = 0; i < world.BodyCount; i++)
            {
                if (!world.GetBody(i, out FlatBody body))
                {
                    throw new Exception("could not find a body at the specified index");
                }
                Vector2 pos = FlatConv.Flat2mono(body.Position);

                if (body.ShapeType is ShapeType.Circle)
                {
                    FlatCircle c = new(pos, body.Radius);
                    shapes.DrawCircleFill(c, 26, colors[i]);
                    shapes.DrawCircle(c, 26, outlines[i]);
                }
                else if (body.ShapeType is ShapeType.Box)
                {
                    shapes.DrawBoxFill(pos, body.Width, body.Height, body.Angle, colors[i]);
                    shapes.DrawBox(pos, body.Width, body.Height, body.Angle, outlines[i]);
                }
            }
            shapes.End();

            Vector2 strsize = consolas18.MeasureString(bodyCountstr);
            sprites.Begin();
            sprites.DrawString(consolas18, bodyCountstr, new Vector2(0, 0), Color.Black);
            sprites.DrawString(consolas18, worldStepTimeStr, new Vector2(0, strsize.Y), Color.Black);
            sprites.End();

            this.screen.Unset();
            this.screen.Present(sprites);

            base.Draw(gameTime);
        }
    }
}