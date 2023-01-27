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
using System.Runtime.CompilerServices;

namespace AoC2022_NoUnity
{
    internal static class Day17
    {
        internal struct Vec2Long : IEquatable<Vec2Long>
        {
            public long x;
            public long y;

            public Vec2Long(long x, long y)
            {
                this.x = x;
                this.y = y;
            }

            public bool Equals(Vec2Long other)
            {
                return x == other.x && y == other.y;
            }

            public static bool operator ==(Vec2Long lhs, Vec2Long rhs)
            {
                return lhs.Equals(rhs);
            }

            public static Vec2Long operator +(Vec2Long lhs, Vec2Long rhs)
            {
                return new Vec2Long(lhs.x + rhs.x, lhs.y + rhs.y);
            }

            public static bool operator !=(Vec2Long lhs, Vec2Long rhs)
            {
                return !lhs.Equals(rhs);
            }

            public override bool Equals(object? obj)
            {
                return obj is Vec2Long && Equals((Vec2Long)obj);
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        private struct Rock
        {
            public Vec2Long[] Points { get; set; }
            public long BottomY { get; private set; } = 0;
            public long Height;

            public bool Collision(Vec2Long point)
            {
                return Points.Contains(point);
            }

            public void Move(Vec2Long movement)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i] += movement;
                }
                BottomY += movement.y;
            }

            public Rock(Rock rock)
            {
                Points = new Vec2Long[rock.Points.Length];
                rock.Points.CopyTo(Points, 0);
                BottomY= 0;
                Height = rock.Height;
            }
        }

        private static List<int> GetValue<TK, List>(this IDictionary<TK, List<int>> dict, TK key)
        {
            List<int> value;
            return dict.TryGetValue(key, out value) ? value : new();
        }

        public static void Solve()
        {
            Vec2Long Down = new Vec2Long(0,-1);
            Vec2Long Left = new Vec2Long(-1,0);
            Vec2Long Right = new Vec2Long(1,0);

            // PARSE
            char[] JetChars = File.ReadAllText("input.txt").ToArray();
            Vec2Long[] Jets = JetChars.Select(c => c == '<' ? Left : Right).ToArray();
            List<Rock> RockTypes = InitializeRockTypes();


            long rockCount = 0;
            int rockIndex = 0;
            int jetIndex = 0;
            int bottom = 0;
            SortedDictionary<long,List<int>> Ground = new ()
            {
                {0, new () { 0,1,2,3,4,5,6 } }
            };
            
            Vec2Long StartPos = new Vec2Long(2, 5);
            while (rockCount < 2022)//1000000000000)
            {
                Rock currentRock = new Rock(RockTypes[rockIndex]);
                currentRock.Move(StartPos);

                Console.WriteLine();
                Console.WriteLine("/////////////////////////////////");
                Console.WriteLine();
                Draw(currentRock, Ground);
                for (long y = StartPos.y - 1; y >= 0; y--)
                {
                    List<int> current = Ground.GetValue<long, List<int>>(y);
                    if (!CanMoveDown(currentRock, current))
                    {
                        long newHeight = currentRock.Height + y + 1 + 4;
                        if (newHeight > StartPos.y)
                            StartPos.y = newHeight;
                        break;
                    }

                    currentRock.Move(Down);
                    Draw(currentRock, Ground);


                    List<int> aboveOne = Ground.GetValue<long, List<int>>(y + 1);
                    List<int> aboveTwo = Ground.GetValue<long, List<int>>(y + 2);
                    if (CanMoveSideways(currentRock, new() { current, aboveOne, aboveTwo }, Jets[jetIndex]))
                        currentRock.Move(Jets[jetIndex]);

                    Draw(currentRock, Ground);
                    jetIndex++;
                    if (jetIndex >= Jets.Length)
                        jetIndex = 0;
                }

                bottom += AddRockToGround(currentRock, Ground);

                rockCount++;
                rockIndex++;
                if (rockIndex >= RockTypes.Count)
                    rockIndex = 0;
            }

            Console.WriteLine(Ground.Keys.Last());
        }

        private static List<Rock> InitializeRockTypes()
        {
            return new List<Rock>()
            {
                new Rock { Height = 1, Points = new Vec2Long[]{ new Vec2Long(0, 0), new Vec2Long(1, 0), new Vec2Long(2, 0), new Vec2Long(3, 0)} },
                new Rock { Height = 3, Points = new Vec2Long[]{ new Vec2Long(1, 0), new Vec2Long(0, 1), new Vec2Long(1, 1), new Vec2Long(2, 1), new Vec2Long(1, 2) } },
                new Rock { Height = 3, Points = new Vec2Long[]{ new Vec2Long(0, 0), new Vec2Long(1, 0), new Vec2Long(2, 0), new Vec2Long(2, 1), new Vec2Long(2, 2) } },
                new Rock { Height = 4, Points = new Vec2Long[]{ new Vec2Long(0, 0), new Vec2Long(0, 1), new Vec2Long(0, 2), new Vec2Long(0, 3)} },
                new Rock { Height = 2, Points = new Vec2Long[]{ new Vec2Long(0, 0), new Vec2Long(1, 0), new Vec2Long(0, 1), new Vec2Long(1, 1)} }
            };
        }

        private static int AddRockToGround(Rock rock, SortedDictionary<long, List<int>> ground)
        {
            foreach (var point in rock.Points)
            {
                ground[point.y] = ground.GetValue<long, List<int>>(point.y);
                ground[point.y].Add((int)point.x);
            }
            //int diff = ground.Count - 100;
            //if(diff > 0)
            //{
            //    for (int i = 0; i < diff; i++)
            //    {
            //        ground.Remove(ground.Keys.ElementAt(i));
            //    }
            //    return diff;
            //}
            return 0;
        }

        private static bool CanMoveDown(Rock rock, List<int> nextGroundLevel)
        {
            if(nextGroundLevel.Count == 0) 
                return true;

            if (rock.Points.Any(p => p.y == rock.BottomY && nextGroundLevel.Contains((int)p.x)))
                return false;

            return true;
        }

        private static bool CanMoveSideways(Rock rock, List<List<int>> currentGroundLevels, Vec2Long Movement)
        {
            for (int i = 0; i < currentGroundLevels.Count; i++)
            {
                var groundLevel = currentGroundLevels[i];

                foreach (var point in rock.Points.Where(p => p.y == rock.BottomY + i))
                {
                    int x = (int)point.x + (int)Movement.x;
                    if (x == -1 || x == 7 || groundLevel.Contains(x))
                        return false;
                }
            }

            return true;
        }

        private static void Draw(Rock rock, SortedDictionary<long, List<int>> ground)
        {
            return;
            StringBuilder sb = new StringBuilder();

            for (long i = rock.Points.Max(p =>p.y); i >= 0; i--)
            {
                sb.Append('|');
                for (int j = 0; j < 7; j++)
                {
                    var point = new Vec2Long(j, i);
                    if (rock.Points.Contains(point))
                        sb.Append('@');
                    else if (ground.ContainsKey(i) && ground[i].Contains(j))
                        sb.Append('#');
                    else
                        sb.Append('.');
                }
                sb.Append('|');
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
