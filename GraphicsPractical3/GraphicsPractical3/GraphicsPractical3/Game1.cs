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
        // R: the camera object
        private Camera camera;
        // R: the angle to rotate the world with
        private float angle;

        // R: Models
        private Model[] models;
        private float[] modelScales;

        // R: model to display
        int displayNumber;
        // R: flag that ensures only one switch per keypress
        bool wasReleased;


        // R: constructor for the game1 class
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
            // R: initialize the view angle
            angle = 0.0f;

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
            Effect bunnyEffect = this.Content.Load<Effect>("Effects/Effect0");

            // R: TODO: Load models
            this.models[0] = this.Content.Load<Model>("Models/bunny");
            this.models[0].Meshes[0].MeshParts[0].Effect = bunnyEffect;
            this.modelScales[0] = 200.0f;

            // R: model 2
            Effect headEffect = this.Content.Load<Effect>("Effects/Effect1");
            this.models[1] = this.Content.Load<Model>("Models/femalehead");
            this.models[1].Meshes[0].MeshParts[0].Effect = headEffect;
            this.modelScales[1] = 1f;

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
            // R: read and process keyboard input
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
            // R: exit the game when the escape key is hit
            if (kbState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            
            // R: rotate: update the viewing angle
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.angle += timeStep * 1.0f; // this last one is the rotation speed

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
            // R: Draw the scenes 
            // R: draw scene 0
            // R: Get the model's only mesh
            ModelMesh mesh0 = this.models[0].Meshes[0];
            Effect effect0 = mesh0.Effects[0];

            // R: Set the effect parameters
            effect0.CurrentTechnique = effect0.Techniques["Technique1"];
            // Matrices for 3D perspective projection
            this.camera.SetEffectParameters(effect0);

            // R: create the world matrix for the model
            Matrix World0 = Matrix.CreateScale(150f) * Matrix.CreateTranslation(100 * (displayNumber), -12, 0) * Matrix.CreateRotationY(angle);

            // R: set the world matrix to the effect
            effect0.Parameters["World"].SetValue(World0);
            effect0.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(World0)));

            Vector4[] lightPositions = new Vector4[5];
            lightPositions[0] = new Vector4(50.0f, 50.0f, 50.0f, 0.0f);
            lightPositions[1] = new Vector4(-50.0f, 50.0f, 50.0f, 0.0f);
            lightPositions[2] = new Vector4(-50.0f, 50.0f, -50.0f, 0.0f);
            lightPositions[3] = new Vector4(50.0f, 50.0f, -50.0f, 0.0f);
            lightPositions[4] = new Vector4(0.0f, 50.0f, 0.0f, 0.0f);
            effect0.Parameters["LightPositions"].SetValue(lightPositions);

            Vector4[] lightColors = new Vector4[5];
            lightColors[0] = new Vector4(0.6f, 0.0f, 0.0f, 0.0f);
            lightColors[1] = new Vector4(0.0f, 0.0f, 0.6f, 0.0f);
            lightColors[2] = new Vector4(0.0f, 0.6f, 0.0f, 0.0f);
            lightColors[3] = new Vector4(0.3f, 0.3f, 0.0f, 0.0f);
            lightColors[4] = new Vector4(0.2f, 0.2f, 0.2f, 0.0f);
            effect0.Parameters["LightColors"].SetValue(lightColors);


            mesh0.Draw();

            // R: draw scene 1
            // R: Get the model's only mesh
            ModelMesh mesh1 = this.models[1].Meshes[0];
            Effect effect1 = mesh1.Effects[0];

            // R: Set the effect parameters
            effect1.CurrentTechnique = effect1.Techniques["Technique1"];
            // Matrices for 3D perspective projection
            this.camera.SetEffectParameters(effect1);

            // R: create the world matrix for the model
            Matrix World1 = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(100 * (displayNumber-1), 0, 0);

            // R: set the world matrix to the effect
            effect1.Parameters["World"].SetValue(World1);

            mesh1.Draw();

            base.Draw(gameTime);
        }
    }
}
