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
using System.Security.AccessControl;

namespace AoC2022_NoUnity
{
    internal static class Day22
    {
       
        private static Vec2 Right = new Vec2(1, 0);
        private static Vec2 Left = new Vec2(-1, 0);
        private static Vec2 Down = new Vec2(0, 1);
        private static Vec2 Up = new Vec2(0, -1);

        private static Dictionary<int, (int min, int max)> Widths = new();
        private static Dictionary<int, (int min, int max)> Heights = new();
    
        public static void Solve()
        {
            // PARSE
            string[] lines = File.ReadAllLines("input.txt");
            char[] commands = lines[lines.Length- 1].ToCharArray();
            Queue<char> commandQueue = new Queue<char>(commands);
            lines = lines[..(lines.Length - 1)];

            int maxWidth = lines.Max(l => l.Length);
            lines = lines.Select(l => l.PadRight(maxWidth)).ToArray();

            FindHeightsAndWidths(lines);

            int firstFreeX = Widths[0].min;
            Vec2 CurrentDir = new Vec2(1,0);
            Vec2 CurrentPos = new Vec2(firstFreeX,0);
            while(commandQueue.Count > 0)
            {
                //Draw(lines, CurrentPos, CurrentDir);
                int stepCount;
                StringBuilder sb = new();

                // If next in queue is integer, find full number and set step count
                while(commandQueue.Count > 0 && int.TryParse(commandQueue.Peek().ToString(), out stepCount))
                {
                    sb.Append(commandQueue.Dequeue());
                }
                if (sb.Length > 0)
                    stepCount = int.Parse(sb.ToString());
                else
                {
                    // If not integer, set new direction (Inversed because up y is negative)
                    CurrentDir = commandQueue.Dequeue() == 'R' ? new Vec2() { x = -CurrentDir.y, y = CurrentDir.x }
                                                                : new Vec2() { x = CurrentDir.y, y = -CurrentDir.x };
                    continue;
                }

                for (int i = 0; i < stepCount; i++)
                {
                    Vec2 nextPos = CurrentPos + CurrentDir;

                    if(nextPos.y < 0 || nextPos.x < 0 || nextPos.x >= lines[0].Length || lines[nextPos.y][nextPos.x] == ' ')
                    {
                        // The next move is a space
                        if(CurrentDir.x != 0)
                        {
                            // Horizontal move
                            int nextX = CurrentDir.x > 0 ? Widths[CurrentPos.y].min : Widths[CurrentPos.y].max;
                            nextPos = new Vec2(nextX, CurrentPos.y);
                        }
                        else
                        {
                            // Vertical move
                            int nextY = CurrentDir.y > 0 ? Heights[CurrentPos.x].min : Heights[CurrentPos.x].max;
                            nextPos = new Vec2(CurrentPos.x, nextY);
                        }
                    }

                    char nextMove = lines[nextPos.y][nextPos.x];
                    if (nextMove == '#')
                        break;

                    if (nextMove == '.')
                        CurrentPos = nextPos;
                }
            }

            Draw(lines, CurrentPos, CurrentDir);
            Console.WriteLine($"Final position = {CurrentPos.x},{CurrentPos.y}");
            Console.WriteLine($"Final direction = {CurrentDir.x},{CurrentDir.y}");
            int dirValue = 0;
            if(CurrentDir.x < 0)
                dirValue = 2;
            else if (CurrentDir.y < 0)
                dirValue = 1;
            else if (CurrentDir.y > 0)
                dirValue = 3;

            int pswd = 1000 * (CurrentPos.y + 1) + 4 * (CurrentPos.x + 1) + dirValue;
            Console.WriteLine("Password = " + pswd);
        }

        private static void Draw(string[] lines, Vec2 currentPos, Vec2 currentDir)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[0].Length; j++)
                {
                    if(i == currentPos.y && j == currentPos.x)
                    {
                        char dir = '>';
                        if (currentDir.x < 0)
                            dir = '<';
                        else if (currentDir.y < 0)
                            dir = '^';
                        else if (currentDir.y > 0)
                            dir = 'v';
                        Console.Write(dir);
                    }
                    else
                        Console.Write(lines[i][j]);
                }
                Console.WriteLine();
            }
        }

        private static void FindHeightsAndWidths(string[] lines)
        {
            List<string> transposedLines = new();
            for (int j = 0; j < lines[0].Length; j++)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    sb.Append(line[j]);
                }
                transposedLines.Add(sb.ToString());
            }

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                int min = line.TakeWhile(c => c == ' ').Count();
                int max = line.Count() - 1 - line.Reverse().TakeWhile(c => c == ' ').Count();
                Widths[i] = (min, max);
            }

            for (int i = 0; i < transposedLines.Count; i++)
            {
                string line = transposedLines[i];

                int min = line.TakeWhile(c => c == ' ').Count();
                int max = line.Count() - 1 - line.Reverse().TakeWhile(c => c == ' ').Count();
                Heights[i] = (min, max);
            }


        }
    }
}
