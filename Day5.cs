using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022_NoUnity
{
    internal static class Day5
    {
        public static void Solve()
        {
            string[] lines = File.ReadAllLines("input.txt");
            int maxHeight = 0;
            int stackCount = 0;

            // Find sizes
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains('1'))
                {
                    maxHeight = i;
                    stackCount = int.Parse(lines[i].Trim().Last().ToString());
                    break;
                }
            }
            List<Stack<char>> stacks = new List<Stack<char>>();

            // Parse
            int boxSize = 4;
            for (int i = maxHeight-1; i >=0; i--)
            {
                string currentLine = lines[i];
                for (int j = 0; j < stackCount; j++)
                {
                    if(j >= stacks.Count)
                        stacks.Add(new Stack<char>());

                    char currentBox = currentLine[1 + j * boxSize];
                    if (string.IsNullOrWhiteSpace(currentBox.ToString()))
                        continue;

                    stacks[j].Push(currentBox);
                }
            }

            // Parse commands
            for (int i = maxHeight + 1; i < lines.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                int count = int.Parse(lines[i].Split(' ')[1].ToString());
                int from = int.Parse(lines[i].Split(' ')[3].ToString());
                int to = int.Parse(lines[i].Split(' ')[5].ToString());

                Stack<char> temp = new Stack<char>();
                for (int j = 0; j < count; j++)
                {
                    char box = stacks[from - 1].Pop();
                    temp.Push(box);
                }
                
                for(int j = 0; j < count; j++)
                {
                    stacks[to-1].Push(temp.Pop());
                }
            }

            Console.WriteLine("Result:");
            stacks.ForEach(stack => Console.Write(stack.Peek()));
        }
    }
}
