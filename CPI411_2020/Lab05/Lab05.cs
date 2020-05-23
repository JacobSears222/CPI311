using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411_2020.SimpleEngine;

namespace Lab05
{   
    public class Lab05 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Effect effect;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 1.33f, 0.1f, 100f);

        MouseState previousMouseState;
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        float angle, angle2 = 0.0f;
        float distance = 20;

        Skybox skybox;
        string[] skyboxTextures;

        public Lab05()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
       
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            string[] skyboxTextures =
            {
                "skybox/SunsetPNG2", "skybox/SunsetPNG1",
                "skybox/SunsetPNG4","skybox/SunsetPNG3",
                "skybox/SunsetPNG6","skybox/SunsetPNG5"
            };

            skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            effect = Content.Load<Effect>("skybox/Skybox");
        }

        protected override void UnloadContent() { }
       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle -= 0.03f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angle += 0.03f;
            }

            cameraPosition = distance * new Vector3((float)System.Math.Sin(angle), 0, (float)System.Math.Cos(angle));
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.UnitY);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["CameraPosition"].SetValue(cameraPosition);

            base.Update(gameTime);
        }
      
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPosition);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "angle:" + angle, Vector2.UnitX + Vector2.UnitY * 12, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
