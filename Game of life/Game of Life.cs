using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game_Of_Life
{
	public class GameOfLife
	{
		private bool[,] Grid;
		public int Width { get; }
		public int Height { get; }

		public GameOfLife(int width, int height)
        {
			Grid = new bool[height, width];
			Width = width;
			Height = height;
        }

		public void changeCell(int y, int x) { Grid[y, x] = !Grid[y, x]; }
		public void setCell(int y, int x, bool color) { Grid[y, x] = color; }
		public bool getCell(int y, int x) { return Grid[y, x]; }

		public List<(int, int)> nextGen()
        {
			List<(int, int)> changed = new List<(int, int)>();
			for(int y = 0; y < Height; y++)
            {
				for (int x = 0; x < Width; x++)
				{
                    int score = 0;

					if (x >= 1)
                    {
						if (Grid[y, x - 1]) 
							score++;
						if (y >= 1 && Grid[y - 1, x - 1]) 
							score++;
						if (y + 1 < Height && Grid[y + 1, x - 1]) 
							score++;
					}
					if (x + 1 < Width)
                    {
						if (Grid[y, x + 1]) 
							score++;
						if (y >= 1 && Grid[y - 1, x + 1]) 
							score++;
						if (y < Height - 1 && Grid[y + 1, x + 1]) 
							score++;
					}
					if (y >= 1 && Grid[y - 1, x]) 
						score++;
					if (y < Height - 1 && Grid[y + 1, x]) 
						score++;

					if ((Grid[y, x] && (score < 2 || score > 3)) || (!Grid[y, x] && score == 3)) 
						changed.Add((y, x));
                } 
			}
			foreach((int y, int x) in changed) 
				Grid[y, x] = !Grid[y, x];
			return changed;
        }
	}
}