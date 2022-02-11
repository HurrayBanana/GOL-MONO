using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOL_MONO
{
    public class Grid
    {
        public const int DEAD = 0;
        public const int ALIVE = 1;
        //public const int EMPTY = 0;

        int[,] grid;
        int[,] temporary;

        /// <summary>
        /// an array holding 2 2d arrays
        /// </summary>
        int[][,] worlds = new int[2][,];

        /// <summary>
        /// reference to which 
        /// </summary>
        int activeWorld = 0;

        public int ItemAt(int x, int y)
        {
            return grid[Y(y), X(x)];
        }
        public int ItemAtT(int x, int y)
        {
            return temporary[Y(y), X(x)];
        }

        int width;

        public int Width
        {
            get { return width; }
        }
        int height;
        private int lastCount;

        public int Height
        {
            get { return height; }
        }

        public Grid(int width, int height)
        {

            grid = new int[height, width];
            temporary = new int[height, width];
            this.width = width;
            this.height = height;

            worlds[0] = grid;
            worlds[1] = temporary;

        }

        /// <summary>
        /// determines the number of neighbours from this position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int NeigbourCount(int x, int y)
        {
            int count = 0;
            int px = 0;
            int py = 0;
            for (int row = y - 1; row < y - 1 + 3; row++)
            {
                for (int col = x - 1; col < x - 1 + 3; col++)
                {
                    //don't count yourself
                    if (!(row == y && col == x))
                    {
                        px = col;
                        py = row;
                        if (px >= width) px -= width;
                        if (px < 0) px += width;
                        if (py >= height) py -= height;
                        if (py < 0) py += height;
                        {
                            count += (grid[py, px] == ALIVE) ? 1 : 0;
                        }
                    }
                }
            }
            return count;
        }

        public void SetNewGeneration()
        {
            lastCount = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    grid[row, col] = temporary[row, col];
                    lastCount += grid[row, col];
                }
            }
        }

        public void Set(int x, int y, int value)
        {
            temporary[Y(y), X(x)] = value;
        }

        /// <summary>
        /// returns a validated x coord
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        int X(int x)
        {
            if (x < 0) return x + width;
            if (x >= width) return x - width;
            return x;
        }
        /// <summary>
        /// returns a validated y coord
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        int Y(int y)
        {
            if (y < 0) return y + height;
            if (y >= height) return y - height;
            return y;
        }
        public void SetGliderDR(int x, int y)
        {
            temporary[Y(y), X(x + 2)] = Grid.ALIVE;
            temporary[Y(y + 1), X(x + 2)] = Grid.ALIVE;
            temporary[Y(y + 2), X(x + 2)] = Grid.ALIVE;
            temporary[Y(y + 1), X(x)] = Grid.ALIVE;
            temporary[Y(y + 2), X(x + 1)] = Grid.ALIVE;
        }
        public void SetGliderDL(int x, int y)
        {
            temporary[Y(y), X(x)] = Grid.ALIVE;
            temporary[Y(y + 1), X(x)] = Grid.ALIVE;
            temporary[Y(y + 2), X(x)] = Grid.ALIVE;
            temporary[Y(y + 1), X(x + 2)] = Grid.ALIVE;
            temporary[Y(y + 2), X(x + 1)] = Grid.ALIVE;
        }


        public int ActiveCount { get { return lastCount; } }
    }
}
