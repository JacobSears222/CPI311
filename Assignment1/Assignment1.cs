using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment1
{   
    public class Assignment1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model;

        Matrix world, view, projection;
        Effect effect;

        Vector3 cameraOffset = new Vector3(0, 0, 0);
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        Vector3 lightPosition = new Vector3(1f, 1, 1);

        float lightAngle, lightAngle2 = 0;
        float angle, angle2 = 0;
        float distance = 0f;
        float midMouseX, midMouseY = 0f;
        int toggleTechnique = 0;
        bool boolHelp = false;
        bool boolInfo = false;
        string shaderType = "Gauraud";

        //object materials
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.9f;

        Vector3 lightDirection = new Vector3(0.5f, 0.6f, 0.4f);

        float specularIntensity = 0.9f;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 diffuseLightDirection = new Vector3(1, 1, 1);
        float diffuseIntensity = 0.6f;

        float shininess = 7.0f;

        SpriteFont font;

        //Mouse Event
        MouseState previousMouseState;
        //int previousScrollValue;

        public Assignment1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }
           
        protected override void Initialize()
        {           
            //previousScrollValue = previousMouseState.ScrollWheelValue;
            base.Initialize();
        }
               
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Box");
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);
            effect = Content.Load<Effect>("Shader");
            font = Content.Load<SpriteFont>("Font");
        }
               
        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Part A - Basic User Interface
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {

                angle += 0.01f * (Mouse.GetState().X - previousMouseState.X);
                angle2 += 0.01f * (Mouse.GetState().Y - previousMouseState.Y);
                Vector3 camera = Vector3.Transform(new Vector3(0, 0, 5), Matrix.CreateTranslation(0, 0, distance) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                distance += 0.1f * (Mouse.GetState().Y - previousMouseState.Y);
                Vector3 camera = Vector3.Transform(new Vector3(0, 0, 5), Matrix.CreateTranslation(0, 0, distance) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);

            }

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                //cameraPosition += new Vector3(0, -1, 0);
                //view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
                midMouseX = 0.1f * (Mouse.GetState().X - previousMouseState.X);
                midMouseY = 0.1f * (Mouse.GetState().Y - previousMouseState.Y);
                Vector3 camera = Vector3.Transform(new Vector3(0, 0, 5), Matrix.CreateTranslation(midMouseX, midMouseY, distance) * Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lightAngle -= 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lightAngle += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                lightAngle2 += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                lightAngle2 -= 0.02f;
            }

            lightPosition = Vector3.Transform(new Vector3(0.5f, 1, 1), Matrix.CreateRotationX(lightAngle2) * Matrix.CreateRotationY(lightAngle));
            
            //Camera and light reset
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
                lightPosition = new Vector3(0.5f, 1, 1);
                angle = 0;
                angle2 = 0;
                lightAngle = 0;
                lightAngle2 = 0;
                distance = 0f;
                midMouseX = 0f;
                midMouseY = 0f;
                ambient = new Vector4(0, 0, 0, 0);
            }

            effect.Parameters["View"].SetValue(view);
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["LightPosition"].SetValue(lightPosition);
                        
            //Part B - model selection
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                model = Content.Load<Model>("box");                
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                model = Content.Load<Model>("Sphere");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                model = Content.Load<Model>("Torus");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                model = Content.Load<Model>("Teapot");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D5))
            {
                model = Content.Load<Model>("bunny");
            }

            //Part C - shader selection and settings
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                toggleTechnique = 0;
                shaderType = "Gouraud";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                toggleTechnique = 1;
                shaderType = "Phong";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                toggleTechnique = 2;
                shaderType = "PhongBlinn";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {

                toggleTechnique = 3;
                shaderType = "Schlick";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                toggleTechnique = 4;
                shaderType = "Toon";

            }
            if (Keyboard.GetState().IsKeyDown(Keys.F6))
            {
                toggleTechnique = 5;
                shaderType = "HalfLife";
            }

            //Light Intensity
            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                ambientIntensity += 0.02f;

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambientIntensity -= 0.04f;
                }                
            }

            //R value intensity
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {                
                ambient += new Vector4(0.02f, 0, 0, 0);                

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambient -= new Vector4(0.04f, 0, 0, 0);
                }
            }

            //G value intensity
            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                ambient += new Vector4(0, 0.02f, 0, 0);

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambient -= new Vector4(0, 0.04f, 0, 0);
                }                                               
            }

            //B value intensity
            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                ambient += new Vector4(0, 0, 0.02f, 0);

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    ambient -= new Vector4(0, 0, 0.04f, 0);
                }                
            }

            //Shininess controls
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    shininess += 0.02f;
                }
                else
                {
                    specularIntensity += 0.02f;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    shininess -= 0.02f;
                }
                else
                {
                    specularIntensity -= 0.02f;
                }
            }

            //Show/Hide help for controls
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    boolHelp = false;
                }
                else
                {
                    boolHelp = true;
                }
            }

            //Show/Hide info about shader: camera angle, light angle, shader type, intensity, specular, rgb values of light, etc
            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    boolInfo = false;
                }
                else
                {
                    boolInfo = true;
                }
            }

            effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
            effect.Parameters["AmbientColor"].SetValue(ambient);

            previousMouseState = Mouse.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
           
            //
            spriteBatch.Begin();
            if (boolInfo)
            {
                spriteBatch.DrawString(font, "light angle: " + lightAngle + "\nlight angle2: " + lightAngle2, new Vector2(25,25), Color.White);
                spriteBatch.DrawString(font, "camera angle: " + angle + "\ncamera angle2: " + angle2 + "\ndistance: " + distance, new Vector2(150, 25), Color.White);
                spriteBatch.DrawString(font, "midMouseX: " + midMouseX + "\nmidMouseY: " + midMouseY + "\nambient: " + ambient, new Vector2(300, 25), Color.White);
                spriteBatch.DrawString(font, "ambientIntensity: " + ambientIntensity + "\nspecularIntensity: " + specularIntensity, new Vector2(425, 25), Color.White);
                spriteBatch.DrawString(font, "shininess: " + shininess + "\nview: " + view + "\nShader Type: " + shaderType, new Vector2 (25, 75), Color.White);

            }

            if (boolHelp)
            {
                spriteBatch.DrawString(font, "Rotate Camera: Mouse Left Drag\nChange the distance of camera to the center: Mouse Right Drag\nTranslate the camera: Mouse Middle Drag \n", new Vector2(25,5), Color.White);
                spriteBatch.DrawString(font, "Rotate the light: Arrow keys \nReset camera and light: S Key", new Vector2(25, 75), Color.White);
                spriteBatch.DrawString(font, "1: A box\n2: A Sphere \n3: A Torus \n4: A Tea Pot \n5: A Bunny", new Vector2(25, 125), Color.White);
                spriteBatch.DrawString(font, "F1: Gouraud (Phong per vertex) \nF2: Phong per pixel\nF3: PhongBlinn\nF4: Schlick\nF5: Toon \nF6: HalfLife", new Vector2(25, 225), Color.White);
                spriteBatch.DrawString(font, "L: Increase the intensity of light (+ Shift key: decrease)\nR: Increase the red value of light (+ Shift key: decrease)\nG: Increase the green value of light (+ Shift key: decrease)\nB: Increase the blue value of light (+ Shift key: decrease)", new Vector2(25, 350), Color.White);
                spriteBatch.DrawString(font, "+ (plus):  Increases the intensity  (minus): Decreases the intensity", new Vector2(25, 425), Color.White);
            }

            spriteBatch.End();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

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
                        effect.Parameters["LightPosition"].SetValue(lightPosition);
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
            base.Draw(gameTime);
        }
    }
}