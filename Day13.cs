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
    internal class Packet : IComparable<Packet>
    {
        public int value = -1;
        public List<Packet>? children;
        public int endIndex = -1;
        public bool IsInteger => value >= 0;

        public void Print()
        {
            if (IsInteger)
                Console.Write(value);
            else
            {
                Console.Write("[");
                children.ForEach(p => p.Print());
                Console.Write("]");
            }
        }

        public int CompareTo(Packet? right)
        {
            Packet left = this;
            if (left.IsInteger && right.IsInteger)
            {
                return Math.Sign(right.value - left.value);
            }

            if (left.IsInteger)
            {
                left.children = new() { new Packet() { value = left.value } };
                left.value = -1;
                return left.CompareTo(right);
            }

            if (right.IsInteger)
            {
                right.children = new() { new Packet() { value = right.value } };
                right.value = -1;
                return left.CompareTo(right);
            }

            for (int i = 0; i < left.children!.Count(); i++)
            {
                if (i >= right.children!.Count())
                    return -1;

                int result = left.children![i].CompareTo(right.children![i]);
                if (result != 0)
                    return result;
            }

            if (left.children!.Count() < right.children!.Count())
                return 1;

            return 0;
        }
    }

    internal static class Day13
    {
        
        public static void Solve()
        {
            string[] lines = File.ReadAllLines("input.txt");
            List<Packet> packets = new();
            for (int i = 0; i < lines.Length; i+=3) 
            {
                packets.Add(ParseList(lines[i]));
                packets.Add(ParseList(lines[i + 1]));
            }

            // Add dividers
            Packet div = new Packet();
            div.children = new List<Packet> { new Packet() { children = new List<Packet> { new Packet() { value = 6 } } } };
            packets.Add(div);

            Packet div2 = new Packet();
            div2.children = new List<Packet> { new Packet() { children = new List<Packet> { new Packet() { value = 2 } } } };
            packets.Add(div2);

            var ordered = packets.OrderByDescending(p => p).ToList();

            Console.WriteLine((ordered.IndexOf(div) + 1) * (ordered.IndexOf(div2) + 1));
        }

        private static Packet ParseList(string packetInput)
        {
            Packet thisPacket = new Packet();
            thisPacket.children = new List<Packet>();

            int i = 0;
            while(i < packetInput.Length)
            {
                if (packetInput[i] == '[') 
                {
                    Packet newPacket = ParseList(packetInput[(i+1)..]);
                    thisPacket.children.Add(newPacket);
                    i += newPacket.endIndex + 2;
                    continue;
                }

                if(packetInput[i] == ']')
                {
                    thisPacket.endIndex = i;
                    break;
                }

                int test;
                if (int.TryParse(packetInput[i].ToString(), out test))
                {
                    // To accomodate for > 1 digits in a number:
                    int j = i;
                    while(int.TryParse(packetInput[j].ToString(), out test))
                    {
                        j++;
                    }
                    Packet newPacket = new() { value = int.Parse(packetInput[i..j]) };
                    thisPacket.children.Add(newPacket);
                    i = j;
                    continue;
                }

                i++;
            }

            return thisPacket;
        }

        private static int Compare(Packet left, Packet right)
        {
            if(left.IsInteger && right.IsInteger) {
                return Math.Sign(right.value - left.value);
            }

            if (left.IsInteger)
            {
                left.children = new() { new Packet() { value = left.value } };
                left.value = -1;
                return Compare(left, right);
            }

            if (right.IsInteger)
            {
                right.children = new() { new Packet() { value = right.value } };
                right.value = -1;
                return Compare(left, right);
            }

            for (int i = 0; i < left.children!.Count(); i++)
            {
                if (i >= right.children!.Count())
                    return -1;

                int result = Compare(left.children![i], right.children![i]);
                if (result != 0)
                    return result;
            }

            if (left.children!.Count() < right.children!.Count())
                return 1;

            return 0;
        }
    }
}
