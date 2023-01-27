using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022_NoUnity
{
    internal static class Day10
    {
        public static void Solve()
        {
            string[] lines = File.ReadAllLines("input.txt");
            int registerValue = 1;
            int cycleCounter = 1;
            int signalStrength = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
            {
                int value = 0;
                int cycles = 1;
                if(line.Contains("addx"))
                {
                    value = int.Parse(line.Split(' ')[1]);
                    cycles = 2;
                }

                for (int i = 0; i < cycles; i++)
                {
                    if (cycleCounter % 40 == 0)
                        sb.AppendLine();

                    int pixelDrawn = cycleCounter % 40 - 1;
                    if (registerValue - 1 <= pixelDrawn && pixelDrawn <= registerValue + 1)
                        sb.Append("#");
                    else
                        sb.Append(".");

                    cycleCounter++;
                }

                registerValue += value;
            }

            Console.WriteLine(sb.ToString());
        }

    }
}
