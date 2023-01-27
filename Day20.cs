using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Numerics;
using System.Diagnostics;

namespace AoC2022_NoUnity
{
    internal static class Day20
    {
       
       private class IdNumber
        {
            public long Number;

            public IdNumber(long number)
            {
                Number = number;
            }
        }


        public static void Solve()
        {
            // PARSE
            string[] lines = File.ReadAllLines("input.txt");
            var list = lines.Select(i => int.Parse(i)).ToList();
            const long key = 811589153;

            List<IdNumber> OriginalNumbers = lines.Select(i => new IdNumber(long.Parse(i) * key)).ToList();
            List<IdNumber> MixedNumbers = new List<IdNumber>(OriginalNumbers);
            Console.WriteLine("[{0}]", string.Join(", ", MixedNumbers.Select(n => n.Number)));

            for (int i = 0; i < 10; i++)
            {
                foreach (var number in OriginalNumbers)
                {
                    int index = MixedNumbers.IndexOf(number);
                    int newIndex = LoopedIndex(OriginalNumbers.Count, (long)index + number.Number);
                    if (newIndex < index)
                    {
                        MixedNumbers.Insert(newIndex, number);
                        MixedNumbers.RemoveAt(index + 1);
                    }
                    else
                    {
                        MixedNumbers.Insert(newIndex + 1, number);
                        MixedNumbers.RemoveAt(index);
                    }
                    //Console.WriteLine("[{0}]", string.Join(", ", MixedNumbers.Select(n => n.Number)));
                }
            }

            int indexOfZero = MixedNumbers.IndexOf(MixedNumbers.Single(n => n.Number == 0));
            long n1 = MixedNumbers[(indexOfZero + 1000) % (MixedNumbers.Count)].Number;
            long n2 = MixedNumbers[(indexOfZero + 2000) % (MixedNumbers.Count)].Number;
            long n3 = MixedNumbers[(indexOfZero + 3000) % (MixedNumbers.Count)].Number;
            Console.WriteLine($"1000th = {n1}");
            Console.WriteLine($"2000th = {n2}");
            Console.WriteLine($"3000th = {n3}");
            Console.WriteLine(n1+n2+n3);
        }

        private static int LoopedIndex(int listSize, long index)
        {
            int maxIndex = listSize - 1;
            int newIndex = (int)(index % (long)maxIndex);
            newIndex = newIndex < 0 ? maxIndex + newIndex: newIndex;

            if (newIndex == 0)
                return maxIndex;

            if (newIndex == maxIndex)
                return 0;

            return newIndex;
        }

    }
}
