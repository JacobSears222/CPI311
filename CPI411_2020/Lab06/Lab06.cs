using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411_2020.SimpleEngine;

namespace Lab06
{
    public class Lab06 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Effect effect;
        Model model;
        Texture2D texture;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 20), Vector3.Zero, Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 1.33f, 0.1f, 100f);

        MouseState previousMouseState;
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        float angle, angle2 = 0.0f;

        Skybox skybox;
        string[] skyboxTextures;

        public Lab06()
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
            effect = Content.Load<Effect>("Reflection");      
            model = Content.Load<Model>("Helicopter");
            texture = Content.Load<Texture2D>("HelicopterTexture");
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle -= (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 -= (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            cameraPosition = Vector3.Transform(Vector3.Zero, Matrix.CreateTranslation(new Vector3(0, 0, 3)) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            previousMouseState = currentMouseState;

            base.Update(gameTime);
        }

        private void DrawModelWithEffect()
        {
            effect.CurrentTechnique = effect.Techniques[0];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["CameraPosition"].SetValue(this.cameraPosition);

                        Matrix worldInverseTranspose = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));

                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                        effect.Parameters["decalMap"].SetValue(texture);
                        effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);

                        pass.Apply();

                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer; 
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPosition);

            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;

            DrawModelWithEffect();

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "angle:" + angle, Vector2.UnitX + Vector2.UnitY * 12, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
