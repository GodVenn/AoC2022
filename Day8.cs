using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022_NoUnity
{
    internal static class Day8
    {
        private static int[][] _grid;
        public static void Solve()
        {
            int scenicScoreMax = 0;
            string[] lines = File.ReadAllLines("input.txt");
            _grid = lines.Select(x => x.Select(y => int.Parse(y.ToString())).ToArray()).ToArray();
            for (int i = 0; i < _grid.Length; i++)
            {
                int[] row = _grid[i];
                for (int j = 0; j < row.Length; j++)
                {
                    int score = getScore(i,j, row[j]);
                    if(score > scenicScoreMax)
                        scenicScoreMax = score;
                }
            }

            Console.WriteLine("Max Score: " + scenicScoreMax);

        }

        private static int getScore(int i, int j, int height)
        {
            int score = 1;
            // Up
            int dist = 0;
            for (int up = i - 1; up >= 0; up--)
            {
                dist++;
                if (_grid[up][j] >= height)
                {
                    break;
                }
            }
            score *= dist;

            // Down
            dist = 0;
            for (int down = i + 1; down < _grid.Count(); down++)
            {
                dist++;
                if (_grid[down][j] >= height)
                {
                    break;
                }
            }
            score *= dist;

            // Right
            dist = 0;
            for (int right = j + 1; right < _grid[0].Count(); right++)
            {
                dist++;
                if (_grid[i][right] >= height)
                {
                    break;
                }
            }
            score*= dist;

            // Left
            dist = 0;
            for (int left = j - 1; left >=0 ; left--)
            {
                dist++;
                if (_grid[i][left] >= height)
                {
                    break;
                }
            }
            score *= dist;

            return score;

        }
        private static bool isVisible(int i, int j, int height)
        {
            // Up
            bool visible = true;
            for (int up = i - 1; up >= 0; up--)
            {
                if (_grid[up][j] >= height)
                {
                    visible = false;
                    break;
                }
            }
            if (visible)
                return true;

            // Down
            visible = true;
            for (int down = i + 1; down < _grid.Count(); down++)
            {
                if (_grid[down][j] >= height)
                {
                    visible = false;
                    break;
                }
            }
            if (visible)
                return true;

            // Right
            visible = true;
            for (int right = j + 1; right < _grid[0].Count(); right++)
            {
                if (_grid[i][right] >= height)
                {
                    visible = false;
                    break;
                }
            }
            if (visible)
                return true;

            visible = true;
            // Left
            for (int left = j - 1; left >=0 ; left--)
            {
                if (_grid[i][left] >= height)
                {
                    visible = false;
                    break;
                }
            }
            if (visible)
                return true;


            return false;
        }

    }
}
