// See https://aka.ms/new-console-template for more information
using AoC2022_NoUnity;

Console.WriteLine("Hello, Peter!");


var start = DateTime.Now;
Day22Part2.Solve();
Console.WriteLine("Time spent: " + DateTime.Now.Subtract(start).Minutes + ":" + DateTime.Now.Subtract(start).Seconds + ":" + DateTime.Now.Subtract(start).Milliseconds);
