using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411_2020.SimpleEngine;

namespace Lab08IP
{
    public class Lab08IP : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect effect;
        Texture2D texture, filter;

        public Lab08IP()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            ScreenManager.Initialize(graphics);

        }
        protected override void Initialize()
        {
            ScreenManager.Initialize(graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.Setup(true, 1920, 1080);

            texture = Content.Load<Texture2D>("RE3HQ");
            filter = Content.Load<Texture2D>("filter");

            effect = Content.Load<Effect>("Post-Complete");
            effect.CurrentTechnique = effect.Techniques["MyShader"];
            effect.Parameters["modelTexture"].SetValue(texture);
            effect.Parameters["filterTexture"].SetValue(filter);
            effect.Parameters["imageWidth"].SetValue((float)texture.Width);
            effect.Parameters["imageHeight"].SetValue((float)texture.Height);
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, 960, 550, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            effect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice gd = graphics.GraphicsDevice;
            gd.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(0, null, null, null, null, effect);
            spriteBatch.Draw(texture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
