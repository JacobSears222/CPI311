using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab04
{   
    public class Lab04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Model model;
        //, model2, model3, model4, model5;
        Effect effect;

        Matrix world, view, projection;
          
        Vector3 cameraOffset = new Vector3(0, 0, 0);
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        Vector3 lightPosition = new Vector3(0.5f, 1, 1);

        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.9f;
        float diffuseIntensity = 0.6f;
        float specularIntensity = 0.9f;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 diffuseLightDirection = new Vector3(1, 1, 1);
        Vector3 lightDirection = new Vector3(0.5f, 0.6f, 0.4f);

        float shininess = 7.0f;

        float angle, angle2 = 0.0f;
        float distance = 10;
        int toggleTechnique = 0;        

        MouseState previousMouseState, currentMouseState;

        public Lab04()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //For Shader file use
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
                
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Torus");
            //model2 = Content.Load<Model>("bunny");
            //model3 = Content.Load<Model>("box");
            //model4 = Content.Load<Model>("Teapot");
            //model5 = Content.Load<Model>("Sphere");
            effect = Content.Load<Effect>("Shader");
            font = Content.Load<SpriteFont>("Font");

            world = Matrix.Identity;
            view = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);
        }

        protected override void UnloadContent() { }
               
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                shininess -= 0.05f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                shininess += 0.05f;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                float offsetx = 0.01f * (Mouse.GetState().X - previousMouseState.X);
                float offsety = 0.01f * (Mouse.GetState().Y - previousMouseState.Y);
                angle += offsetx;
                angle2 += offsety;
            }

            /*if (Mouse.GetState().RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                distance += (previousMouseState.X - currentMouseState.X);
                distance -= (previousMouseState.Y - currentMouseState.Y);
                cameraPosition = new Vector3(0, 0, distance);

            }
            */

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                toggleTechnique = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                toggleTechnique = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                toggleTechnique = 2;
            }            

            Vector3 camera = Vector3.Transform(new Vector3(0, 0, 20), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);

            previousMouseState = Mouse.GetState();                                  

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            effect.CurrentTechnique = effect.Techniques[toggleTechnique];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["LightPosition"].SetValue(lightDirection);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["DiffuseLightDirection"].SetValue(diffuseLightDirection);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);

                        pass.Apply();
                        Matrix worldInverseTranspose = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);

                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;

                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "angle:" + angle, Vector2.UnitX + Vector2.UnitY * 12, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
