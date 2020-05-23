using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab02
{
    public class Lab02 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect;

        //Lab02 Stuff
        float angle = 0.0f;
        float distance = 1.0f;
        Matrix world = Matrix.Identity;
        Matrix view;
        Matrix projection;

        VertexPositionTexture[] vertices =
        {
            new VertexPositionTexture(new Vector3(0,1,0), new Vector2(0.5f,0f)),
            new VertexPositionTexture(new Vector3(1,0,0), new Vector2(1.0f,1.0f)),
            new VertexPositionTexture(new Vector3(-1,0,0), new Vector2(0f,1.0f)),
        };

        public Lab02()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize() { base.Initialize(); }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("SimplestShader");

            effect.Parameters["MyTexture"].SetValue(Content.Load<Texture2D>("logo_mg"));

        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Lab02
            /* if(Keyboard.GetState().IsKeyDown(Keys.Left))
             {
                 angle += 0.02f;
                 Vector3 offset = new Vector3((float)System.Math.Cos(angle), (float)System.Math.Sin(angle), 0);
                 effect.Parameters["offset"].SetValue(offset);
             }
             */

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angle += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle -= 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                distance += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                distance -= 0.02f;
            }

            Vector3 cameraPosition = distance * new Vector3((float)System.Math.Sin(angle), 0, (float)System.Math.Cos(angle));
            Vector3 offset = new Vector3((float)System.Math.Cos(angle), (float)System.Math.Sin(angle), 0);

            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0,0,0), new Vector3(0,1,0));

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);

            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}