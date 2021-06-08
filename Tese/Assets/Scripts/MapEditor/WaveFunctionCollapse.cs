using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour, IWFC
{
    public bool WFC(int[,] input_grid, int[,] output_grid, int pattern_size, int max_iter, bool nLoop, bool includeInput, Vector2Int inputStart, bool WFCRotate90, bool WFCRotate180, bool WFCRotate270, bool WFCMirrorVert, bool WFCMirrorHor)
    {
        if (pattern_size > output_grid.GetLength(0) || pattern_size > output_grid.GetLength(1))
        {
            Debug.Log("pattern size incompativel com tamanho de output");
            return false;
        }

        if (pattern_size > input_grid.GetLength(0) || pattern_size > input_grid.GetLength(1))
        {
            Debug.Log("pattern size incompativel com tamanho de input");
            return false;
        }

        //list of patterns found in the input
        List<Pattern> patterns = new List<Pattern>();
        //list of frequences of the patterns
        List<int> freqs = new List<int>();

        //creation of the pattern grid
        int[,] pattern_grid = new int[input_grid.GetLength(0), input_grid.GetLength(1)];
        for (int x = 0; x < input_grid.GetLength(0); x++)
        {
            for (int y = 0; y < input_grid.GetLength(1); y++)
            {
                int[,] gp = GetPatternFromGrid(x, y, input_grid, pattern_size);
                Pattern p = new Pattern(gp);
                if (!patterns.Contains(p))
                {
                    patterns.Add(p);
                    freqs.Add(0);
                }
                pattern_grid[x, y] = patterns.IndexOf(p);
                freqs[patterns.IndexOf(p)] += 1;
            }
        }

        //Obtaining the relative frequences of the patterns
        float[] rel_freqs = new float[freqs.Count];
        int sum = input_grid.GetLength(0) * input_grid.GetLength(1);
        for (int i = 0; i < freqs.Count; i++)
        {
            rel_freqs[i] = (float)freqs[i] / sum;
        }

        //Lists of the neighbours of each pattern for each cardinal direction
        List<int>[] nUp = new List<int>[patterns.Count];
        List<int>[] nDown = new List<int>[patterns.Count];
        List<int>[] nLeft = new List<int>[patterns.Count];
        List<int>[] nRight = new List<int>[patterns.Count];
        //Initially, all patterns are possible for each position of the grid
        List<int> possibilities = new List<int>();
        for (int i = 0; i < patterns.Count; i++)
        {
            nUp[i] = new List<int>();
            nDown[i] = new List<int>();
            nLeft[i] = new List<int>();
            nRight[i] = new List<int>();
            possibilities.Add(i);
        }

        if (pattern_size == 1)
        {
            //Add the possible neighbours for each cardinal direction of each pattern
            for (int x = 0; x < pattern_grid.GetLength(0); x++)
            {
                for (int y = 0; y < pattern_grid.GetLength(1); y++)
                {
                    if (nLoop || ValidPos(pattern_grid.GetLength(0), pattern_grid.GetLength(1), new Vector2Int(x, y + 1)))
                    {
                        int up = pattern_grid[x, Utils.LoopInt(0, pattern_grid.GetLength(1), y + 1)];
                        nUp[pattern_grid[x, y]].Add(up);
                    }
                    if (nLoop || ValidPos(pattern_grid.GetLength(0), pattern_grid.GetLength(1), new Vector2Int(x, y - 1)))
                    {
                        int down = pattern_grid[x, Utils.LoopInt(0, pattern_grid.GetLength(1), y - 1)];
                        nDown[pattern_grid[x, y]].Add(down);
                    }
                    if (nLoop || ValidPos(pattern_grid.GetLength(0), pattern_grid.GetLength(1), new Vector2Int(x - 1, y)))
                    {
                        int left = pattern_grid[Utils.LoopInt(0, pattern_grid.GetLength(0), x - 1), y];
                        nLeft[pattern_grid[x, y]].Add(left);
                    }
                    if (nLoop || ValidPos(pattern_grid.GetLength(0), pattern_grid.GetLength(1), new Vector2Int(x + 1, y)))
                    {
                        int right = pattern_grid[Utils.LoopInt(0, pattern_grid.GetLength(0), x + 1), y];
                        nRight[pattern_grid[x, y]].Add(right);
                    }
                }
            }
        }
        else if (pattern_size >= 2)
        {
            if (WFCRotate90)
            {

            }
            if (WFCRotate180)
            {

            }
            if (WFCRotate270)
            {

            }
            if (WFCMirrorVert)
            {

            }
            if (WFCMirrorHor)
            {

            }

            for (int i = 0; i < patterns.Count; i++)
            {
                for (int j = i; j < patterns.Count; j++)
                {
                    Pattern pi = patterns[i];
                    Pattern pj = patterns[j];
                    //Up - pj está acima de pi
                    if (pi.IsNeighbour(Direction.Up, pj))
                    {
                        nUp[i].Add(j);
                        nDown[j].Add(i);
                    }
                    //Down
                    if (pi.IsNeighbour(Direction.Down, pj))
                    {
                        nDown[i].Add(j);
                        nUp[j].Add(i);
                    }
                    //Left
                    if (pi.IsNeighbour(Direction.Left, pj))
                    {
                        nLeft[i].Add(j);
                        nRight[j].Add(i);
                    }
                    //Right
                    if (pi.IsNeighbour(Direction.Right, pj))
                    {
                        nRight[i].Add(j);
                        nLeft[j].Add(i);
                    }
                }
            }

            //DEBUG - PARA REMOVER
            Debug.Log("num patterns = " + patterns.Count);
            for (int i = 0; i < patterns.Count; i++)
            {
                PrintPattern(patterns[i].grid);
                //Debug.Log("num Up Neighbours = " + nUp[i].Count);
                //for (int j = 0; j < nUp[i].Count; j++)
                //{
                //    Debug.Log(nUp[i][j]);
                //}
                //Debug.Log("num Down Neighbours = " + nDown[i].Count);
                //for (int j = 0; j < nDown[i].Count; j++)
                //{
                //    Debug.Log(nDown[i][j]);
                //}
                //Debug.Log("num Left Neighbours = " + nLeft[i].Count);
                //for (int j = 0; j < nLeft[i].Count; j++)
                //{
                //    Debug.Log(nLeft[i][j]);
                //}
                //Debug.Log("num Right Neighbours = " + nRight[i].Count);
                //for (int j = 0; j < nRight[i].Count; j++)
                //{
                //    Debug.Log(nRight[i][j]);
                //}
            }
        }

        //Grid of patterns that will be created with WFC
        List<int>[,] output_pattern_grid = new List<int>[output_grid.GetLength(0) - pattern_size + 1, output_grid.GetLength(1) - pattern_size + 1];
        for (int x = 0; x < output_grid.GetLength(0) - pattern_size + 1; x++)
        {
            for (int y = 0; y < output_grid.GetLength(1) - pattern_size + 1; y++)
            {
                output_pattern_grid[x, y] = new List<int>(possibilities);
            }
        }

        if (includeInput)
        {
            for (int x = 0; x < input_grid.GetLength(0); x++)
            {
                for (int y = 0; y < input_grid.GetLength(1); y++)
                {
                    int[,] gp = GetPatternFromGrid(x, y, input_grid, pattern_size);
                    Pattern p = new Pattern(gp);
                    output_pattern_grid[inputStart.x + x, inputStart.y + y] = new List<int>() { patterns.IndexOf(p) };
                }
            }
        }


        //real meat
        //1st step - analyse the grid (if its solved or if there was a colision)
        //2nd step - find the position with least entropy and collapse one of its possible values
        //3rd step - propagate the change to the neighboors, recalculating their possible values 

        max_iter = output_pattern_grid.GetLength(0) * output_pattern_grid.GetLength(1);
        for (int i = 0; i < max_iter; i++)
        {
            if (i == max_iter - 1) Debug.Log("Max iter");
            //If all count ==1, break
            if (CheckGridCollapsed(output_pattern_grid))
            {
                Debug.Log("WFC done in " + (i - 1));
                break;
            }

            if (CheckCollision(output_pattern_grid))
            {
                Debug.Log("WFC upsi " + i);
                return false;
            }

            //Find pos with least entropy
            Vector2Int point = FindLeastEntropy(output_pattern_grid, rel_freqs);

            //Debug.Log(point.x+" "+point.y);

            //collapse point 
            int value = SelectPossibleValue(output_pattern_grid[point.x, point.y], rel_freqs);
            output_pattern_grid[point.x, point.y] = new List<int>() { value };
            //Debug.Log("point " + point.x + " " + point.y + " colapsed with value " + value);

            //propagation step
            Propagate(output_pattern_grid, point, nUp, nDown, nLeft, nRight);
        }

        //Convert the output pattern grind into the output grid, with the original int values
        for (int x = 0; x < output_pattern_grid.GetLength(0); x++)
        {
            for (int y = 0; y < output_pattern_grid.GetLength(1); y++)
            {
                PutPatternOnGrid(x, y, output_grid, patterns[output_pattern_grid[x, y][0]].grid);
                //output_grid[x, y] = patterns[output_pattern_grid[x, y][0]]; 
            }
        }

        Utils.PrintIntGrid(output_pattern_grid);

        return true;
    }

    public class Pattern : IEquatable<Pattern>
    {
        public int[,] grid;
        int pattern_size;

        public Pattern(int[,] grid)
        {
            this.grid = grid;
            this.pattern_size = grid.GetLength(0);
        }

        //O padrão (other) na direção dir faz overlap?
        public bool IsNeighbour(Direction dir, Pattern other)
        {
            Direction dirO = OppositeDirection(dir);

            int[,] subT = this.GetSubGrid(dir);
            int[,] subO = other.GetSubGrid(dirO);

            return CompareGrids(subT, subO);
        }

        public int[,] GetSubGrid(Direction dir)
        {
            int xMax = 0;
            int xMin = 0;
            int yMax = 0;
            int yMin = 0;
            switch (dir)
            {
                case Direction.Up:
                    xMax = pattern_size;
                    yMax = pattern_size - 1;
                    xMin = 0;
                    yMin = 1;
                    break;

                case Direction.Down:
                    xMax = pattern_size;
                    yMax = pattern_size - 1;
                    xMin = 0;
                    yMin = 0;
                    break;

                case Direction.Left:
                    xMax = pattern_size - 1;
                    yMax = pattern_size;
                    xMin = 0;
                    yMin = 0;
                    break;

                case Direction.Right:
                    xMax = pattern_size - 1;
                    yMax = pattern_size;
                    xMin = 1;
                    yMin = 0;
                    break;

            }

            int[,] sub = new int[xMax, yMax];
            //Debug.Log("subx=" + sub.GetLength(0) + " suby=" + sub.GetLength(1));
            for (int x = 0; x < sub.GetLength(0); x++)
            {
                for (int y = 0; y < sub.GetLength(1); y++)
                {
                    sub[x, y] = this.grid[xMin + x, yMin + y];
                }
            }
            return sub;
        }

        public bool Equals(Pattern other)
        {
            return CompareGrids(this.grid, other.grid);
        }

        public static bool operator ==(Pattern a, Pattern b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Pattern a, Pattern b)
        {
            return !(a == b);
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    public static Direction OppositeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
        }
        return Direction.Up;
    }

    private static bool CompareGrids(int[,] a, int[,] b)
    {
        if ((a.GetLength(0) != b.GetLength(0)) || (a.GetLength(1) != b.GetLength(1)))
        {
            return false;
        }
        for (int x = 0; x < a.GetLength(0); x++)
        {
            for (int y = 0; y < a.GetLength(1); y++)
            {
                if (a[x, y] != b[x, y]) return false;
            }
        }
        return true;
    }

    private int[,] GetPatternFromGrid(int x, int y, int[,] grid, int pattern_size)
    {
        int[,] gp = new int[pattern_size, pattern_size];
        for (int px = 0; px < pattern_size; px++)
        {
            for (int py = 0; py < pattern_size; py++)
            {
                gp[px, py] = grid[Utils.LoopInt(0, grid.GetLength(0), x + px), Utils.LoopInt(0, grid.GetLength(1), y + py)];
            }
        }
        return gp;
    }

    private void PutPatternOnGrid(int x, int y, int[,] grid, int[,] pattern)
    {
        int pattern_size = pattern.GetLength(0);
        for (int px = 0; px < pattern_size; px++)
        {
            for (int py = 0; py < pattern_size; py++)
            {
                if (ValidPos(grid.GetLength(0), grid.GetLength(1), new Vector2Int(x + px, y + py)))
                {
                    grid[x + px, y + py] = pattern[px, py];
                }
            }
        }
    }

    private void PrintPattern(int[,] pattern)
    {
        string print = "pattern: ";
        int pattern_size = pattern.GetLength(0);
        for (int px = 0; px < pattern.GetLength(0); px++)
        {
            for (int py = 0; py < pattern.GetLength(1); py++)
            {
                print += pattern[px, py] + ",";
            }
            print += "; ";
        }
        Debug.Log(print);
    }



    //WFC's propagation step
    private void Propagate(List<int>[,] grid, Vector2Int point, List<int>[] nUp, List<int>[] nDown, List<int>[] nLeft, List<int>[] nRight)
    {
        //Neighbours to be checked are put on a queue - initially filled with the neighbours of the modified point
        Queue<Vector2Int> wave = new Queue<Vector2Int>();
        Vector2Int[] neighbours = new Vector2Int[] { point + new Vector2Int(1, 0), point + new Vector2Int(-1, 0), point + new Vector2Int(0, 1), point + new Vector2Int(0, -1) };
        foreach (Vector2Int n in neighbours)
        {
            if (ValidPos(grid.GetLength(0), grid.GetLength(1), n) && grid[n.x, n.y].Count >= 2)
            {
                wave.Enqueue(n);
            }
        }
        //Unitl the queue is empty, each neighbour is checked for changes on their probabilities - if there are, it's own neighbours are added to the queue
        while (wave.Count != 0)
        {
            Vector2Int pos = wave.Dequeue();
            if (RecalculatePossibilities(grid, pos, nUp, nDown, nLeft, nRight))
            {
                neighbours = new Vector2Int[] { pos + new Vector2Int(1, 0), pos + new Vector2Int(-1, 0), pos + new Vector2Int(0, 1), pos + new Vector2Int(0, -1) };
                foreach (Vector2Int n in neighbours)
                {
                    if (ValidPos(grid.GetLength(0), grid.GetLength(1), n) && grid[n.x, n.y].Count >= 2)
                    {
                        wave.Enqueue(n);
                    }
                }
            }
        }
    }

    //Acording with the possible neighbours of each pattern, for the given position, the possible patterns that can be placed in it are calculated
    private bool RecalculatePossibilities(List<int>[,] grid, Vector2Int point, List<int>[] nUp, List<int>[] nDown, List<int>[] nLeft, List<int>[] nRight)
    {
        List<int> possibilities = new List<int>(grid[point.x, point.y]);
        bool changed = false;
        Vector2Int upPoint = point + new Vector2Int(0, 1);
        Vector2Int downPoint = point + new Vector2Int(0, -1);
        Vector2Int leftPoint = point + new Vector2Int(-1, 0);
        Vector2Int rightPoint = point + new Vector2Int(1, 0);

        //For each possible pattern in the current list of possibilities for this point
        foreach (int pos in grid[point.x, point.y])
        {

            //up
            if (ValidPos(grid.GetLength(0), grid.GetLength(1), upPoint))
            {
                bool match = false;
                //for each possible pattern in the list of possibilities for the neighbour above this point
                foreach (int npos in grid[upPoint.x, upPoint.y])
                {
                    //if the current pattern has the neighbours pattern as a neighbour, then it is still a possibility
                    if (nUp[pos].Contains(npos)) match = true;
                }
                //if there was no match, the pattern is removed from the possibilities for this point
                if (!match)
                {
                    possibilities.Remove(pos);
                    changed = true;
                    continue;
                }
            }
            //down
            if (ValidPos(grid.GetLength(0), grid.GetLength(1), downPoint))
            {
                bool match = false;
                foreach (int npos in grid[downPoint.x, downPoint.y])
                {
                    if (nDown[pos].Contains(npos)) match = true;
                }
                if (!match)
                {
                    possibilities.Remove(pos);
                    changed = true;
                    continue;
                }
            }
            //left
            if (ValidPos(grid.GetLength(0), grid.GetLength(1), leftPoint))
            {
                bool match = false;
                foreach (int npos in grid[leftPoint.x, leftPoint.y])
                {
                    if (nLeft[pos].Contains(npos)) match = true;
                }
                if (!match)
                {
                    possibilities.Remove(pos);
                    changed = true;
                    continue;
                }
            }
            //right
            if (ValidPos(grid.GetLength(0), grid.GetLength(1), rightPoint))
            {
                bool match = false;
                foreach (int npos in grid[rightPoint.x, rightPoint.y])
                {
                    if (nRight[pos].Contains(npos)) match = true;
                }
                if (!match)
                {
                    possibilities.Remove(pos);
                    changed = true;
                    continue;
                }
            }
        }
        grid[point.x, point.y] = possibilities;

        return changed;
    }

    private bool ValidPos(int width, int height, Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height) return false;
        return true;
    }

    private bool CheckGridCollapsed(List<int>[,] grid)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y].Count != 1) return false;
            }
        }
        return true;
    }

    private bool CheckCollision(List<int>[,] grid)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y].Count == 0) return true;
            }
        }
        return false;
    }

    //The pattern is selected form the list of possibilities with regards to their relative frequencies
    private int SelectPossibleValue(List<int> possibilities, float[] rel_freqs)
    {
        float sum = 0f;
        foreach (int posb in possibilities)
        {
            sum += (float)rel_freqs[posb];
        }
        float randomValue = UnityEngine.Random.Range(0, sum);

        sum = 0f;
        foreach (int posb in possibilities)
        {
            sum += (float)rel_freqs[posb];
            if (randomValue <= sum)
            {
                return posb;
            }
        }
        return possibilities[possibilities.Count - 1];

    }

    private Vector2Int FindLeastEntropy(List<int>[,] grid, float[] rel_freqs)
    {
        Vector2Int pos = new Vector2Int(0, 0);
        float minEntropy = float.MaxValue;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y].Count > 1)
                {
                    float entropy = Entropy(grid[x, y], rel_freqs) + UnityEngine.Random.Range(0, 0.01f);
                    //Debug.Log(entropy);
                    if (entropy < minEntropy)
                    {

                        minEntropy = entropy;
                        pos.x = x;
                        pos.y = y;
                    }
                }
            }
        }
        return pos;
    }

    private float Entropy(List<int> possibilities, float[] rel_freqs)
    {
        float total_sum_weights = 0f;
        float current_sum_weights = 0f;
        foreach (int posb in possibilities)
        {
            total_sum_weights += rel_freqs[posb];
            current_sum_weights += Mathf.Log(rel_freqs[posb], 2);
        }
        float total_sum_weights_log = Mathf.Log(total_sum_weights, 2);

        return total_sum_weights_log - (current_sum_weights / total_sum_weights);
    }
}
