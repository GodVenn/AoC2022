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
    internal static class Day15
    {
        internal class Sensor
        {
            public Vec2 SensorPos;
            public Vec2 BeaconPos;

            public int ScanRadius => Manhattan(SensorPos, BeaconPos);

        }

        private static int Manhattan(Vec2 p1, Vec2 p2)
        {
            return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
        }

        public static void Solve()
        {
            // PARSE
            List<Sensor> Sensors = new List<Sensor>();
            var start = DateTime.Now;
            string[] lines = File.ReadAllLines("input.txt");
            foreach (var line in lines)
            {
                var components = line.Split(' ');
                Vec2 SensorPos = new Vec2(int.Parse(components[2].Split('=')[1].Split(',')[0]), int.Parse(components[3].Split('=')[1].Split(':')[0]));
                Vec2 BeaconPos = new Vec2(int.Parse(components[8].Split('=')[1].Split(',')[0]), int.Parse(components[9].Split('=')[1].Split(',')[0]));
                Sensors.Add(new Sensor() { BeaconPos= BeaconPos, SensorPos = SensorPos });
            }


            // SOLVE
            int maxSearch = 4000000;
            Vec2 DistressSignal= new Vec2(0, 0);
            for (int y = 0; y <= maxSearch; y++)
            {
                var rangesOccupied = OccupiedRanges(Sensors, y, maxSearch);
                if (rangesOccupied.Count > 1 || rangesOccupied.First() != (0, maxSearch))
                {
                    var range = rangesOccupied.FirstOrDefault(r => r.min == 0);
                    int targetX = range == (0,0) ? 0 : range.max + 1;
                    DistressSignal = new Vec2(targetX, y);
                    break;
                }
            }

            Console.WriteLine($"Signal is at {DistressSignal.x},{DistressSignal.y}");
            Console.WriteLine($"Frequency is {((ulong)DistressSignal.x) * 4000000 + (ulong)DistressSignal.y}");
        }


        private static List<(int min, int max)> OccupiedRanges(List<Sensor> Sensors, int y, int maxSearch)
        {
            List<(int min, int max)> ranges = new List<(int min, int max)>();
            foreach (var sensor in Sensors)
            {
                int radiusAtLine = sensor.ScanRadius - Math.Abs(sensor.SensorPos.y - y);
                if (radiusAtLine < 0)
                    continue;
                int minX = Math.Max(0, sensor.SensorPos.x - radiusAtLine);
                int maxX = Math.Min(sensor.SensorPos.x + radiusAtLine, maxSearch);

                ranges.Add((minX, maxX));

                // Merge ranges
                bool changed = false;
                do
                {
                    changed = false;
                    for (int i = 0; i < ranges.Count; i++)
                    {
                        for (int j = i + 1; j < ranges.Count; j++)
                        {
                            var r1 = ranges[i];
                            var r2 = ranges[j];
                            var merged = MergeRanges(r1, r2);
                            if (merged is not null)
                            {
                                ranges[i] = merged.Value;
                                ranges.RemoveAt(j);
                                changed = true;
                                break;
                            }

                            merged = MergeRanges(r2, r1);
                            if (merged is not null)
                            {
                                ranges[j] = merged.Value;
                                ranges.RemoveAt(i);
                                changed = true;
                                break;
                            }

                        }
                        if(changed) break;
                    }
                } while (changed);

            }
            return ranges;
        }

        private static (int min, int max)? MergeRanges((int min, int max) r1, (int min, int max) r2)
        {
            (int min, int max)? merged = null;

            // Merge r2 into r1
            if (r1.min <= r2.min && r2.min <= r1.max + 1)
            {
                merged = (r1.min, Math.Max(r1.max, r2.max));
            } else if (r1.min - 1 <= r2.max && r2.max <= r1.max)
            {
                merged = (Math.Min(r2.min, r1.min), r1.max);
            }

            return merged;
        }
      
    }
}
