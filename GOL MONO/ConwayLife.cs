using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GOL_MONO
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ConwayLife : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /*  
         * 
         *  
         1 Any live cell with fewer than two live neighbours dies, as if caused by under-population.
         2 Any live cell with two or three live neighbours lives on to the next generation.
         3 Any live cell with more than three live neighbours dies, as if by overcrowding.
         4 Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
         */
        Random r = new Random();
        GraphicsDevice g;
        Texture2D pixelTexture;
        private Grid world;
        SpriteBatch spriteBatch;
        private KeyboardState newState;
        private KeyboardState oldState;
        float time;
        int maxLife = 0;
        int minLife = 1000;
        float resetTime = 20;
        float newLifeInterval = 5;
        int width = 70;
        int height = 40;
        int w = 10;
        int h = 10;
        int generation = 0;

        public ConwayLife(Game game, GraphicsDevice g)
            : base(game)
        {
            // TODO: Construct any child components here
            this.g = g;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelTexture = new Texture2D(g, 1, 1, true, SurfaceFormat.Color);
            uint[] pixel = { Color.White.PackedValue };
            pixelTexture.SetData<uint>(pixel);

            this.world = new Grid(width, height);

            NiceGrowthThenStable();


            world.SetNewGeneration();

            base.LoadContent();
        }


        private void Reset()
        {
            this.world = new Grid(70, 40);

            NiceGrowthThenStable();


            world.SetNewGeneration();
            time = 0;
        }

        private void NiceGrowthThenStable()
        {
            world.SetGliderDR(5, 5);
            world.SetGliderDR(5, 15);
            world.SetGliderDL(12, 5);
            world.SetGliderDR(25, 10);
            world.SetGliderDL(27, 25);
            world.SetGliderDL(2, 25);
            world.SetGliderDL(10, 0);
            world.SetGliderDR(35, 10);
            world.SetGliderDR(0, 10);
            world.SetGliderDL(0, 25);
            world.SetGliderDL(2, 35);
            world.SetGliderDR(25, 30);
        }
        bool run = true;
        int frame = 0;
        int frameThresh = 3;
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > resetTime)
            {
                //   Reset();
                //IntroduceGliders();
            }
            oldState = newState;
            newState = Keyboard.GetState();
            frame++;
            if (frame >= frameThresh)
            {
                frame = 0;
                if (newState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space) || run)
                {
                    for (int row = 0; row < world.Height; row++)
                    {
                        for (int col = 0; col < world.Width; col++)
                        {
                            Rule1(col, row);
                            Rule2(col, row);
                            Rule3(col, row);
                            Rule4(col, row);
                        }
                    }
                    world.SetNewGeneration();
                    generation++;
                }
            }
            maxLife = Math.Max(maxLife, world.ActiveCount);
            minLife = Math.Min(minLife, world.ActiveCount);
            base.Update(gameTime);
        }

        private void IntroduceGliders()
        {
            for (int i = 0; i < 3; i++)
            {
                world.SetGliderDR(r.Next(width), r.Next(height));
                world.SetGliderDL(r.Next(width), r.Next(height));
            }
            time = 0;
            resetTime = newLifeInterval;
        }
        /// <summary>
        ///  Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void Rule4(int col, int row)
        {
            if (world.ItemAt(col, row) == Grid.DEAD && world.NeigbourCount(col, row) == 3)
                world.Set(col, row, Grid.ALIVE);

        }
        /// <summary>
        /// Any live cell with more than three live neighbours dies, as if by overcrowding.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void Rule3(int col, int row)
        {
            if (world.ItemAt(col, row) == Grid.ALIVE && world.NeigbourCount(col, row) > 3)
                world.Set(col, row, Grid.DEAD);

        }
        /// <summary>
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void Rule2(int col, int row)
        {
            if (world.ItemAt(col, row) == Grid.ALIVE && (world.NeigbourCount(col, row) == 2 || world.NeigbourCount(col, row) == 3))
                world.Set(col, row, Grid.ALIVE);
        }

        /// <summary>
        /// 1 Any live cell with fewer than two live neighbours dies, as if caused by under-population.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void Rule1(int col, int row)
        {
            if (world.ItemAt(col, row) == Grid.ALIVE && world.NeigbourCount(col, row) < 2)
            {
                world.Set(col, row, Grid.DEAD);
            }
        }


        public override void Draw(GameTime gameTime)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch.Begin();
            for (int row = 0; row < world.Height; row++)
            {
                for (int col = 0; col < world.Width; col++)
                {
                    if (world.ItemAt(col, row) == Grid.ALIVE)
                        spriteBatch.Draw(pixelTexture, new Rectangle(20 + col * w, 20 + row * h, w, h), Color.Black);
                    else
                        spriteBatch.Draw(pixelTexture, new Rectangle(20 + col * w, 20 + row * h, w, h), Color.White);

                }
            } 
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public string ActiveCells { get { return world.ActiveCount.ToString(); } }
    }
}
