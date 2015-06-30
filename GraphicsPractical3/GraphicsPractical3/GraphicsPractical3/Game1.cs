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
        // R: the initial Camera position
        private Vector3 cameraPosition;
        // R: the angle to rotate the world with
        private float angle;

        // R: Models
        private Model[] models;

        // R: model to display
        private int displayNumber;
        // R: flag that ensures only one switch per keypress
        private bool wasReleased;

        // E5
        // N: Grayscale Picture of QueenQuad :)
        private VertexPositionNormalTexture[] QueenQuadVertices;
        private short[] QueenQuadIndices;
        private Matrix QueenQuadTransform;
        Effect QueenQuadEffect;


        // E6
        // R: render targets for blurring
        private RenderTarget2D renderTargetOriginal;
        private RenderTarget2D renderTargeHorizontalBlur;

        // Effect for the blur
        private Effect effectE6Blur;
        // vertices and indices for the blurQuad
        private VertexPositionNormalTexture[] quadVertices;
        private short[] quadIndices;

        // R: filter used for the Gaussian blur
        float[] gaussianDistribution;


        // M3
        // vertices for the mirrorQuad
        private VertexPositionNormalTexture[] mirrorQuad;
        // Effect for drawing the mirror
        Effect mirrorEffect;
        // scale and position of the mirrorQuad
        float mirrorScale;
        Vector3 mirrorPosition;
        
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
            // R: Turn on the Depth stencil
            this.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
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
            cameraPosition = new Vector3(0, 50, 100);
            this.camera = new Camera(cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            // R: initialize the view angle
            angle = 0.0f;

            this.IsMouseVisible = true;

            // R: initialize displayNumber
            displayNumber = 0;
            bool wasReleased = true;
            // R: initialize model array and scale array (due to the different sizes of models)
            models = new Model[5];

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
            renderTargetOriginal = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);
            renderTargeHorizontalBlur = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24Stencil8);

            // Here all content is loaded for all scenes

            // E1
            // R: model E1
            Effect effectE1 = this.Content.Load<Effect>("Effects/EffectE1");
            this.models[0] = this.Content.Load<Model>("Models/bunny");
            this.models[0].Meshes[0].MeshParts[0].Effect = effectE1;

            // R: Set the effect parameters for scene E1
            effectE1.CurrentTechnique = effectE1.Techniques["Technique1"];

            // R: Set the lights
            Vector4[] lightPositions = new Vector4[5];
            lightPositions[0] = new Vector4(50.0f, 50.0f, 50.0f, 0.0f);
            lightPositions[1] = new Vector4(-50.0f, 50.0f, 50.0f, 0.0f);
            lightPositions[2] = new Vector4(-50.0f, 50.0f, -50.0f, 0.0f);
            lightPositions[3] = new Vector4(50.0f, 50.0f, -50.0f, 0.0f);
            lightPositions[4] = new Vector4(0.0f, 50.0f, 0.0f, 0.0f);
            effectE1.Parameters["LightPositions"].SetValue(lightPositions);

            Vector4[] lightColors = new Vector4[5];
            lightColors[0] = new Vector4(0.6f, 0.0f, 0.0f, 0.0f);
            lightColors[1] = new Vector4(0.0f, 0.0f, 0.6f, 0.0f);
            lightColors[2] = new Vector4(0.0f, 0.6f, 0.0f, 0.0f);
            lightColors[3] = new Vector4(0.3f, 0.3f, 0.0f, 0.0f);
            lightColors[4] = new Vector4(0.2f, 0.2f, 0.2f, 0.0f);
            effectE1.Parameters["LightColors"].SetValue(lightColors);

            // E5
            // load "QueenQuadEffect" effect for the quad
            QueenQuadEffect = this.Content.Load<Effect>("Effects/QueenEffect");
            // set the technique of effect
            this.QueenQuadEffect.CurrentTechnique = QueenQuadEffect.Techniques["Technique1"];

            // load the texture to the QueenQuadEffect
            QueenQuadEffect.Parameters["QueenTexture"].SetValue(Content.Load<Texture>("Textures/queen"));
            // setup the World TLV
            this.QueenQuadEffect.Parameters["World"].SetValue(Matrix.CreateScale(100.0f));
            // Position the camera dead ahead of the quad, and pass to the QueenEffect
            new Camera(new Vector3(0, 0, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0)).SetEffectParameters(QueenQuadEffect);
            // Setup the QueenQuad
            setupQueenQuad();


            // E6
            // R: load the model and effect for the scene
            Effect effectE6 = this.Content.Load<Effect>("Effects/EffectE6M3");
            this.models[2] = this.Content.Load<Model>("Models/bunny2");
            this.models[2].Meshes[0].MeshParts[0].Effect = effectE6;

            // R: set the light
            lightPositions = new Vector4[1];
            lightPositions[0] = new Vector4(50.0f, 50.0f, 50.0f, 0.0f);
            effectE6.Parameters["LightPositions"].SetValue(lightPositions);
            lightColors = new Vector4[1];
            lightColors[0] = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
            effectE6.Parameters["LightColors"].SetValue(lightColors);

            // load the effect for blurring
            effectE6Blur = this.Content.Load<Effect>("Effects/EffectE6M3_blur");
            // set the view and projection matrices as if the camera is dead ahead of the projection quad

            this.setupQuad();

            // R: set the gaussian distribution
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
            effectE6Blur.Parameters["BlurKernel"].SetValue(gaussianDistribution);

            // M3
            // NB: scene M3 also reuses the model+effect from E6
            // Load the effect of the mirror
            mirrorEffect = this.Content.Load<Effect>("Effects/EffectM3Mirror");
            mirrorEffect.CurrentTechnique = mirrorEffect.Techniques["Technique1"];
            mirrorScale = 35.0f;
            // R: define the position of the mirror
            mirrorPosition = new Vector3(-0.5f * mirrorScale, 0, -40.0f);
            this.setupMirror();
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
            // R: update scene E1
            if (displayNumber == 0)
            {
                // R: Get the model's only effect
                Effect effectE1 = this.models[0].Meshes[0].Effects[0];

                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effectE1);

                // R: create the world matrix for the model
                Matrix World = Matrix.CreateScale(150f) * Matrix.CreateTranslation(0, -12, 0) * Matrix.CreateRotationY(angle);

                // R: set the world matrix to the effect
                effectE1.Parameters["World"].SetValue(World);
                effectE1.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(World)));
            }

            // R: update scene E6
            if (displayNumber == 2)
            {
                // R: Get the model's only effect
                Effect effectE6 = this.models[2].Meshes[0].Effects[0];

                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effectE6);

                // R: create the world matrix for the model
                Matrix World = Matrix.CreateScale(150f) * Matrix.CreateTranslation(0, -12, 0) * Matrix.CreateRotationY(angle);

                // R: set the world matrix to the effect
                effectE6.Parameters["World"].SetValue(World);
                effectE6.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(World)));
            } 
            
            // R: update scene M3
            // it's the same as scene E6
            if (displayNumber == 4)
            {
                // R: update the scene
                // R: Get the model's only effect
                Effect effectE6 = this.models[2].Meshes[0].Effects[0];

                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effectE6);

                // R: create the world matrix for the model
                Matrix World = Matrix.CreateScale(150f) * Matrix.CreateTranslation(0, -12, 0) * Matrix.CreateRotationY(angle);

                // R: set the world matrix to the effect
                effectE6.Parameters["World"].SetValue(World);
                effectE6.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(World)));

                // R: update the mirror
                // R: create the world position matrix for the mirrorQuad
                Matrix mirrorWorld = Matrix.CreateScale(mirrorScale) * Matrix.CreateTranslation(mirrorPosition);
                mirrorEffect.Parameters["World"].SetValue(mirrorWorld);
                this.camera.SetEffectParameters(mirrorEffect);
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
            // R: draw scene E1
            if (displayNumber == 0)
            {
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
                
                // R: get the model's only mesh
                ModelMesh meshE1 = this.models[0].Meshes[0];

                // R: draw the mesh
                meshE1.Draw();
            }


            // draw E5
            if (displayNumber == 1)
            {
                // Clear the screen in a predetermined color and clear the depth buffer
                this.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);
                
                // added: draw the QueenQuad to the backbuffer
                this.DrawQueenQuad();
            }

            // R: Draw scene E6
            if (displayNumber == 2)
            {
                // R: set render target to texture before clearing
                GraphicsDevice.SetRenderTarget(renderTargetOriginal);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
                
                // R: get the model's only mesh
                ModelMesh meshE6 = this.models[2].Meshes[0];
                // R: get the model's effect
                Effect effectE6 = meshE6.Effects[0];
                // R: set the technique
                effectE6.CurrentTechnique = effectE6.Techniques["RenderScene"];

                // R: draw the mesh
                meshE6.Draw();

                // R: draw the texture to the second renderTarget
                GraphicsDevice.SetRenderTarget(renderTargeHorizontalBlur);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

                // R: set the camera perpendicular on the blurQuad
                this.camera.Eye = new Vector3(0, 0, 100);

                // added: set the technique of the quad
                this.effectE6Blur.CurrentTechnique = effectE6Blur.Techniques["Technique1"];
                // set the matrices for 3D perspective projection in the effect
                this.camera.SetEffectParameters(effectE6Blur);
                this.effectE6Blur.Parameters["World"].SetValue(Matrix.CreateScale(55.5f));
                this.effectE6Blur.Parameters["t"].SetValue((Texture2D)renderTargetOriginal);

                float BlurDistanceX = 1.0f / (float)this.graphics.PreferredBackBufferWidth;
                this.effectE6Blur.Parameters["BlurDistanceX"].SetValue(BlurDistanceX);

                // added: draw the quad
                // added: apply effect passes
                foreach (EffectPass pass in this.effectE6Blur.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                // added: draw the quad using the QuadEffect
                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.quadVertices, 0, this.quadVertices.Length, this.quadIndices, 0, this.quadIndices.Length / 3);
                GraphicsDevice.SetRenderTarget(null);

                // R: draw to the screen

                // added: set the technique of the quad
                this.effectE6Blur.CurrentTechnique = effectE6Blur.Techniques["Technique2"];
                // Matrices for 3D perspective projection
                this.camera.SetEffectParameters(effectE6Blur);
                this.effectE6Blur.Parameters["World"].SetValue(Matrix.CreateScale(55.5f));
                this.effectE6Blur.Parameters["t"].SetValue((Texture2D)renderTargeHorizontalBlur);

                float BlurDistanceY = 1.0f / (float)this.graphics.PreferredBackBufferHeight;
                this.effectE6Blur.Parameters["BlurDistanceY"].SetValue(BlurDistanceY);

                // added: draw the quad
                // added: apply effect passes
                foreach (EffectPass pass in this.effectE6Blur.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                // added: draw the quad using the QuadEffect
                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.quadVertices, 0, this.quadVertices.Length, this.quadIndices, 0, this.quadIndices.Length / 3);

                // reset the camera to the correct position
                this.camera.Eye = new Vector3(0, 50, 100);
            }

            // R: draw scene M3
            if (displayNumber == 4)
            {
                // R: get the model's only mesh
                // R: we're reusing the model of scene 2
                ModelMesh meshM3 = this.models[2].Meshes[0];
                // R: get the model's effect
                Effect effectE6 = meshM3.Effects[0];
                // R: set the technique
                effectE6.CurrentTechnique = effectE6.Techniques["RenderScene"];
                camera.SetEffectParameters(effectE6);

                // clear the backbuffer, including the stencil
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.CornflowerBlue, 1.0f, 0);

                // render the mirrorquad to the stencil buffer
                GraphicsDevice.DepthStencilState = addIfMirror;
                this.mirrorEffect.CurrentTechnique.Passes[0].Apply();
                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.mirrorQuad, 0, this.mirrorQuad.Length, this.quadIndices, 0, this.quadIndices.Length / 3);

                // mirror the camera position, camera look direction, and world
                // camera-position
                Vector3 cameraPositionMirrored = cameraPosition;
                cameraPositionMirrored.Z = (-1 * cameraPosition.Z) + mirrorPosition.Z*2;
                // origin
                Vector3 originMirrored = new Vector3(0, 0, mirrorPosition.Z * 2);
                camera = new Camera(cameraPositionMirrored, originMirrored, new Vector3(0, 1, 0));
                // world
                Matrix reflection = Matrix.CreateScale(new Vector3(-1, 1, 1));
                // R: create the world matrix for the model
                Matrix reflectedWorld4 = Matrix.CreateScale(150f) * Matrix.CreateTranslation(100 * (displayNumber - 4), -12, 0) * Matrix.CreateRotationY(angle) * reflection;
                
                // reset the depth buffer
                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

                // render the scene to the backbuffer where the stencilbuffer == 1, i.e. in the mirrorQuad
                GraphicsDevice.DepthStencilState = checkMirror;
                camera.SetEffectParameters(effectE6);
                effectE6.Parameters["World"].SetValue(reflectedWorld4);
                meshM3.Draw();

                // undo mirroring of camera and world
                camera = new Camera(cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                // R: recreate the normal world matrix for the model
                Matrix World = Matrix.CreateScale(150f) * Matrix.CreateTranslation(100 * (displayNumber - 4), -12, 0) * Matrix.CreateRotationY(angle);
                camera.SetEffectParameters(effectE6);
                effectE6.Parameters["World"].SetValue(World);

                // render the scene to the backbuffer normally
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.Clear(ClearOptions.Stencil | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
                meshM3.Draw();
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

        // This function sets up the mirrorQuad in de XY-plane
        private void setupMirror()
        {
            // Normal points along the z-axis
            Vector3 quadNormal = new Vector3(0, 0, 1);

            this.mirrorQuad = new VertexPositionNormalTexture[4];
            // Top left
            this.mirrorQuad[0].Position = new Vector3(0, 1, 0);
            this.mirrorQuad[0].Normal = quadNormal;
            this.mirrorQuad[0].TextureCoordinate = new Vector2(0, 0);
            // Top right
            this.mirrorQuad[1].Position = new Vector3(1, 1, 0);
            this.mirrorQuad[1].Normal = quadNormal;
            this.mirrorQuad[1].TextureCoordinate = new Vector2(1, 0);
            // Bottom left
            this.mirrorQuad[2].Position = new Vector3(0, 0, 0);
            this.mirrorQuad[2].Normal = quadNormal;
            this.mirrorQuad[2].TextureCoordinate = new Vector2(0, 1);
            // Bottom right
            this.mirrorQuad[3].Position = new Vector3(1, 0, 0);
            this.mirrorQuad[3].Normal = quadNormal;
            this.mirrorQuad[3].TextureCoordinate = new Vector2(1, 1);
        }

        // Sets up the QueenQuad for E5
        private void setupQueenQuad()
        {
            float scale = 50.0f;

            // Normal points up
            Vector3 QueenQuadNormal = new Vector3(0, 0, 1);

            this.QueenQuadVertices = new VertexPositionNormalTexture[4];
            // Top left
            this.QueenQuadVertices[0].Position = new Vector3(-0.5f, 0.3f, 0);
            this.QueenQuadVertices[0].Normal = QueenQuadNormal;
            this.QueenQuadVertices[0].TextureCoordinate = new Vector2(0, 0);
            // Top right
            this.QueenQuadVertices[1].Position = new Vector3(0.5f, 0.3f, 0);
            this.QueenQuadVertices[1].Normal = QueenQuadNormal;
            this.QueenQuadVertices[1].TextureCoordinate = new Vector2(1, 0);
            // Bottom left
            this.QueenQuadVertices[2].Position = new Vector3(-0.5f, -0.3f, 0);
            this.QueenQuadVertices[2].Normal = QueenQuadNormal;
            this.QueenQuadVertices[2].TextureCoordinate = new Vector2(0, 1);
            // Bottom right
            this.QueenQuadVertices[3].Position = new Vector3(0.5f, -0.3f, 0);
            this.QueenQuadVertices[3].Normal = QueenQuadNormal;
            this.QueenQuadVertices[3].TextureCoordinate = new Vector2(1, 1);

            this.QueenQuadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
            this.QueenQuadTransform = Matrix.CreateScale(scale);
        }

        // Draws the QueenQuad for E5
        protected void DrawQueenQuad()
        {
            // added: apply effect passes
            foreach (EffectPass pass in this.QueenQuadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            // added: draw the picture using the QueenQuadEffect
            this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.QueenQuadVertices, 0, this.QueenQuadVertices.Length, this.QueenQuadIndices, 0, this.QueenQuadIndices.Length / 3);
        }

        // stencil buffers used to create a correct mirror scene
        // code from iloveshaders.blogspot.nl/2011/05/using-stencil-buffer-rendering-stencil.html
        DepthStencilState addIfMirror = new DepthStencilState()
        {
            StencilEnable = true,
            StencilFunction = CompareFunction.Always,
            StencilPass = StencilOperation.Increment
        };
        DepthStencilState checkMirror = new DepthStencilState()
        {
            StencilEnable = true,
            StencilFunction = CompareFunction.Equal,
            ReferenceStencil = 1,
            StencilPass = StencilOperation.Keep
        };
    }
}
