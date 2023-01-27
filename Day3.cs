using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022_NoUnity
{
    internal static class Day3
    {
        public static void Solve()
        {
            int score = 0;
            string[] lines = File.ReadAllLines("input.txt");
            for (int j = 0; j < lines.Length; j += 3)
            {
                bool found = false;
                string partOne = lines[j];
                string partTwo = lines[j + 1];
                string partThree = lines[j + 2];

                // Check a-z
                for (int i = 97; i < 123; i++)
                {
                    char c = (char)i;
                    if (partOne.Contains(c) && partTwo.Contains(c) && partThree.Contains(c))
                    {
                        score += i - 96;
                        found = true;
                        break;
                    }
                }

                if (found == true)
                    continue;

                // Check A-Z
                for (int i = 65; i < 91; i++)
                {
                    char c = (char)i;
                    if (partOne.Contains(c) && partTwo.Contains(c) && partThree.Contains(c))
                    {
                        score += i - 38;
                        break;
                    }
                }
            }

            Console.WriteLine(score);
        }
    }
}
