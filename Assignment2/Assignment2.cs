﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411_2020.SimpleEngine;

namespace Assignment2
{
    public class Assignment2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect;
        Model model;
        Texture2D texture;
        SpriteFont font;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 20), Vector3.Zero, Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 1.33f, 0.1f, 1000f);

        Vector3 cameraOffset = new Vector3(0, 0, 0);
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        Vector3 lightPosition = new Vector3(0, 0, 1);
        float lightAngleX, lightAngleY, cameraAngleX, cameraAngleY, angle, angle2 = 0;

        float distance = 10.0f;

        int toggleTechnique = 0;
        bool boolHelp = false;
        bool boolInfo = false;
        bool boolValue = true;

        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.9f;

        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 diffuseLightDirection = new Vector3(1, 1, 1);
        float diffuseIntensity = 0.6f;

        Vector4 specularColor = new Vector4(1, 1, 1, 1);

        float shininess = 100.0f;
        float etaRatio = 0.70f;

        Vector3 fresnelEtaRatio = new Vector3(0.1f, 0.1f, 0.1f);
        float reflectivity = 0.5f;
        float fresnelPower = 2;
        float fresnelScale = 15;
        float fresnelBias = 0.5f;

        Skybox skybox;
        string[] skyboxTextures;

        MouseState previousMouseState;
        KeyboardState previousKeyboardState;

        int shaderType = 0;
        int skyboxType = 0;
        int modelType = 0;

        public Assignment2()
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

            model = Content.Load<Model>("Box");
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);
            effect = Content.Load<Effect>("Assignment2Shader");
            font = Content.Load<SpriteFont>("Font");
            texture = Content.Load<Texture2D>("HelicopterTexture");
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            bool shift = false;
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in pressedKeys)
                if (key == Keys.LeftShift || key == Keys.RightShift) shift = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) lightAngleX += 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) lightAngleX -= 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) lightAngleY += 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) lightAngleY -= 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { distance = 10; cameraAngleX = cameraAngleY = lightAngleX = lightAngleY = 0; }            

            if (Keyboard.GetState().IsKeyDown(Keys.D6)) { model = Content.Load<Model>("Helicopter"); boolValue = true; }

            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) reflectivity += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) reflectivity -= 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.F7)) shaderType = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.F8)) shaderType = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.F9)) shaderType = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.F10)) shaderType = 3;

            if (Keyboard.GetState().IsKeyDown(Keys.D7)) skyboxType = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) skyboxType = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D9)) skyboxType = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D0)) skyboxType = 3;

            if (Keyboard.GetState().IsKeyDown(Keys.R) && !shift) fresnelEtaRatio.X += 0.04f;
            if (Keyboard.GetState().IsKeyDown(Keys.G) && !shift) fresnelEtaRatio.Y += 0.04f;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && !shift) fresnelEtaRatio.Z += 0.04f;

            if (Keyboard.GetState().IsKeyDown(Keys.R) && shift) fresnelEtaRatio.X -= 0.04f;
            if (Keyboard.GetState().IsKeyDown(Keys.G) && shift) fresnelEtaRatio.Y -= 0.04f;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && shift) fresnelEtaRatio.Z -= 0.04f;

            if (Keyboard.GetState().IsKeyDown(Keys.Q) && !shift) fresnelPower += 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && !shift) fresnelScale += 1f;
            if (Keyboard.GetState().IsKeyDown(Keys.E) && !shift) fresnelBias += 0.02f;

            if (Keyboard.GetState().IsKeyDown(Keys.Q) && shift) fresnelPower -= 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && shift) fresnelScale -= 1f;
            if (Keyboard.GetState().IsKeyDown(Keys.E) && shift) fresnelBias -= 0.02f;

            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H)) boolInfo = !boolInfo;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion)) boolHelp = !boolHelp;


            if (previousMouseState.RightButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                cameraOffset.Z += (Mouse.GetState().X - previousMouseState.X) / 100f;
                distance += (Mouse.GetState().X - previousMouseState.X) / 100f;
            }

            if (previousMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                cameraAngleX += (previousMouseState.X - Mouse.GetState().X);
                cameraAngleY += (previousMouseState.Y - Mouse.GetState().Y);
            }

            if (previousMouseState.MiddleButton == ButtonState.Pressed && Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                cameraOffset.X += (Mouse.GetState().X - previousMouseState.X) / 100f;
                cameraOffset.Y -= (Mouse.GetState().Y - previousMouseState.Y) / 100f;
            }

            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance), Matrix.CreateRotationX(MathHelper.ToRadians(cameraAngleY)) * Matrix.CreateRotationY(MathHelper.ToRadians(cameraAngleX)));
            view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(MathHelper.ToRadians(cameraAngleY)) * Matrix.CreateRotationY(MathHelper.ToRadians(cameraAngleX))));
            lightPosition = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationX(MathHelper.ToRadians(lightAngleX)) * Matrix.CreateRotationY(MathHelper.ToRadians(lightAngleY)));

            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;

            RasterizerState ras = new RasterizerState();
            ras.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = ras;

            if (skyboxType == 0)
            {
                string[] skyboxTextures =
                {
                    "Colors/debug_posx", "Colors/debug_negx",
                    "Colors/debug_posy", "Colors/debug_negy",
                    "Colors/debug_posz", "Colors/debug_negz" 
                };
                skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }

            else if (skyboxType == 1)
            {
                string[] skyboxTextures =
                {
                    "Office/nvlobby_new_posx", "Office/nvlobby_new_negx",
                    "Office/nvlobby_new_posy", "Office/nvlobby_new_negy",
                    "Office/nvlobby_new_posz", "Office/nvlobby_new_negz"
                };
                skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }

            else if (skyboxType == 2)
            {
                string[] skyboxTextures =
                {
                    "Daytime/grandcanyon_posx", "Daytime/grandcanyon_negx",
                    "Daytime/grandcanyon_posy", "Daytime/grandcanyon_negy",
                    "Daytime/grandcanyon_posz", "Daytime/grandcanyon_negz"
                };
                skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }

            else if (skyboxType == 3)
            {
                string[] skyboxTextures =
                {
                   "Planets/Planets", "Planets/Planets2",
                   "Planets/Planets3", "Planets/Planets4",
                    "Planets/Planets5", "Planets/Planets6",
                };
               skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            }

            skybox.Draw(view, projection, cameraPosition);
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
            DrawModelWithEffect();
            spriteBatch.Begin();
            if (boolHelp) showHelp();
            if (boolInfo) showInfo();
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void showHelp()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Mouse: Views", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Arrow Keys: Light Position", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type S Key: Reset Camera", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type 6: To have Helicopter", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type 7,8,9,0: Skybox Texture change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type F7,F8,F9,F10: Shader Change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type plus, minus: Reflectivity Change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type R, G, B Keys (+shift decreases): Color Change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type Q, W, E Keys(+shift decreases): Fresnel Power, Scale, Bias change", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type H Key: Display Parameters", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Type ? Key: Help Menu", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
        }

        private void showInfo()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Light Angles X(" + lightAngleX + "), Y(" + lightAngleY + ")", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Light Pos" + show(lightPosition), Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Camera Angles X(" + cameraAngleX + "), Y(" + cameraAngleY + ")", Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Camera Pos" + show(cameraPosition), Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Shader: " + shaderType, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Reflectivity: " + reflectivity, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Fresnel Power: " + fresnelPower, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Fresnel Bias: " + fresnelBias, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Fresnel Scale: " + fresnelScale, Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Fresnel etaRatio: " + show(fresnelEtaRatio), Vector2.UnitX * 400 + Vector2.UnitY * 15 * (i++), Color.White);
        }

        private string show(Vector4 v)
        {
            return "(" + v.X.ToString("0.00") + "," + v.Y.ToString("0.00") + "," + v.Z.ToString("0.00") + "," + v.W.ToString("0.00") + ")";
        }
        private string show(Vector3 v)
        {
            return "(" + v.X.ToString("0.00") + "," + v.Y.ToString("0.00") + "," + v.Z.ToString("0.00") + ")";
        }
        void DrawModelWithEffect()
        {
            effect.CurrentTechnique = effect.Techniques[shaderType];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["DiffuseLightDirection"].SetValue(lightPosition);
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(shininess);
                        effect.Parameters["EtaRatio"].SetValue(etaRatio);
                        effect.Parameters["FresnelEtaRatio"].SetValue(fresnelEtaRatio);
                        effect.Parameters["Reflectivity"].SetValue(reflectivity);
                        effect.Parameters["FresnelBias"].SetValue(fresnelBias);
                        effect.Parameters["FresnelScale"].SetValue(fresnelScale);
                        effect.Parameters["FresnelPower"].SetValue(fresnelPower);


                        if (boolValue) effect.Parameters["decalMap"].SetValue(texture);
                        effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }
    }
}