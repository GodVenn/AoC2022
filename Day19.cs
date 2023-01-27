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
    internal static class Day19
    {
       
        private static int currentBestGeodeCount;
        struct Blueprint
        {
            public int OreRobotCost;
            public int ClayRobotCost;
            public (int ore, int clay) ObsidianRobotCost;
            public (int ore, int obsidian) GeodeRobotCost;
            public int MaxOreProd => Math.Max(ClayRobotCost, Math.Max(ObsidianRobotCost.ore, GeodeRobotCost.ore));
        }

        struct State
        {
            public int oreCount, clayCount, obsidianCount, geodeCount, oreRobotCount, clayRobotCount, obsidianRobotCount, geodeRobotCount, minutesLeft;
        }

        public static void Solve()
        {
            List<Blueprint> blueprints = new();
            List<int> blueprintQualityLevels = new();
            // PARSE
            string[] lines = File.ReadAllLines("input.txt");
            foreach (string line in lines)
            {
                string[] components = line.Split(' ');
                Blueprint blueprint = new();
                blueprint.OreRobotCost = int.Parse(components[6]);
                blueprint.ClayRobotCost = int.Parse(components[12]);
                blueprint.ObsidianRobotCost = (int.Parse(components[18]), int.Parse(components[21]));
                blueprint.GeodeRobotCost = (int.Parse(components[27]), int.Parse(components[30]));
                blueprints.Add(blueprint);
            }

            int geodeMultiplication = 1;
            for (int i = 0; i < 3; i++)
            {
                int geodeCount = FindBestGeodeCount(blueprints[i]);
                geodeMultiplication *= geodeCount;
                blueprintQualityLevels.Add((i + 1) * geodeCount);
                Console.WriteLine($"Blueprint {i+1}: Geode count = {geodeCount}");
            }

            Console.WriteLine("Blueprint quality sum: " + blueprintQualityLevels.Sum());
            Console.WriteLine($"Geode multiply: {geodeMultiplication}");

        }

        private static int FindBestGeodeCount(Blueprint blueprint)
        {
            State newState = new();
            newState.oreRobotCount = 1;
            newState.minutesLeft = 32;
            currentBestGeodeCount = 0;
            NextMinute(blueprint, newState);
            return currentBestGeodeCount;
        }

        private static void NextMinute(Blueprint blueprint, State previousState)
        {

            // Produce
            State CurrentState = previousState;
            CurrentState.minutesLeft--;
            CurrentState.oreCount += CurrentState.oreRobotCount;
            CurrentState.clayCount += CurrentState.clayRobotCount;
            CurrentState.obsidianCount += CurrentState.obsidianRobotCount;
            CurrentState.geodeCount += CurrentState.geodeRobotCount;

            if(CurrentState.minutesLeft <= 0)
            {
                if (CurrentState.geodeCount > currentBestGeodeCount)
                    currentBestGeodeCount = CurrentState.geodeCount;
                return;
            }

            // Explore choices

            // Build Geode Robot
            if (previousState.oreCount >= blueprint.GeodeRobotCost.ore && previousState.obsidianCount >= blueprint.GeodeRobotCost.obsidian)
            {
                State NextState = CurrentState;
                NextState.oreCount -= blueprint.GeodeRobotCost.ore;
                NextState.obsidianCount -= blueprint.GeodeRobotCost.obsidian;
                NextState.geodeRobotCount++;
                NextMinute(blueprint, NextState);

                // If can produce geode robot, not worth to try other paths (?)
                return;
            }

            // Build Obsidian Robot
            if (previousState.oreCount >= blueprint.ObsidianRobotCost.ore && previousState.clayCount >= blueprint.ObsidianRobotCost.clay && previousState.obsidianRobotCount < blueprint.GeodeRobotCost.obsidian)
            {
                State NextState = CurrentState;
                NextState.oreCount -= blueprint.ObsidianRobotCost.ore;
                NextState.clayCount -= blueprint.ObsidianRobotCost.clay;
                NextState.obsidianRobotCount++;
                NextMinute(blueprint, NextState);

                // If can produce obsidian robot, not worth to try other paths (?)
                return;
            }

            // Save resources
            NextMinute(blueprint, CurrentState);

            // Build Ore Robot
            if(previousState.oreCount >= blueprint.OreRobotCost && previousState.oreRobotCount < blueprint.MaxOreProd)
            {
                State NextState = CurrentState;
                NextState.oreCount -= blueprint.OreRobotCost;
                NextState.oreRobotCount++;
                NextMinute(blueprint, NextState);
            }

            // Build Clay Robot
            if (previousState.oreCount >= blueprint.ClayRobotCost && previousState.clayRobotCount < blueprint.ObsidianRobotCost.clay)
            {
                State NextState = CurrentState;
                NextState.oreCount -= blueprint.ClayRobotCost;
                NextState.clayRobotCount++;
                NextMinute(blueprint, NextState);
            }



        }
      
    }
}
