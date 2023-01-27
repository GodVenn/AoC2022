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
    internal static class Day16
    {
        private class Valve
        {
            public string Name;
            public int FlowRate;
            public List<string> Neighbors;
        }

        private static Dictionary<(Valve v1, Valve v2), uint> Distances = new();
        private static List<Valve> Valves = new();
        private static List<Valve> ClosedValves = new();
        private static int maxFlowRate = 0;
        public static void Solve()
        {
            // PARSE
            string[] lines = File.ReadAllLines("input.txt");
            foreach (string line in lines)
            {
                string[] components = line.Split(' ');
                string name = components[1];
                int rate = int.Parse(components[4].Split('=')[1].Split(';')[0]);
                List<string> neighborNames = components[9..].ToList().Select(s => s.Split(',')[0]).ToList();
                Valve valve = new Valve()
                {
                    Name = name,
                    FlowRate = rate,
                    Neighbors = neighborNames
                };
                Valves.Add(valve);
            }


            // Find Distances
            FindDistances();

            // Find best path
            ClosedValves = new List<Valve>(Valves);
            ClosedValves.RemoveAll(valve => valve.FlowRate == 0);

            int TimeLeft = 30;
            int TotalFlow = 0;
            Valve currentValve = Valves.Single(v => v.Name == "AA");

            OpenValves(currentValve, currentValve, 26, 26, ClosedValves, 0);


            Console.WriteLine(maxFlowRate);
          
        }

        private static void OpenValves(Valve CurrentHumanValve, Valve CurrentElephantValve, int HumanTimeLeft, int ElephantTimeLeft, List<Valve> ClosedValves, int totalFlow)
        {
            if(CurrentHumanValve.FlowRate > 0 && HumanTimeLeft > 0)
            {
                // Time spent opening valve
                HumanTimeLeft--;
                // Open valve
                ClosedValves.Remove(CurrentHumanValve);
                totalFlow += HumanTimeLeft * CurrentHumanValve.FlowRate;
            }

            if (CurrentElephantValve.FlowRate > 0 && ElephantTimeLeft > 0)
            {
                // Time spent opening valve
                ElephantTimeLeft--;
                // Open valve
                ClosedValves.Remove(CurrentElephantValve);
                totalFlow += ElephantTimeLeft * CurrentElephantValve.FlowRate;
            }

            if ((HumanTimeLeft <= 0 && ElephantTimeLeft <= 0) || !ClosedValves.Any())
            {
                if(totalFlow > maxFlowRate)
                    maxFlowRate = totalFlow;
                return;
            }

            for (int i = 0; i < ClosedValves.Count; i++)
            {

                for (int j = 0; j < ClosedValves.Count; j++)
                {
                    if (i == j)
                        continue;
                    
                    var nextHumanValve = ClosedValves[i];
                    int humanDistance = (int)GetDistance(CurrentHumanValve, nextHumanValve).Value;
                    
                    var nextElephantValve = ClosedValves[j];
                    int elephantDistance = (int)GetDistance(CurrentElephantValve, nextElephantValve).Value;

                    OpenValves(nextHumanValve, nextElephantValve, HumanTimeLeft - humanDistance, ElephantTimeLeft - elephantDistance, new List<Valve>(ClosedValves), totalFlow);

                    if (ElephantTimeLeft <= 0)
                        break;
                }

                if (HumanTimeLeft <= 0)
                    break;

            }

            // PART 1:
            //foreach (var nextValve in ClosedValves)
            //{
            //    int distance = (int)GetDistance(CurrentValve, nextValve).Value;

            //    foreach (var nextElephantValve in ClosedValves)
            //    {
            //        if (nextValve == nextElephantValve)
            //            continue;

            //        int elephantDistance = (int)GetDistance(CurrentValve, nextElephantValve).Value;

            //        ClosedAnyMore = true;
            //        OpenValves(nextValve, nextElephantValve, TimeLeft - distance, ElephantTimeLeft - elephantDistance, new List<Valve>(ClosedValves), totalFlow);
            //    }
            //}

        }

        private static void FindDistances()
        {
            foreach (var v1 in Valves)
            {
                foreach (var v2 in Valves)
                {
                    if (Distances.ContainsKey((v1, v2)))
                        continue;

                    if (Distances.ContainsKey((v2, v1)))
                        continue;

                    uint distance = FindDistance(v1, v2, new List<string>());
                    Distances[(v1,v2)] = distance;
                }
            }
        }

        private static uint? GetDistance(Valve v1, Valve v2)
        {

            if (Distances.ContainsKey((v1, v2)))
                return Distances[(v1, v2)];

            if (Distances.ContainsKey((v2, v1)))
                return Distances[(v2, v1)];

            return null;
        }

        private static uint FindDistance(Valve current, Valve Goal, List<string> visited)
        {
            visited.Add(current.Name);

            if (current.Name == Goal.Name)
                return 0;

            var DictDistance = GetDistance(current, Goal);
            if (DictDistance.HasValue)
                return DictDistance.Value;

            uint minDistance = uint.MaxValue;
            foreach (string neighbor in current.Neighbors)
            {
                if (visited.Contains(neighbor))
                    continue;

                Valve neighborValve = Valves.Single(v => v.Name == neighbor);

                uint distance = FindDistance(neighborValve, Goal, new List<string>(visited));
                if (distance < minDistance)
                    minDistance = distance;
            }
            return minDistance == uint.MaxValue ? uint.MaxValue : minDistance + 1;
        }
      
    }
}
