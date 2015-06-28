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
        private int displayNumber;
        // R: flag that ensures only one switch per keypress
        private bool wasReleased;

        // R: render targets for blurring
        private RenderTarget2D renderTargetOriginal;
        private RenderTarget2D renderTargeHorizontalBlur;


        private Effect effect3;
        private VertexPositionNormalTexture[] quadVertices;
        private short[] quadIndices;

        // R: filter used for the Gaussian blur
        float[] gaussianDistribution;
        
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

            // Set up render targets for blurring
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            renderTargetOriginal = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            renderTargeHorizontalBlur = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);

            // TODO: use this.Content to load your game content here
            // R: TODO: load effects

            // R: TODO: Load models
            // R: model 0
            Effect effect0 = this.Content.Load<Effect>("Effects/Effect0");
            this.models[0] = this.Content.Load<Model>("Models/bunny");
            this.models[0].Meshes[0].MeshParts[0].Effect = effect0;
            this.modelScales[0] = 200.0f;

            // R: model 1
            Effect effect1 = this.Content.Load<Effect>("Effects/Effect1");
            this.models[1] = this.Content.Load<Model>("Models/femalehead");
            this.models[1].Meshes[0].MeshParts[0].Effect = effect1;
            this.modelScales[1] = 1f;

            // R: model 2
            Effect effect2 = this.Content.Load<Effect>("Effects/Effect2");
            this.models[2] = this.Content.Load<Model>("Models/bunny2");
            this.models[2].Meshes[0].MeshParts[0].Effect = effect2;
            this.modelScales[2] = 200.0f;

            // R: Set the effect parameters for scene 0
            effect0.CurrentTechnique = effect0.Techniques["Technique1"];

            // R: Set the lights
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

            // R: scene 2

            // R: set the light
            lightPositions = new Vector4[1];
            lightPositions[0] = new Vector4(50.0f, 50.0f, 50.0f, 0.0f);
            effect2.Parameters["LightPositions"].SetValue(lightPositions);
            lightColors = new Vector4[1];
            lightColors[0] = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
            effect2.Parameters["LightColors"].SetValue(lightColors);


            effect3 = this.Content.Load<Effect>("Effects/Effect3");
            this.setupQuad();

            // R: load the gaussian blur
            // Calculated from http://dev.theomader.com/gaussian-kernel-calculator/, with sigma = 0.785 and size = 7
            // This best approximates the kernel from https://en.wikipedia.org/wiki/Gaussian_blur
            gaussianDistribution = new float[7] 
            {
                0.00072f,
                0.027289f,
                0.23407f,
                0.475842f,
                0.23407f,
                0.027289f,
                0.00072f
            };
            // Normalize the distribution
            gaussianDistribution = this.normalize(gaussianDistribution);
            // R: pass the 1D kernel to the effect
            effect3.Parameters["BlurKernel"].SetValue(gaussianDistribution);
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
                this.displayNumber = (displayNumber + 1) % models.Length;
                this.angle = 0.0f;
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

            // R: update the scenes
            // R: update scene 0
            if (displayNumber == 0)
            {
                // R: Get the model's only effect
                Effect effect0 = this.models[0].Meshes[0].Effects[0];

                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effect0);

                // R: create the world matrix for the model
                Matrix World0 = Matrix.CreateScale(150f) * Matrix.CreateTranslation(100 * (displayNumber), -12, 0) * Matrix.CreateRotationY(angle);

                // R: set the world matrix to the effect
                effect0.Parameters["World"].SetValue(World0);
                effect0.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(World0)));
            }

            // R: update scene 2
            if (displayNumber == 2)
            {
                // R: Get the model's only effect
                Effect effect2 = this.models[2].Meshes[0].Effects[0];

                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effect2);

                // R: create the world matrix for the model
                Matrix World2 = Matrix.CreateScale(150f) * Matrix.CreateTranslation(100 * (displayNumber-2), -12, 0) * Matrix.CreateRotationY(angle);

                // R: set the world matrix to the effect
                effect2.Parameters["World"].SetValue(World2);
                effect2.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(World2)));
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            // TODO: Add your drawing code here
            // R: Draw the scenes 
            // R: draw scene 0
            if (displayNumber == 0)
            {
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
                
                // R: get the model's only mesh
                ModelMesh mesh0 = this.models[0].Meshes[0];

                // R: draw the mesh
                mesh0.Draw();
            }

            if (displayNumber == 1)
            {
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
                
                // R: draw scene 1
                // R: Get the model's only mesh
                ModelMesh mesh1 = this.models[1].Meshes[0];
                Effect effect1 = mesh1.Effects[0];

                // R: Set the effect parameters
                effect1.CurrentTechnique = effect1.Techniques["Technique1"];
                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effect1);

                // R: create the world matrix for the model
                Matrix World1 = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(100 * (displayNumber - 1), 0, 0);

                // R: set the world matrix to the effect
                effect1.Parameters["World"].SetValue(World1);

                mesh1.Draw();
            }

            // R: Draw scene 2
            if (displayNumber == 2)
            {
                // R: set render target to texture before clearing
                GraphicsDevice.SetRenderTarget(renderTargetOriginal);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
                
                // R: get the model's only mesh
                ModelMesh mesh2 = this.models[2].Meshes[0];
                // R: get the model's effect
                Effect effect2 = mesh2.Effects[0];
                // R: set the technique
                effect2.CurrentTechnique = effect2.Techniques["RenderScene"];

                // R: draw the mesh
                mesh2.Draw();
                GraphicsDevice.SetRenderTarget(renderTargeHorizontalBlur);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

                // R: draw the texture to the second renderTarget

                this.camera.Eye = new Vector3(0, 0, 100);

                // added: set the technique of the quad
                this.effect3.CurrentTechnique = effect3.Techniques["Technique1"];
                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effect3);
                this.effect3.Parameters["World"].SetValue(Matrix.CreateScale(55.5f));
                this.effect3.Parameters["t"].SetValue((Texture2D)renderTargetOriginal);

                float BlurDistanceX = 1.0f / (float)this.graphics.PreferredBackBufferWidth;
                this.effect3.Parameters["BlurDistanceX"].SetValue(BlurDistanceX);

                // added: draw the quad
                // added: apply effect passes
                foreach (EffectPass pass in this.effect3.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                // added: draw the quad using the QuadEffect
                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.quadVertices, 0, this.quadVertices.Length, this.quadIndices, 0, this.quadIndices.Length / 3);
                GraphicsDevice.SetRenderTarget(null);

                // R: draw to the screen

                // added: set the technique of the quad
                this.effect3.CurrentTechnique = effect3.Techniques["Technique2"];
                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effect3);
                this.effect3.Parameters["World"].SetValue(Matrix.CreateScale(55.5f));
                this.effect3.Parameters["t"].SetValue((Texture2D)renderTargeHorizontalBlur);

                float BlurDistanceY = 1.0f / (float)this.graphics.PreferredBackBufferHeight;
                this.effect3.Parameters["BlurDistanceY"].SetValue(BlurDistanceY);

                // added: draw the quad
                // added: apply effect passes
                foreach (EffectPass pass in this.effect3.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                // added: draw the quad using the QuadEffect
                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.quadVertices, 0, this.quadVertices.Length, this.quadIndices, 0, this.quadIndices.Length / 3);

                this.camera.Eye = new Vector3(0, 50, 100);
            }

            base.Draw(gameTime);
        }

        // set up the projection quad used for blurring
        private void setupQuad()
        {
            // Normal points up
            Vector3 quadNormal = new Vector3(0, 1, 0);

            this.quadVertices = new VertexPositionNormalTexture[4];
            // Top left
            this.quadVertices[0].Position = new Vector3(-1, 0.75f, 0);
            this.quadVertices[0].Normal = quadNormal;
            this.quadVertices[0].TextureCoordinate = new Vector2(0, 0);
            // Top right
            this.quadVertices[1].Position = new Vector3(1, 0.75f, 0);
            this.quadVertices[1].Normal = quadNormal;
            this.quadVertices[1].TextureCoordinate = new Vector2(1, 0);
            // Bottom left
            this.quadVertices[2].Position = new Vector3(-1, -0.75f, 0);
            this.quadVertices[2].Normal = quadNormal;
            this.quadVertices[2].TextureCoordinate = new Vector2(0, 1);
            // Bottom right
            this.quadVertices[3].Position = new Vector3(1, -0.75f, 0);
            this.quadVertices[3].Normal = quadNormal;
            this.quadVertices[3].TextureCoordinate = new Vector2(1, 1);

            this.quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
        }

        // This function normalizes an array of floats
        // used to normalize the gaussian distribution for blurring
        private float[] normalize(float[] values)
        {
            // declare the sum of the array
            float sum = 0;
            //declare the output array
            float[] result = new float[values.Length];
            foreach (float f in values)
            {
                sum += f;
            }
            for (int i = 0; i < values.Length; i++ )
            {
                result[i] = values[i] / sum;
            }
            return values;
        }
    }
}
