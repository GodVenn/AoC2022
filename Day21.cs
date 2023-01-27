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
    internal static class Day21
    {
       
    
        public static void Solve()
        {
            // PARSE
            Dictionary<string,string> lines = File.ReadAllLines("input.txt").ToDictionary(l => l.Split(':')[0]);
            foreach (var line in lines)
            {
                lines[line.Key] = line.Value.Split(':')[1];
            }
            long? root1 = ResultOf(lines["root"].Split(' ')[1], lines);
            long? root2 = ResultOf(lines["root"].Split(' ')[3], lines);
            long target = root1 is null ? root2.Value : root1.Value;
            string targetName = root1 is null ? lines["root"].Split(' ')[1] : lines["root"].Split(' ')[3];

            
            Console.WriteLine(ReverseEquation("humn", target, targetName, lines));
        }

        public static long? ReverseEquation(string targetVariable, long targetValue, string name, Dictionary<string, string> lines)
        {
            if (name == targetVariable)
                return null;

            string monkeyYelledWhat = lines[name];
            long result;
            if (long.TryParse(monkeyYelledWhat, out result))
                return result;

            var components = monkeyYelledWhat.Split(' ');
            long? v1 = ResultOf(components[1], lines);
            long? v2 = ResultOf(components[3], lines);

            if (v1 is null)
            {
                if (components[2] == "+")
                {
                    targetValue -= v2.Value;
                    if (components[1] == targetVariable)
                        return targetValue;
                    return ReverseEquation(targetVariable, targetValue, components[1], lines);
                }

                if (components[2] == "-")
                {
                    targetValue += v2.Value;
                    if (components[1] == targetVariable)
                        return targetValue;
                    return ReverseEquation(targetVariable, targetValue, components[1], lines);
                }

                if (components[2] == "/")
                {
                    targetValue *= v2.Value;
                    if (components[1] == targetVariable)
                        return targetValue;
                    return ReverseEquation(targetVariable, targetValue, components[1], lines);
                }

                if (components[2] == "*")
                {
                    targetValue /= v2.Value;
                    if (components[1] == targetVariable)
                        return targetValue;
                    return ReverseEquation(targetVariable, targetValue, components[1], lines);
                }
            }
            else
            {


                if (components[2] == "+")
                {
                    targetValue -= v1.Value;
                    if (components[3] == targetVariable)
                        return targetValue;
                    return ReverseEquation(targetVariable, targetValue, components[3], lines);
                }

                if (components[2] == "-")
                {
                    targetValue = v1.Value - targetValue;
                    if (components[3] == targetVariable)
                        return targetValue;
                    return ReverseEquation(targetVariable, targetValue, components[3], lines);
                }

                if (components[2] == "/")
                {
                    targetValue = v1.Value / targetValue;
                    if (components[3] == targetVariable)
                        return targetValue;
                    return ReverseEquation(targetVariable, targetValue, components[3], lines);
                }

                if (components[2] == "*")
                {
                    targetValue /= v1.Value;
                    if (components[3] == targetVariable)
                        return targetValue;
                    return ReverseEquation(targetVariable, targetValue, components[3], lines);
                }
            }

            throw new Exception("Wrong");

        }

        public static long? ResultOf(string name, Dictionary<string, string> lines)
        {
            if (name == "humn")
                return null;

            string monkeyYelledWhat = lines[name];
            long result;
            if (long.TryParse(monkeyYelledWhat, out result))
                return result;

            var components = monkeyYelledWhat.Split(' ');
            long? v1 = ResultOf(components[1], lines);
            long? v2 = ResultOf(components[3], lines);

            if (v1 is null || v2 is null)
                return null;

            if (components[2] == "+")
                return v1 + v2;

            if (components[2] == "-")
                return v1 - v2;

            if (components[2] == "/")
                return v1 / v2;

            if (components[2] == "*")
                return v1 * v2;

            throw new Exception("wrong");
        }
    }
}
