using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022_NoUnity
{
    internal static class Day9
    {
        public static void Solve()
        {
            string[] lines = File.ReadAllLines("input.txt");
            List<(int x, int y)> visited = new List<(int x, int y)>();
            (int x, int y) start = (0, 0);
            visited.Add(start);
            List<(int x, int y)> ropePieces = new(){ start, start, start, start, start, start, start, start, start, start };
            foreach (var line in lines)
            {
                var movement = ParseCommand(line);
                int amplitude = Math.Max(Math.Abs(movement.x), Math.Abs(movement.y));
                for (int i = 0; i < amplitude; i++)
                {
                    ropePieces[0] = (ropePieces[0].x + movement.x / amplitude, 
                                    ropePieces[0].y + movement.y / amplitude);

                    for (int j = 0; j < ropePieces.Count - 1; j++)
                    {
                        var head = ropePieces[j];
                        var tail = ropePieces[j + 1];

                        if (NotAdjacent(head, tail))
                        {
                            (int x, int y) diff = (head.x - tail.x, head.y - tail.y);

                            tail.x += Math.Sign(diff.x);
                            tail.y += Math.Sign(diff.y);
                        }

                        ropePieces[j] = head;
                        ropePieces[j + 1] = tail;
                    }

                    if (!visited.Contains(ropePieces.Last()))
                        visited.Add(ropePieces.Last());
                }
            }

            Console.WriteLine("Visited " + visited.Count);

        }


        private static void DrawStep((int x, int y) head, (int x, int y) tail)
        {
            StringBuilder sb = new();
            for (int i = 4; i >= 0; i--)
            {
                for (int j = 0; j <= 5; j++)
                {
                    if (i == 0 && j == 0)
                        sb.Append("s");
                    else if (i == head.y && j == head.x)
                        sb.Append("H");
                    else if (i == tail.y && j == tail.x)
                        sb.Append("T");
                    else
                        sb.Append(".");
                }
                sb.Append("\n");
            }

            Console.WriteLine(sb.ToString());
        }

        private static void Draw(List<(int x, int y)> visited)
        {
            StringBuilder sb = new();
            for (int i = visited.Max(v => v.y); i >= visited.Min(v =>v.y); i--)
            {
                for (int j = visited.Min(v => v.x); j <= visited.Max(v => v.x); j++)
                {
                    if (i == 0 && j == 0)
                        sb.Append("s");
                    else if (visited.Contains((j, i)))
                        sb.Append("#");
                    else
                        sb.Append(".");
                }
                sb.Append("\n");
            }

            Console.WriteLine(sb.ToString());
        }

        private static bool NotAdjacent((int x, int y) head, (int x, int y) tail)
        {
            float ampSqrd = MathF.Pow(head.x - tail.x, 2) + MathF.Pow(head.y - tail.y, 2);
            return ampSqrd > 2;
        }

        private static (int x, int y) ParseCommand(string command)
        {
            (int x, int y) result = (0, 0);
            int amount = int.Parse(command.Split(' ')[1]);
            string dir = command.Split(' ')[0];

            switch (dir)
            {
                case "U": 
                    result.y += amount;
                    break;
                case "D":
                    result.y -= amount;
                    break;
                case "L":
                    result.x -= amount;
                    break;
                case "R":
                    result.x += amount;
                    break;
            }

            return result;
        }

    }
}
