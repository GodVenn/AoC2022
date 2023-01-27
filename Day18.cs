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
    internal static class Day18
    {
        internal struct Vec3 : IEquatable<Vec3>
        {
            public int x;
            public int y;
            public int z;



            public Vec3(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public Vec3(string vec)
            {
                var comps = vec.Split(',');
                x = int.Parse(comps[0]);
                y = int.Parse(comps[1]);
                z = int.Parse(comps[2]);
            }

            public override string ToString()
            {
                return "[" + x + "," + y + "," + z + "]";
            }

            public bool Equals(Vec3 other)
            {
                return x == other.x && y == other.y && z == other.z;
            }

            public static bool operator ==(Vec3 lhs, Vec3 rhs)
            {
                return lhs.Equals(rhs);
            }

            public static Vec3 operator +(Vec3 lhs, Vec3 rhs)
            {
                return new Vec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
            }

            public static bool operator !=(Vec3 lhs, Vec3 rhs)
            {
                return !lhs.Equals(rhs);
            }

            public override bool Equals(object? obj)
            {
                return obj is Vec3 && Equals((Vec3)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(x, y, z);
            }
        }

        public static void Solve()
        {
            // PARSE
            Vec3[] DropParts = File.ReadAllLines("input.txt").Select(l => new Vec3(l)).ToArray();
            Dictionary<Vec3, int> DropWithSideCount = new Dictionary<Vec3, int>();
            foreach (Vec3 p in DropParts)
            {
                DropWithSideCount[p] = 6;
            }

            // Remove shared sides
            //for (int i = 0; i < DropParts.Length; i++)
            //{
            //    var current = DropParts[i];
            //    for (int j = i + 1; j < DropParts.Length; j++)
            //    {
            //        var other = DropParts[j];

            //        int diff1 = Math.Abs(other.x - current.x);
            //        int diff2 = Math.Abs(other.y - current.y);
            //        int diff3 = Math.Abs(other.z - current.z);

            //        // Remove shared sides
            //        if (diff1 + diff2 + diff3 == 1)
            //        {
            //            DropWithSideCount[current]--;
            //            DropWithSideCount[other]--;
            //        }
            //    }
            //}

            // Remove shared and blocked sides
            for (int i = 0; i < DropParts.Length; i++)
            {
                var current = DropParts[i];
                var neighbors = new List<Vec3>
                {
                    current + new Vec3(1, 0, 0),
                    current + new Vec3(-1, 0, 0),
                    current + new Vec3(0, 1, 0),
                    current + new Vec3(0, -1, 0),
                    current + new Vec3(0, 0, 1),
                    current + new Vec3(0, 0, -1)
                };

                foreach (Vec3 p in neighbors)
                {
                    if (!CanEscape(p, new List<Vec3>(), DropParts))
                        DropWithSideCount[current]--;
                }
            }




            Console.WriteLine(DropWithSideCount.Sum(kv => kv.Value));
           
        }

        private static bool CanEscape(Vec3 currentPoint, List<Vec3> visited, Vec3[] DropParts)
        {
            if (DropParts.Contains(currentPoint))
                return false;

            visited.Add(currentPoint);

            var neighbors = new List<Vec3>
            {
                currentPoint + new Vec3(1, 0, 0),
                currentPoint + new Vec3(-1, 0, 0),
                currentPoint + new Vec3(0, 1, 0),
                currentPoint + new Vec3(0, -1, 0),
                currentPoint + new Vec3(0, 0, 1),
                currentPoint + new Vec3(0, 0, -1)
            };

            foreach (Vec3 p in neighbors.Except(DropParts).Except(visited))
            {
                if (HasAnyFreePath(p, DropParts))
                    return true;
                else if(CanEscape(p, visited, DropParts)) 
                    return true;
            }

            return false;
        }

        private static bool HasAnyFreePath(Vec3 point, Vec3[] DropParts)
        {
            var current = point;
            int x1 = 0;
            int x2 = 0;
            int y1 = 0;
            int y2 = 0;
            int z1 = 0;
            int z2 = 0;
            for (int j = 0; j < DropParts.Length; j++)
            {
                var other = DropParts[j];

                int diff1 = Math.Abs(other.x - current.x);
                int diff2 = Math.Abs(other.y - current.y);
                int diff3 = Math.Abs(other.z - current.z);

                // If no shared coordinate
                if (diff1 * diff2 * diff3 != 0)
                    continue;

                if (diff1 == 0 && diff2 == 0)
                {
                    if (other.z > current.z)
                        z2 = 1;
                    else
                        z1 = 1;
                }
                else if (diff1 == 0 && diff3 == 0)
                {
                    if (other.y > current.y)
                        y2 = 1;
                    else
                        y1 = 1;
                }
                else if (diff2 == 0 && diff3 == 0)
                {
                    if (other.x > current.x)
                        x2 = 1;
                    else
                        x1 = 1;
                }
            }

            return x1 * x2 * y1 * y2 * z1 * z2 == 0;

        }
       
    }
}
