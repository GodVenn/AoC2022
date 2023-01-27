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

    public struct Pos : IEquatable<Pos>
    {
        public int i;
        public int j;

        public Pos(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        public bool Equals(Pos other)
        {
            return i == other.i && j == other.j;
        }
    }

    internal static class Day12
    {
        private static List<(List<Pos> visited, int score)> PossibleMoveCounts = new();
        private static List<Pos> globalVisits = new();
        private static int[][] _grid;
        private static int goalNumber = 'E';
        private static int startNumber = 'S';
        private static Pos startPos;
        private static Pos goalPos;
        public static void Solve()
        {
            string[] lines = File.ReadAllLines("input.txt");
            _grid = lines.Select(x => x.Select(y => (int)y).ToArray()).ToArray();
            startPos.i = _grid.TakeWhile(line => !line.Contains(startNumber)).Count();
            startPos.j = _grid[startPos.i].TakeWhile(n => n != startNumber).Count();

            goalPos.i = _grid.TakeWhile(line => !line.Contains(goalNumber)).Count();
            goalPos.j = _grid[goalPos.i].TakeWhile(n => n != goalNumber).Count();

            _grid[startPos.i][startPos.j] = 'a';
            _grid[goalPos.i][goalPos.j] = 'z';

            //PossibleMoveCounts.Add((new(), int.MaxValue));
            //FindPath(startPos.i, startPos.j, 0, new List<Pos>());
            List<List<Pos>> possiblePaths = new List<List<Pos>>();
            List<Pos> startPoints = new List<Pos>();
            for (int i = 0; i < _grid.Length; i++)
            {
                for (int j = 0; j < _grid[0].Length; j++)
                {
                    if (_grid[i][j] == 'a')
                        startPoints.Add(new Pos(i,j));

                }

            }

            foreach (var pos in startPoints)
            {
                startPos = pos;
                List<Pos> path = A_Star();
                if(path.Count > 0)
                    possiblePaths.Add(path);
            }
            var shortest = possiblePaths.OrderBy(p => p.Count).First();
            DrawPath(shortest);
            Console.WriteLine(shortest.Count - 1);
        }

        private static List<Pos> Reconstruct(Dictionary<Pos, Pos> cameFrom, Pos current)
        {
            List<Pos> path = new List<Pos>() { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }

        private static List<Pos> A_Star()
        {
            List<Pos> Open = new List<Pos>() { startPos };
            Dictionary<Pos, Pos> cameFrom = new ();
            Dictionary<Pos, float> gScore = new ();
            gScore[startPos] = 0;
            Dictionary<Pos, float> fScore = new ();
            fScore[startPos] = Heuristic(startPos);

            while (Open.Count> 0)
            {
                Pos current = Open.OrderBy(p => fScore[p]).First();
                if (current.Equals(goalPos))
                    return Reconstruct(cameFrom, current);

                Open.Remove(current);
                foreach (Pos neighbor in GetNeighbors(current))
                {
                    if(!gScore.ContainsKey(neighbor))
                        gScore[neighbor] = float.MaxValue;

                    float tentativeGScore = gScore[current] + 1;
                    if(tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + Heuristic(neighbor);
                        if(!Open.Contains(neighbor))
                            Open.Add(neighbor);
                    }
                }

            }

            return new List<Pos>();
        }

        private static float Heuristic(Pos pos)
        {
            return 0.7f * Math.Abs(_grid[pos.i][pos.j] - _grid[goalPos.i][goalPos.j]) + 0.3f * (Math.Abs(pos.i - goalPos.i) + Math.Abs(pos.j - goalPos.j));
        }

        private static List<Pos> GetNeighbors(Pos node)
        {
            List<Pos> possibleMoves = new();

            int currentHeight = _grid[node.i][node.j];
            int i = node.i;
            int j = node.j;
            int iNext;
            int jNext;
            if (i > 0)
            {
                iNext = i - 1;
                jNext = j;
                if (CanMove(currentHeight, _grid[iNext][jNext]))
                    possibleMoves.Add(new Pos(iNext, jNext));
            }

            if (i < _grid.Length - 1)
            {
                iNext = i + 1;
                jNext = j;
                if (CanMove(currentHeight, _grid[iNext][jNext]))
                    possibleMoves.Add(new Pos(iNext, jNext));
            }

            if (j > 0)
            {
                iNext = i;
                jNext = j - 1;
                if (CanMove(currentHeight, _grid[iNext][jNext]))
                    possibleMoves.Add(new Pos(iNext, jNext));
            }

            if (j < _grid[0].Length - 1)
            {
                iNext = i;
                jNext = j + 1;
                if (CanMove(currentHeight, _grid[iNext][jNext]))
                    possibleMoves.Add(new Pos(iNext, jNext));
            }

            return possibleMoves;
        }

        private static bool CanMove(int current, int next)
        {
            if (next > current + 1 )//|| next < current)
                return false;
            return true;
        }

        private static void FindPath(int i, int j, int depth, List<Pos> visited)
        {
            DrawPath(visited);

            if (depth >= PossibleMoveCounts.Min(x => x.score))
                return;

            int current = _grid[i][j];
            if (i == goalPos.i && j == goalPos.j)
            {
                PossibleMoveCounts.Add((visited, depth));
                Console.WriteLine("Found one possible path: " + depth);
                DrawPath(visited);
                return;
            }

            List<Pos> possibleMoves = new();
            visited.Add(new Pos(i, j));
            globalVisits.Add(new Pos(i, j));

            int iNext;
            int jNext;
            if (i > 0)
            {
                iNext = i - 1;
                jNext = j;
                if (CanMove(current, _grid[iNext][jNext]))
                    possibleMoves.Add(new Pos(iNext, jNext));
            }

            if (i < _grid.Length - 1)
            {
                iNext = i + 1;
                jNext = j;
                if (CanMove(current, _grid[iNext][jNext]))
                    possibleMoves.Add(new Pos(iNext, jNext));
            }

            if (j > 0)
            {
                iNext = i;
                jNext = j - 1;
                if (CanMove(current, _grid[iNext][jNext]))
                    possibleMoves.Add(new Pos(iNext, jNext));
            }

            if (j < _grid[0].Length - 1)
            {
                iNext = i;
                jNext = j + 1;
                if (CanMove(current, _grid[iNext][jNext]))
                    possibleMoves.Add(new Pos(iNext, jNext));
            }

            foreach(var move in possibleMoves.Except(globalVisits).OrderByDescending(m => _grid[m.i][m.j]).ThenBy(m => Math.Abs(m.i - goalPos.i) + Math.Abs(m.j - goalPos.j)))
            {
                FindPath(move.i, move.j, depth + 1, new List<Pos> (visited));
            }
        }

        private static void DrawPath(List<Pos> visited)
        {
            StringBuilder sb = new();
            for (int i = 0; i < _grid.Length; i++)
            {
                for (int j = 0; j < _grid[0].Length; j++)
                {
                    Pos currentPos = new(i, j);
                    if (currentPos.Equals(startPos))
                        sb.Append("S");
                    else if (currentPos.Equals(goalPos))
                        sb.Append("E");
                    else if (visited.Contains(currentPos))
                    {
                        int index = visited.IndexOf(currentPos);
                        if (index >= visited.Count - 1)
                            sb.Append("#");
                        else
                        {
                            Pos next = visited[index + 1];
                            if (next.i > i)
                                sb.Append("v");
                            else if (next.i < i)
                                sb.Append("^");
                            else if (next.j > j)
                                sb.Append(">");
                            else
                                sb.Append("<");
                        }
                    }
                    else
                        sb.Append((char)_grid[i][j]);
                }
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
