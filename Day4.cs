using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022_NoUnity
{
    internal static class Day4
    {
        public static void Solve()
        {
            string[] lines = File.ReadAllLines("input.txt");
            int count = 0;
            foreach (string line in lines)
            {
                string one = line.Split(',')[0];
                string two = line.Split(',')[1];

                if ((int.Parse(one.Split('-')[1]) >= int.Parse(two.Split('-')[0]) 
                    && int.Parse(one.Split('-')[1]) <= int.Parse(two.Split('-')[1]))
                    ||
                    (int.Parse(one.Split('-')[0]) >= int.Parse(two.Split('-')[0])
                    && int.Parse(one.Split('-')[0]) <= int.Parse(two.Split('-')[1])))
                {
                    count++;
                    continue;
                }

                if ((int.Parse(two.Split('-')[1]) >= int.Parse(one.Split('-')[0])
                    && int.Parse(two.Split('-')[1]) <= int.Parse(one.Split('-')[1]))
                    ||
                    (int.Parse(two.Split('-')[0]) >= int.Parse(one.Split('-')[0])
                    && int.Parse(two.Split('-')[0]) <= int.Parse(one.Split('-')[1])))
                {
                    count++;
                    continue;
                }
            }

            Console.WriteLine(count);
        }
    }
}
