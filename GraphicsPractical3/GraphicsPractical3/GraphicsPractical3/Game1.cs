using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GraphicsPractical3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private FrameRateCounter frameRateCounter;

        // Game objects and variables
        private Camera camera;

        // R: Models
        private Model[] models;
        private float[] modelScales;

        // R: model to display
        int displayNumber;
        bool wasReleased;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Create and add a frame rate counter
            this.frameRateCounter = new FrameRateCounter(this);
            this.Components.Add(this.frameRateCounter);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Copy over the device's rasterizer state to change the current fillMode
            this.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
            // Set up the window
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = false;
            // Let the renderer draw and update as often as possible
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            // Flush the changes to the device parameters to the graphics card
            this.graphics.ApplyChanges();
            // Initialize the camera
            this.camera = new Camera(new Vector3(0, 50, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            this.IsMouseVisible = true;

            // R: initialize displayNumber
            displayNumber = 0;
            bool wasReleased = true;
            // R: initialize model array
            models = new Model[6];
            modelScales = new float[6];

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // R: TODO: load effects
            Effect bunnyEffect = this.Content.Load<Effect>("Effects/Effect1");
            // R: TODO: Load models
            this.models[0] = this.Content.Load<Model>("Models/bunny");
            this.models[0].Meshes[0].MeshParts[0].Effect = bunnyEffect;
            this.modelScales[0] = 200.0f;

            // R: model 2
            Effect headEffect = this.Content.Load<Effect>("Effects/CellShader");
            this.models[1] = this.Content.Load<Model>("Models/femalehead");
            this.models[1].Meshes[0].MeshParts[0].Effect = headEffect;
            this.modelScales[1] = 2f;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // added: Get keyboard state
            KeyboardState kbState = Keyboard.GetState();
            // added: switch views at the press of spacebar
            if (wasReleased && kbState.IsKeyDown(Keys.Space))
            {
                displayNumber = (displayNumber + 1) % models.Length;
                wasReleased = false;
            }
            if (!wasReleased && kbState.IsKeyUp(Keys.Space))
            {
                wasReleased = true;
            }


            if (displayNumber == 1)
            {
                // get the meshes effect
                Effect headEffect = this.models[1].Meshes[0].Effects[0];
                // R: Set the effect parameters
                headEffect.CurrentTechnique = headEffect.Techniques["Technique1"];
                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(headEffect);

                // R: create the world matrix for the model
                Matrix World1 = Matrix.CreateScale(2f) * Matrix.CreateTranslation(100 * (displayNumber-1), 0, 0);

                // R: set the world matrix to the effect
                headEffect.Parameters["DiffuseColor"].SetValue(new Vector4(1f, 1f, 0.0f, 1.0f));
                headEffect.Parameters["World"].SetValue(World1);
                headEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(World1)));
            }

            // Update the window title
            this.Window.Title = "XNA Renderer | FPS: " + this.frameRateCounter.FrameRate;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            // TODO: Add your drawing code here
            // R: Draw the scenes that have instantiated models
            ModelMesh mesh;
            Effect effect;
            Matrix World;

            // R: draw scene 0
            // R: Get the model's only mesh
            mesh = this.models[0].Meshes[0];
            effect = mesh.Effects[0];

            

            mesh.Draw();

            // R: draw scene 1
            // R: Get the model's only mesh
            mesh = this.models[1].Meshes[0];
            
            mesh.Draw();

            base.Draw(gameTime);
        }
    }
}
