using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab01
{
    public class Lab01 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect;

        VertexPositionTexture[] vertices =
        {
            new VertexPositionTexture(new Vector3(0,1,0), new Vector2(0.5f,0f)),
            new VertexPositionTexture(new Vector3(1,0,0), new Vector2(1.0f,1.0f)),
            new VertexPositionTexture(new Vector3(-1,0,0), new Vector2(0f,1.0f)),
        };

        /* For Color Changing
         * VertexPositionColor[] vertices =
        {
            new VertexPositionColor(new Vector3(0,1,0), Color.White),
            new VertexPositionColor(new Vector3(1,0,0), Color.Blue),
            new VertexPositionColor(new Vector3(-1,0,0), Color.Red),
        };
        */
        public Lab01()
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
