using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022_NoUnity
{
    internal static class Day7
    {
        static string[]? lines;
        static int sum;
        static List<int> sizes = new();

        public static void Solve()
        {
            lines = File.ReadAllLines("input.txt");
            int unusedSpace = 70000000 - sizeOfDir("/", 0);
            int missingSpace = 30000000 - unusedSpace;
            Console.WriteLine("Total: " + sum);

            int deleteSize = sizes.OrderBy(i => i).First(i => i >= missingSpace);
            Console.WriteLine("Delete size: " + deleteSize);
        }

        private static int sizeOfDir(string dir, int startLine)
        {
            int size = 0;
            int depth = 0;

            // Find directory
            for (int i = startLine; i < lines.Count(); i++)
            {
                string line = lines[i];

                if (!line.Contains("$ cd "))
                    continue;

                if (line.Contains("$ cd .."))
                    depth--;
                else if (line.Equals("$ cd " + dir) && depth == 0)
                {
                    // Loop through content
                    for (int j = i + 2; j < lines.Count(); j++)
                    {
                        line = lines[j];
                        if (line.Contains("$ cd "))
                            break;

                        int res;
                        if (int.TryParse(line.Split(' ')[0], out res))
                            size += res;

                        if (line.Split(' ')[0].Equals("dir"))
                            size += sizeOfDir(line.Split(' ')[1], j);
                    }
                    break;
                }
                else
                    depth++;
            }
            Console.WriteLine("Directory " + dir + ": " + size);
            if (size <= 100000)
                sum += size;
            sizes.Add(size);
            return size;
        }
    }
}
