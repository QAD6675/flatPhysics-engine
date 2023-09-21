using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flat;
using FlatPhysics;
using Flat.Graphics;
using Flat.Input;

using System;

namespace FlatPhysX
{
    public class simulation3 : Game
    {
        private GraphicsDeviceManager _graphics;
        public Camera cam;
        private Screen screen;
        private Shapes shapes;
        private FlatKeyboard Kinput;
        private FlatMouse Minput;
        private Sprites sprites;


        public simulation3()
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
            cam.Zoom = 5;

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            Kinput = FlatKeyboard.Instance;
            Minput = FlatMouse.Instance;

            Kinput.Update();
            Minput.Update();

            if (Kinput.IsKeyAvailable)
            {
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
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            screen.Set();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            shapes.Begin(cam);
            shapes.End();

            this.screen.Unset();
            this.screen.Present(sprites);

            base.Draw(gameTime);
        }
    }
}