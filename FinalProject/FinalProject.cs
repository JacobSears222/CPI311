using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411_2020.SimpleEngine;

namespace FinalProject
{
    public class FinalProject : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Effect effect;
        Texture2D texture, filter;

        float offset = 0.0f;

        KeyboardState preKeyboard;

        bool showHelp = true;
        int technique = 0;
        string[] techniques =
        {
            "Red", "Green", "Blue"
        };

        public FinalProject()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            IsMouseVisible = true;
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

            font = Content.Load<SpriteFont>("Font"); 

            effect = Content.Load<Effect>("Post-Complete");
            effect.Parameters["modelTexture"].SetValue(texture);
            effect.Parameters["filterTexture"].SetValue(filter);
            effect.Parameters["imageWidth"].SetValue((float)texture.Width);
            effect.Parameters["imageHeight"].SetValue((float)texture.Height);
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, 960, 550, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(offset, offset, 0);
            effect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //Images
            if (Keyboard.GetState().IsKeyDown(Keys.F1)) texture = Content.Load<Texture2D>("RE3HQ");
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) texture = Content.Load<Texture2D>("Rainbow");
            if (Keyboard.GetState().IsKeyDown(Keys.F3)) texture = Content.Load<Texture2D>("Couple");
            if (Keyboard.GetState().IsKeyDown(Keys.F4)) texture = Content.Load<Texture2D>("Tree");
            if (Keyboard.GetState().IsKeyDown(Keys.F5)) texture = Content.Load<Texture2D>("Thor");
            if (Keyboard.GetState().IsKeyDown(Keys.F6)) texture = Content.Load<Texture2D>("Space");
            //if (Keyboard.GetState().IsKeyDown(Keys.F7)) texture = Content.Load<Texture2D>("Pups");
            if (Keyboard.GetState().IsKeyDown(Keys.F8)) texture = Content.Load<Texture2D>("Me2");

            // Shaders
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) technique = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) technique = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) technique = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) technique = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) technique = 4;
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) technique = 5;
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) technique = 6;
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) technique = 7;
            if (Keyboard.GetState().IsKeyDown(Keys.D9)) technique = 8;
            if (Keyboard.GetState().IsKeyDown(Keys.D0)) technique = 9;
            if (Keyboard.GetState().IsKeyDown(Keys.N)) technique = 10;
            if (Keyboard.GetState().IsKeyDown(Keys.R)) technique = 11;
            if (Keyboard.GetState().IsKeyDown(Keys.G)) technique = 12;
            if (Keyboard.GetState().IsKeyDown(Keys.B)) technique = 13;
            
            //All Filters
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) technique = 14;

            if (Keyboard.GetState().IsKeyDown(Keys.Up)) offset = MathHelper.Clamp(offset + 0.001f, 0.00f, 1.00f);
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) offset = MathHelper.Clamp(offset - 0.001f, 0.00f, 1.00f);

            if (Keyboard.GetState().IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) technique = 15;
            if (Keyboard.GetState().IsKeyDown(Keys.G) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) technique = 16;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) technique = 17;
            if (Keyboard.GetState().IsKeyDown(Keys.T) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) technique = 18;
            if (Keyboard.GetState().IsKeyDown(Keys.Y) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) technique = 19;
            if (Keyboard.GetState().IsKeyDown(Keys.U) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) technique = 20;

            effect.Parameters["offset"].SetValue(offset);

            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !preKeyboard.IsKeyDown(Keys.OemQuestion)) showHelp = !showHelp;               

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice gd = graphics.GraphicsDevice;
            gd.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(0, null, null, null, null, effect);
            spriteBatch.Draw(texture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);

            if (showHelp)
            {
                int i = 0;
                spriteBatch.DrawString(font, "F1 - F6 Keys to change image shown", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.White);
                spriteBatch.DrawString(font, "0 - 7 Keys to Change to Different Color combination filters", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                //8,9,0, and N are all negative shaders
                spriteBatch.DrawString(font, "8 - 0 and N Keys to Change to Different Shader / color filters", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "R key is a inverse R value", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "G key is a inverse G value", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "B key is a inverse B value", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "Up/Down arrows to +/- offset ", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "Space is to see all filters", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "Offset " + offset, Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "R + Shift key is a changing offset R value", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "G + Shift key is a changing offset G value", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "B + Shift key is a changing offset B value", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "T + Shift key is Like GPU Gems Modified Ramp Red", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "Y + Shift key is Gems Modified Ramp Green", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "U + Shift key is Gems Modified Ramp Red", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);
                spriteBatch.DrawString(font, "? to toggle Help", Vector2.UnitX * 10 + Vector2.UnitY * 25 * (i++), Color.LightGreen);


            }
            spriteBatch.End();
            effect.CurrentTechnique = effect.Techniques[technique];

            
            base.Draw(gameTime);
        }
    }
}
