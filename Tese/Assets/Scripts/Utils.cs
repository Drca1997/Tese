using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils
{
    //Receives 3 int parameters: max, min, and x
    //Returns a new int contained between min and max (not including max) and related with x
    //Used manly to recalculate positions on the grid as if the cells on one extreme are neighbours of the ones on the other extreme
    //After value max-1 comes value min, and before value min comes value max-1
    public static int LoopInt(int min, int max, int x)
    {
        //if x is smaller than min, then (max - 1) - (min - 1) is added to it 
        //LoopInt is recursevly called once again in case x is not inbetween min and max-1 yet
        if (x < min)
        {
            x = x + (max - 1) - (min - 1);
            x = LoopInt(min, max, x);
        }

        //if x is greater than max, then (max - 1) + (min - 1) is subtracted to it 
        //LoopInt is recursevly called once again in case x is not inbetween min and max-1 yet
        if (x >= max)
        {
            x = x - (max - 1) + (min - 1);
            x = LoopInt(min, max, x);
        }

        //If the initial value x was already inbetween min and max-1, then it is returned unchanged
        return x;
    }

    //Receives a List<Agent> (aList) and a string (aType) as parametres
    //Returns the first Agent object on the aList that as an typeName component equal to aType 
    //otherwise, null is returned
    public static GameAgent AgentListContinesType(List<GameAgent> aList, string aType)
    {
        foreach(GameAgent a in aList)
        {
            if (string.Compare(a.typeName, aType) == 0) return a;
        }
        return null;
    }

    //Receives Vector2Int (agentPos), Vector2Int (relativePos), and int (width) and int (height)
    //Returns a Vector2Int
    //The grid loops horizontally and vertically such as the cells at one extreme have the cells at the other extreme as neighbors
    //This function receives a relative position to the Agent and returns the real position on the grid 
    public static Vector2Int GetRealPos(Vector2Int agentPos, Vector2Int relativePos, int width, int height)
    {
        Vector2Int realPos = agentPos + relativePos;

        //Constraining the position on both axis
        realPos.x = Utils.LoopInt(0, width, realPos.x);
        realPos.y = Utils.LoopInt(0, height, realPos.y);

        return realPos;
    }

    //Receives Vector2Int (gridPos), Grid (grid), List<string> (colliderTypeList)
    //Returns bool
    //Function used to check if a given position on the grid contains an Agent with a typeName component contained in the given colliderTypeList
    public static bool CollisionCheck(Vector2Int gridPos, Grid grid, List<string> colliderTypeList)
    {
        foreach (GameAgent a in grid.agentGrid[gridPos.x, gridPos.y])
        {
            if (colliderTypeList.Contains(a.typeName)) return true;
        }
        return false;
    }

    //Receives a string (text) and a Vector3 (position) as parameters
    //Creates a GameObject with a TextMesh component on the position given as parameter and with the given text
    public static void CreateText(string text, Vector3 position)
    {
        GameObject gameObject = new GameObject("Texto_Fixolas", typeof(TextMesh));
        Transform trans = gameObject.transform;
        trans.localPosition = position;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 20;
        textMesh.color = Color.black;
        textMesh.anchor = TextAnchor.MiddleCenter;
    }

    //Receives a Color (color), a string (name), a Vector3 (position), and a float (cellSize) as parameters
    //Creates and returns a GameObject with a SpriteRenderer component on the position given as parameter
    //The GameObject has the scale, color, and name given as parameters  
    public static GameObject CreateSquare(Color color, string name, Vector3 position, float cellSize)
    {
        GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
        Transform trans = gameObject.transform;
        trans.localPosition = position;
        trans.localScale = new Vector3(cellSize, cellSize);
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        //"Textures/um_pixel" is a white image of 1x1 dimensions
        sprite.sprite = Resources.Load<Sprite>("Textures/um_pixel");

        sprite.color = color;
        return gameObject;
    }


    //Returns a List<Agent> containting all the Agent objects found in the List<Agent>[,] given has a parameter
    public static List<GameAgent> PutAgentsInList(List<GameAgent>[,] agents)
    {
        List<GameAgent> listAgents = new List<GameAgent> { };
        for (int x = 0; x < agents.GetLength(0); x++)
        {
            for (int y = 0; y < agents.GetLength(1); y++)
            {
                foreach (GameAgent a in agents[x, y])
                {
                    listAgents.Add(a);
                }
            }
        }
        return listAgents;
    }

    //Implementation of the Fisher–Yates shuffle for List<T>
    //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm

    public static void Shuffle<T>(List<T> list, System.Random prng)
    {
        for(int i = list.Count-1; i>0; i--)
        {
            //number greater or equal to 0 and lesser than i
            int j= prng.Next(0, i+1);
            //swap list[i] and list[j]d
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    //Receives a List<Agent> (agentList) and a string[] (priorityList)
    //Returns the first Agent found on agentList with the typeName equal to the highest string on priorityList
    public static GameAgent GetAgent(List<GameAgent> agentList, string[] priorityList)
    {
        if (agentList.Count == 0) return null;
        else if (agentList.Count == 1) return agentList[0];
        else
        {
            foreach (string type in priorityList)
            {
                GameAgent a = Utils.AgentListContinesType(agentList, type);
                if (a != null) return a;
            }
        }
        return null;
    }

    
    public static void PrintAgentGrid(List<GameAgent>[,] agentGrid)
    {
        string print = "Agent Grid:\n";
        for (int y = agentGrid.GetLength(1)-1; y >=0 ; y--)
        {
            string line = "";
            for (int x = 0; x < agentGrid.GetLength(0); x++)
            {
                if (agentGrid[x, y].Count != 0) {
                    line += agentGrid[x, y][0].typeName+" | ";
                }
                else
                {
                    line += "empty | ";
                }
            }
            print += line + "\n";
        }
        Debug.Log(print);
    }


    public static void PrintIntGrid(List<int>[,] intGrid)
    {
        string print = "Int Grid:\n";
        for (int y = intGrid.GetLength(1) - 1; y >= 0; y--)
        {
            string line = "";
            for (int x = 0; x < intGrid.GetLength(0); x++)
            {
                if (intGrid[x, y].Count != 0)
                {
                    line += intGrid[x, y][0] + " | ";
                }
                else
                {
                    line += "  | ";
                }
            }
            print += line + "\n";
        }
        Debug.Log(print);
    }


    public static void PutOnGrid(List<int>[,] grid, List<Vector2Int> pattern, int filler)
    {
        foreach(Vector2Int pos in pattern)
        {
            Vector2Int realPos = GetRealPos(pos, new Vector2Int(0, 0), grid.GetLength(0), grid.GetLength(1));
            grid[realPos.x, realPos.y].Add(filler);
        }
    }

    public static void PutOnGrid(int[,] grid, List<Vector2Int> pattern, int filler)
    {
        foreach (Vector2Int pos in pattern)
        {
            Vector2Int realPos = GetRealPos(pos, new Vector2Int(0, 0), grid.GetLength(0), grid.GetLength(1));
            grid[realPos.x, realPos.y] = filler;
        }
    }

    public static void PutOnGrid(List<int>[,] grid, int[,] matrix)
    {
        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (matrix[i, j] != 0)
                {
                    grid[i, j].Add(matrix[i, j]);
                }
            }
        }
    }


    //Receives int (width) and int (height)
    //Returns a list of relative positions on a pattern of the frame of a box with the indicated dimensions
    public static List<Vector2Int> PatternBox(int width, int height)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();
        List<Vector2Int> sideW1 = PatternDirectionalLine(width, new Vector2Int(1, 0));
        List<Vector2Int> sideW2 = OffsetPattern(new List<Vector2Int>(sideW1), new Vector2Int(0, height-1));
        List<Vector2Int> sideH1 = PatternDirectionalLine(height, new Vector2Int(0, 1));
        List<Vector2Int> sideH2 = OffsetPattern(new List<Vector2Int>(sideH1), new Vector2Int(width-1, 0));
        pattern = MergePatterns(MergePatterns(MergePatterns(sideW1, sideW2), sideH1), sideH2);

        return pattern;
    }

    //Receives int (width) and int (height)
    //Returns a list of relative positions on a pattern of a filled box with the indicated dimensions
    public static List<Vector2Int> PatternFilledBox(int width, int height)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j< height; j++)
            {
                pattern.Add(new Vector2Int(i, j));
            }
        }

        return pattern;
    }

    //Variant of the previous function
    //Receives int (width) and int (height), int (stepSkipWidth, int (stepFillWidth), int (stepSkipHeight) and int (stepFillHeight)
    //Returns a list of relative positions on a pattern of a filled box with the indicated dimensions
    //The stepSkip and stepFill are used for cases when we don't want a continuously filled box
    //the pattern is repeated, containing stepFill poisitions and skipping the next stepSkip positions
    public static List<Vector2Int> PatternFilledBox(int width, int height, int stepSkipWidth, int stepFillWidth, int stepSkipHeight, int stepFillHeight)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();

        bool filling = true;
        int filled = 0;
        int skipped = 0;

        for (int i = 0; i < height; i++)
        {
            if (filling || stepSkipHeight == 0)
            {
                //Directional Line
                List<Vector2Int> newLine = OffsetPattern(PatternDirectionalDiscontinuousLine(width, new Vector2Int(1, 0), stepSkipWidth, stepFillWidth), new Vector2Int(0, i));
                pattern = MergePatterns(pattern, newLine);

                filled++;
                if (filled >= stepFillHeight)
                {
                    filling = false;
                    skipped = 0;
                }
            }
            else
            {
                skipped++;
                if (skipped >= stepSkipHeight)
                {
                    filling = true;
                    filled = 0;
                }
            }
        }

        return pattern;
    }

    //Receives int (size)
    //Returns a list of relative positions on a cross pattern in which each "hand" extends size units
    public static List<Vector2Int> PatternCross(int size)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();
        pattern.Add(new Vector2Int(0, 0));
        for(int i=1; i <= size; i++)
        {
            pattern.Add(new Vector2Int(i, 0));
            pattern.Add(new Vector2Int(-i, 0));
            pattern.Add(new Vector2Int(0, i));
            pattern.Add(new Vector2Int(0, -i));
        }
        return pattern;
    }

    //Variant of the previous function
    //Recieves Vector2Int (center), Grid (grid), List<string> (colliderTypes), List<string> stoppingTypes
    //It will also return a list of relative positions on a cross pattern
    //However, the size of the "hands" of the cross will be stopped short if an Agent contained in colliderTypes is in the way 
    //If the Agent is contained in stopingTypes, on the other hand, the position will sitll be added to the pattern
    public static List<Vector2Int> PatternCross(int size, Vector2Int center, Grid grid, List<string> colliderTypes, List<string> stoppingTypes)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();
        //add the center of the cross
        pattern.Add(new Vector2Int(0, 0));

        int[] xDirection = { 1, -1, 0, 0 };
        int[] yDirection = { 0, 0, 1, -1 };
        //do the process for each "hand" of the cross
        for (int i = 0; i < 4; i++)
        {
            //run along the hand until a collider is found, or the hand is at max size
            List<Vector2Int> hand = PatternDirectionalLine(size, new Vector2Int(xDirection[i], yDirection[i]), center + new Vector2Int(xDirection[i], yDirection[i]), grid, colliderTypes, stoppingTypes);
            hand = OffsetPattern(hand, new Vector2Int(xDirection[i], yDirection[i]));
            pattern = MergePatterns(hand, pattern);
        }
        return pattern;
    }


    //Receives a int (size), Vector2Int (direction)
    //Returns a a list of relative positions on a straight line acording with the given direction (ex.: (1,0) for right and (-1,1) for left and up) and size
    public static List<Vector2Int> PatternDirectionalLine(int size, Vector2Int direction)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();
        for (int i = 0; i < size; i++)
        {
            pattern.Add(direction * i);
        }
        return pattern;
    }

    //Variant of the previous function
    //Receives a int (size), Vector2Int (direction), Vector2Int (center), Grid (grid), List<string> (colliderTypes), List<string> (stoppingTypes)
    //It will also return a list of relative positions on a line pattern
    //However, the line will be stopped short if an Agent contained in colliderTypes is in the way 
    //If the Agent is contained in stopingTypes, on the other hand, the position will sitll be added to the pattern
    public static List<Vector2Int> PatternDirectionalLine(int size, Vector2Int direction, Vector2Int center, Grid grid, List<string> colliderTypes, List<string> stoppingTypes)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();
        for(int i = 0; i < size; i++)
        {
            Vector2Int newPos = direction * i;
            Vector2Int realPos = GetRealPos(center, newPos, grid.width, grid.height);
            //If this checks, then don't add more positions
            if (CollisionCheck(realPos, grid, colliderTypes))
            {
                break;
            }
            //If this checks, then add this position, but no more
            else if (CollisionCheck(realPos, grid, stoppingTypes))
            {
                pattern.Add(direction * i);
                break;
            }
            //If none check, add this position and check the next
            pattern.Add(direction*i);
        }
        return pattern;
    }


    //Receives a int (size), Vector2Int (direction), int (stepSkip), int (stepFill)
    //Returns a a list of relative positions on a straight line acording with the given direction (ex.: (1,0) for right and (-1,1) for left and up) and size
    //stepSkip and stepFill are used for when we don't want a continous line
    //stepSkip and stepFill characterize the resulting repeating pattern - how many positions are skipped and how many are included before the next skip
    //The pattern will start by including the first stepFill positions and then skip the next stepSkip positions, repeating until size is met
    public static List<Vector2Int> PatternDirectionalDiscontinuousLine(int size, Vector2Int direction, int stepSkip, int stepFill)
    {
        bool filling = true;
        int filled = 0;
        int skipped = 0;
        List<Vector2Int> pattern = new List<Vector2Int>();
        for (int i = 0; i < size; i++)
        {
            if (filling || stepSkip == 0)
            {
                pattern.Add(direction * i);
                filled++;
                if (filled >= stepFill) {
                    filling = false;
                    skipped = 0;
                }
            }
            else
            {
                skipped++;
                if(skipped >= stepSkip)
                {
                    filling = true;
                    filled = 0;
                }
            }
        }
        return pattern;
    }


    //Variant of the previous function
    //Receives a int (size), Vector2Int (direction), int (stepSkip), int (stepFill), Vector2Int (center), Grid (grid), List<string> (colliderTypes), List<string> (stoppingTypes)
    //It will also return a list of relative positions on a line pattern
    //However, the line will be stopped short if an Agent contained in colliderTypes is in the way 
    //If the Agent is contained in stopingTypes, on the other hand, the position will sitll be added to the pattern
    public static List<Vector2Int> PatternDirectionalDiscontinuousLine(int size, Vector2Int direction, int stepSkip, int stepFill, Vector2Int center, Grid grid, List<string> colliderTypes, List<string> stoppingTypes)
    {
        bool filling = true;
        int filled = 0;
        int skipped = 0;
        List<Vector2Int> pattern = new List<Vector2Int>();
        for (int i = 0; i < size; i++)
        {
            Vector2Int newPos = direction * i;
            Vector2Int realPos = GetRealPos(center, newPos, grid.width, grid.height);
            if (filling || stepSkip == 0)
            {
                //If this checks, then don't add more positions
                if (CollisionCheck(realPos, grid, colliderTypes))
                {
                    break;
                }
                //If this checks, then add this position, but no more
                else if (CollisionCheck(realPos, grid, stoppingTypes))
                {
                    pattern.Add(direction * i);
                    break;
                }
                //If none check, add this position and check the next
                pattern.Add(direction * i);

                filled++;
                if (filled >= stepFill)
                {
                    filling = false;
                    skipped = 0;
                }
            }
            else
            {
                if (CollisionCheck(realPos, grid, colliderTypes) || CollisionCheck(realPos, grid, stoppingTypes)) { break; }
                skipped++;
                if (skipped >= stepSkip)
                {
                    filling = true;
                    filled = 0;
                }
            }
        }
        return pattern;
    }


    //Receives List<Vector2Int> (pattern) and Vector2Int (offset)
    //Returns a List<Vector2Int> - the given pattern offseted by offset
    public static List<Vector2Int> OffsetPattern(List<Vector2Int> pattern, Vector2Int offset)
    {
        List<Vector2Int> offseted_pattern = new List<Vector2Int>();
        foreach (Vector2Int pos in pattern)
        {
            offseted_pattern.Add(pos + offset);
        }
        return offseted_pattern;
    }


    //Receives List<Vector2Int> (pattern1) and List<Vector2Int> (pattern2)
    //Returns a List<Vector2Int> - the result of the merging of the two List<Vector2Int> given as parameters
    //There are no duplicate Vector2Int in the returned List<Vector2Int>, its the union of the two patterns
    public static List<Vector2Int> MergePatterns(List<Vector2Int> pattern1, List<Vector2Int> pattern2)
    {
        List<Vector2Int> merged_pattern = new List<Vector2Int>(pattern1);
        merged_pattern.AddRange(pattern2);
        //remove duplicate positions
        merged_pattern = new HashSet<Vector2Int>(merged_pattern).ToList();

        return merged_pattern;
    }

    //Receives List<Vector2Int> (pattern), float (percent) and System.Random (prng)
    //Returns a List<Vector2Int> - the result of removing randomly percent% of the positions on the pattern
    public static List<Vector2Int> RandomizePattern(List<Vector2Int> pattern, int percent, System.Random prng)
    {
        List<Vector2Int> rando_pattern = new List<Vector2Int>();
        for (int i = 0; i < pattern.Count; i++)
        {
            if (prng.Next(0, 100) < percent)
            {
                rando_pattern.Add(pattern[i]);
            }
        }
        return rando_pattern;
    }

}