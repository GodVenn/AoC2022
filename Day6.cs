using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022_NoUnity
{
    internal static class Day6
    {
        public static void Solve()
        {
            string input = File.ReadAllText("input.txt");
            List<char> sequence = new List<char>();
            int seqSize = 14;

            // init
            sequence = input[0..seqSize].ToList();

            for (int i = seqSize; i < input.Length; i++)
            {
                if(sequence.Distinct().Count() == seqSize)
                {
                    Console.WriteLine("Result: " + i);
                    break;
                }

                sequence.RemoveAt(0);
                sequence.Add(input[i]);
            }
        }
    }
}
