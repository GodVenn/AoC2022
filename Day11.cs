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

namespace AoC2022_NoUnity
{
    internal class Monkey
    {
        public ulong InspectionCount = 0;
        public List<ulong>? ItemWorryLevels;
        public Func<ulong, ulong>? Inspect;

        public string arg2;
        public string ops;

        public uint DivisionTest;
        public int TrueMonkeyId;
        public int FalseMonkeyId;

        public Monkey() { }

        public int WhereThrow(ulong Item)
        {
            if (Item % DivisionTest == 0)
                return TrueMonkeyId; 
            else 
                return FalseMonkeyId;
        }

        public ulong InspectMethod(ulong Item)
        {
            ulong argNum = Item;
            if (!arg2.Equals("old"))
                argNum = ulong.Parse(arg2);

            if (ops.Equals("*"))
                return Item * argNum;
            else if (ops.Equals("+"))
                return Item + argNum;
            else
                throw new Exception("WHAT IS THIS");
        }
    }

    internal static class Day11
    {
        private static List<Monkey> Monkeys = new List<Monkey>();

        public static void Solve()
        {
            Parse();

            uint lcm = 1;
            Monkeys.ForEach(x => { lcm *= x.DivisionTest; });

            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < Monkeys.Count; j++)
                {
                    Monkey currentMonkey = Monkeys[j];
                    currentMonkey.InspectionCount += (ulong)currentMonkey.ItemWorryLevels!.Count;
                    currentMonkey.ItemWorryLevels = currentMonkey.ItemWorryLevels!.Select(x => currentMonkey.InspectMethod(x) % lcm).ToList();
                    currentMonkey.ItemWorryLevels!.ForEach(x => Monkeys[currentMonkey.WhereThrow(x)].ItemWorryLevels!.Add(x));
                    currentMonkey.ItemWorryLevels = new();
                }
            }

            for (int j = 0; j < Monkeys.Count; j++)
            {
                Monkey currentMonkey = Monkeys[j];
                Console.WriteLine($"Monkey {j} inspected items {currentMonkey.InspectionCount} times.");
            }

            var counts = Monkeys.OrderByDescending(x => x.InspectionCount).Take(2).Select(m => m.InspectionCount).ToList();
            ulong monkeyBusiness = counts[0] * counts[1];

            Console.WriteLine($"Monkey business: {monkeyBusiness}");
        }

        private static void Parse()
        {
            string[] lines = File.ReadAllLines("input.txt");
            for (int i = 0; i < lines.Count(); i += 7)
            {
                Monkey monkey= new Monkey();

                var items = lines[i + 1].Split(":")[1].Split(",");
                monkey.ItemWorryLevels = items.Select(x => ulong.Parse(x)).ToList();

                monkey.Inspect = ParseFunction(lines[i + 2]);

                var components = lines[i + 2].Split("=")[1].Split(" ");
                monkey.ops = components[2];
                monkey.arg2 = components[3];

                monkey.DivisionTest = uint.Parse(lines[i + 3].Split(" ").Last());
                monkey.TrueMonkeyId = int.Parse(lines[i + 4].Split(" ").Last());
                monkey.FalseMonkeyId = int.Parse(lines[i + 5].Split(" ").Last());

                Monkeys.Add(monkey);
            }
        }

        private static Func<ulong, ulong> ParseFunction(string functionString)
        {
            var components = functionString.Split("=")[1].Split(" ");
            string operation = components[2];
            string secondArgument = components[3];

            ParameterExpression arg1 = Expression.Parameter(typeof(ulong), "old");
            Expression arg2;
            ulong argInt;
            if (ulong.TryParse(secondArgument, out argInt))
                arg2 = Expression.Constant(argInt, typeof(ulong));
            else
                arg2 = arg1;

            Expression operationExpr = operation switch
            {
                "*" => Expression.Multiply(arg1, arg2),
                "+" => Expression.Add(arg1, arg2),
                "-" => Expression.Subtract(arg1, arg2),
                "/" => Expression.Divide(arg1, arg2)
            };

            Expression<Func<ulong, ulong>> funcExpr = Expression.Lambda<Func<ulong, ulong>>(operationExpr, new ParameterExpression[] { arg1 });

            return funcExpr.Compile();
        }

    }
}
