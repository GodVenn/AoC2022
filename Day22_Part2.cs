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
    internal static class Day22Part2
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
                    Vec2 nextDir = CurrentDir;

                    // Wrapping
                    if(nextPos.y < 0 || nextPos.x < 0 || nextPos.x >= maxWidth || lines[nextPos.y][nextPos.x] == ' ')
                    {
                        // The next move is a space or edge
                        (nextPos, nextDir) = NextSide(CurrentPos, CurrentDir);
                        //Draw(lines, CurrentPos, CurrentDir);
                        //Draw(lines, nextPos, nextDir);
                    }

                    // Moving (Or not)
                    char nextMove = lines[nextPos.y][nextPos.x];
                    if (nextMove == '#')
                    {
                        break;
                    }

                    if (nextMove == '.')
                    {
                        CurrentPos = nextPos;
                        CurrentDir = nextDir;
                    }
                }
            }

            Draw(lines, CurrentPos, CurrentDir);
            Console.WriteLine($"Final position = [{CurrentPos.x},{CurrentPos.y}]");
            Console.WriteLine($"Final direction = [{CurrentDir.x},{CurrentDir.y}]");
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

        private static (Vec2 nextPos, Vec2 nextDir) NextSide(Vec2 pos, Vec2 dir) 
        {
            if(dir.x > 0)
            {
                if (pos.y < 50)
                    return (new Vec2(99, 149 - pos.y), new Vec2(-1, 0));
                if (50 <= pos.y && pos.y < 100)
                    return (new Vec2(50 + pos.y, 49), new Vec2(0, -1));
                if (100 <= pos.y && pos.y < 150)
                    return (new Vec2(149, 149 - pos.y), new Vec2(-1, 0));
                if (pos.y >= 150)
                    return (new Vec2(pos.y - 100, 149), new Vec2(0, -1));
            }
            if(dir.x < 0)
            {
                if (pos.y < 50)
                    return (new Vec2(0, 149 - pos.y), new Vec2(1, 0));
                if (50 <= pos.y && pos.y < 100)
                    return (new Vec2(pos.y - 50, 150), new Vec2(0, 1));
                if (100 <= pos.y && pos.y < 150)
                    return (new Vec2(50, 149 - pos.y), new Vec2(1, 0));
                if (pos.y >= 150)
                    return (new Vec2(pos.y - 100, 0), new Vec2(0, 1));
            }
            if(dir.y < 0)
            {
                if (pos.x < 50)
                    return (new Vec2(50, 50 + pos.x), new Vec2(1, 0));
                if (50 <= pos.x && pos.x < 100)
                    return (new Vec2(0, 100 + pos.x), new Vec2(1, 0));
                if (100 <= pos.x && pos.x < 150)
                    return (new Vec2(pos.x - 100, 199), new Vec2(0, -1));
            }
            if (dir.y > 0)
            {
                if (pos.x < 50)
                    return (new Vec2(100 + pos.x, 0), new Vec2(0, 1));
                if (50 <= pos.x && pos.x < 100)
                    return (new Vec2(49, 100 + pos.x), new Vec2(-1, 0));
                if (100 <= pos.x && pos.x < 150)
                    return (new Vec2(149, pos.x - 50), new Vec2(-1, 0));
            }

            throw new Exception("Error");
        }
    }
}
