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
    internal struct Vec2 : IEquatable<Vec2>
    {
        public int x;
        public int y;

        public Vec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Vec2 other)
        {
            return x == other.x && y == other.y;
        }

        public static bool operator ==(Vec2 lhs, Vec2 rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Vec2 operator +(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static bool operator !=(Vec2 lhs, Vec2 rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object? obj)
        {
            return obj is Vec2 && Equals((Vec2)obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    internal class StraightLine
    {
        private Vec2 point1, point2;

        public int MinX => Math.Min(point1.x, point2.x);
        public int MaxX => Math.Max(point1.x, point2.x);
        public int MinY => Math.Min(point1.y, point2.y);
        public int MaxY => Math.Max(point1.y, point2.y);

        public bool ContainsPoint(Vec2 point)
        {
            if(point.x == point1.x && (MinY <= point.y && point.y <= MaxY))
                return true;

            if (point.y == point1.y && (MinX <= point.x && point.x <= MaxX))
                return true;

            return false;
        }

        public StraightLine(Vec2 point1, Vec2 point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }
    }

    internal static class Day14
    {

        private static List<StraightLine> RockPatterns = new List<StraightLine>();
        private static List<Vec2> SandPoints = new List<Vec2>();
        private static int BottomLine;
        private static int sandCount = 0;

        public static void Solve()
        {
            var start = DateTime.Now;
            string[] lines = File.ReadAllLines("input.txt");
            BuildRock(lines);
            BottomLine = RockPatterns.Max(l => l.MaxY) + 2;


            bool FallingToVoid = false;
            while(!FallingToVoid)
            {
                FallingToVoid = !DropSand(0);
            }

            //Draw();
            Console.WriteLine(sandCount);
            Console.WriteLine("Time spent: " + DateTime.Now.Subtract(start).Minutes + ":" + DateTime.Now.Subtract(start).Seconds);
        }

        private static void Draw()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= SandPoints.Max(l => l.y) + 1; i++)
            {
                sb.AppendLine();
                for (int j = SandPoints.Min(l => l.x) - 1; j <= SandPoints.Max(l => l.x) + 1; j++)
                {
                    Vec2 Point = new Vec2(j, i);
                    if (SandPoints.Contains(Point))
                        sb.Append('o');
                    else if (PointIsInRock(Point))
                        sb.Append('#');
                    else if (i == 0 && j == 500)
                        sb.Append('+');
                    else
                        sb.Append('.');
                }
            }
            Console.WriteLine(sb.ToString());
        }

        private static void BuildRock(string[] lines)
        {
            foreach (string line in lines)
            {
                var points = line.Trim().Split("->");
                for (int i = 0; i < points.Length - 1; i++)
                {
                    var coords1 = points[i].Split(',').Select(x => int.Parse(x)).ToArray();
                    var coords2 = points[i+1].Split(',').Select(x => int.Parse(x)).ToArray();
                    RockPatterns.Add(new StraightLine(new Vec2(coords1[0], coords1[1]), new Vec2(coords2[0], coords2[1])));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxY">
        /// If the sand falls below this Y it falls forever
        /// </param>
        /// <returns>
        /// A bool for wheter or not the sand landed anywhere
        /// </returns>
        private static bool DropSand(int maxY)
        {
            Vec2 Pos = new Vec2(500, 0);
            bool moving = true;
            while (moving)
            {
                //if (Pos.y >= maxY)
                //    return false;

                Vec2 TestPos = Pos + new Vec2(0, 1);
                if (PointIsFree(TestPos))
                {
                    Pos = TestPos;
                    continue;
                }

                TestPos = Pos + new Vec2(-1,1);
                if (PointIsFree(TestPos))
                {
                    Pos = TestPos;
                    continue;
                }

                TestPos = Pos + new Vec2(1,1);
                if (PointIsFree(TestPos))
                {
                    Pos = TestPos;
                    continue;
                }

                // No points to fall in, movement stopped
                moving = false;
            }

            AddSand(Pos);

            if (Pos == new Vec2(500, 0))
                return false;

            return true;
        }

        private static void AddSand(Vec2 pos)
        {
            sandCount++;
            SandPoints.Add(pos);

            // Optimization for large amounts of sand: Remove them from collision check if they are covered above
            bool left = !PointIsFree(pos + new Vec2(-1, 0));
            bool twoLeft = !PointIsFree(pos + new Vec2(-2, 0));
            bool right = !PointIsFree(pos + new Vec2(1, 0));
            bool twoRight = !PointIsFree(pos + new Vec2(2, 0));
            if (left && twoLeft)
                SandPoints.Remove(pos + new Vec2(-1, 1));

            if (left && right)
                SandPoints.Remove(pos + new Vec2(0, 1));

            if (right && twoRight)
                SandPoints.Remove(pos + new Vec2(1, 1));
        }

        private static bool PointIsFree(Vec2 pos)
        {
            return !PointIsInRock(pos) && !SandPoints.Contains(pos);
        }

        private static bool PointIsInRock(Vec2 pos)
        {
            if(pos.y == BottomLine)
                return true;

            foreach (var rockLine in RockPatterns)
            {
                if (rockLine.ContainsPoint(pos))
                    return true;
            }

            return false;
        }

    }
}
