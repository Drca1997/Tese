using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    
    public const int beSafeGoalPriority = 1;
    public const int attackEnemyGoalPriority = 2;
    public const int gridWidth = 8;
    public const double explodibleBlockCost = 5.0;

    public static int[,] deepCopyWorld(int[,] world)
    {
        int[,] deepCopy = new int[world.GetLength(0), world.GetLength(1)];
        for (int i = 0; i < world.GetLength(0); i++)
        {
            for (int j = 0; j < world.GetLength(1); j++)
            {
                deepCopy[i, j] = world[i, j];
            }
        }
        return deepCopy;
    }
    public static IEnumerable GridIterator(int[,] grid)
    {
        for (int i=0; i < grid.GetLength(0); i++)
        {
            for (int j=0; j < grid.GetLength(1); j++)
            {
               yield return new int[2] { i, j };
            }
        }
    }


    public static int [] GetTileFromIndex(int index, int width)
    {
        int x = index / width;
        int y = index % width;
        return new int[2] {x, y };
    }

    public static List<int> GetNeighbouringTilesIndexes(int [,] grid, int index)
    {
        int[] tempIndexes = new int[4];
        tempIndexes[0] = index - grid.GetLength(0);
        tempIndexes[1] = index - 1;
        tempIndexes[2] = index + 1;
        tempIndexes[3] = index + grid.GetLength(0);
        List<int> validIndexes = new List<int>();
        if (tempIndexes[0] >= 0) //north neighbour
        {
            validIndexes.Add(tempIndexes[0]);
        }
        if (index % grid.GetLength(0) != 0) //EastNeighbour-> 1ºelem de cada linha é multiplo da largura do grid
        {
            validIndexes.Add(tempIndexes[1]);
        }
        if (tempIndexes[2] % grid.GetLength(0) != 0)//West Neighbour-> ultimo elem de cada linha nao pode ser multiplo da largura do grid
        {
            validIndexes.Add(tempIndexes[2]);
        }
        if (tempIndexes[3] < grid.GetLength(0) * grid.GetLength(1)) //south neighbour
        {
            validIndexes.Add(tempIndexes[3]);
        }
        return validIndexes;

    }

    public static List<int[]> GetAdjacentTiles(int[,] grid, int[] tile)
    {
        List<int[]> adjacentTiles = new List<int[]>();
        if (tile[1] + 1 < grid.GetLength(1))
        {
            adjacentTiles.Add(new int[2] { tile[0], tile[1] + 1 });

            if (tile[1] + 2 < grid.GetLength(1))
            {
                adjacentTiles.Add(new int[2] { tile[0], tile[1] + 2 });
            }
        }
        if (tile[1] - 1 >= 0)
        {
            adjacentTiles.Add(new int[2] { tile[0], tile[1] - 1 });
            if (tile[1] - 2 >= 0)
            {
                adjacentTiles.Add(new int[2] { tile[0], tile[1] - 2 });
            }

        }
        if (tile[0] + 1 < grid.GetLength(0))
        {
            adjacentTiles.Add(new int[2] { tile[0] + 1, tile[1] });
            if (tile[0] + 2 < grid.GetLength(0))
            {
                adjacentTiles.Add(new int[2] { tile[0] + 2, tile[1] });
            }
        }
        if (tile[0] - 1 >= 0)
        {
            adjacentTiles.Add(new int[2] { tile[0] - 1, tile[1] });
            if (tile[0] - 2 >= 0)
            {
                adjacentTiles.Add(new int[2] { tile[0] - 2, tile[1] });
            }
        }
        return adjacentTiles;
    }

    public static Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static TextMesh CreateText(string text, Transform parent, Vector3 position, Vector3 scale, Color color, int fontsize, TextAnchor anchor, TextAlignment alignment)
    {
        GameObject obj = new GameObject("Text", typeof(TextMesh));

        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        TextMesh label = obj.GetComponent<TextMesh>();
        label.text = text;
        label.color = color;
        label.fontSize = fontsize;
        label.anchor = anchor;
        label.alignment = alignment;
        return label;
    }

    public static GameObject CreateSpriteRenderer(Vector3 worldPosition, Sprite sprite, float cellSize)
    {
        GameObject obj = new GameObject();
        obj.transform.position = worldPosition + new Vector3(cellSize, cellSize) * 0.5f;
        obj.AddComponent<SpriteRenderer>();
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        return obj;
    }

    public static bool IsTileWalkable(TempGrid grid, int x, int y) {
        if (x >= 0 && x < grid.Width && y >= 0 && y < grid.Height)
        {
            if (grid.Array[x, y] == 1 || grid.Array[x, y] == 4)
            {
                return true;
            }

            
        }
        return false;
    }

    public static string ActionToString(int action)
    {
        switch (action)
        {
            case 0:
                return "moveu-se para cima";
            case 1:
                return "moveu-se para baixo";
            case 2:
                return "moveu-se para a esquerda";
            case 3:
                return "moveu-se para a direita";
            case 4:
                return "PLANTOU BOMBA";
            case 5:
                return "permaneceu no mesmo sítio";
               
        }
        return null;
    }

    public static bool IsValidAction(TempGrid grid, BaseAgent agent, int action)
    {
        switch (action)
        {
            case 0: //move up
                if (agent.Y + 1 < grid.Array.GetLength(1)) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X, agent.Y + 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 1: //move down
                if (agent.Y - 1 >= 0) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X, agent.Y - 1)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 2: //move left
                if (agent.X - 1 >= 0) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X - 1, agent.Y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 3: //move right
                if (agent.X + 1 < grid.Array.GetLength(0)) // se esta dentro dos limites
                {
                    if (Utils.IsTileWalkable(grid, agent.X + 1, agent.Y)) //Se é walkable
                    {
                        return true;
                    }
                }
                return false;
            case 4: //plant bomb
                if (!agent.PlantedBomb)
                {
                    agent.PlantedBomb = true;
                    return true;
                }

                return false;

            case 5: //do nothing
                return true;
            default:
                break;
        }
        return true;
    }


    public static bool IsTileSafe(int[,] grid, int [] tile)
    {
        
        if (grid[tile[0], tile[1]] == 4 || grid[tile[0], tile[1]] == 5) // se esta uma bomba na tile
        {
            return false;
        }
        if(!IsNorthTilesSafe(grid, tile))
        {
            return false;
        }
        if (!IsSouthTilesSafe(grid, tile))
        {
            return false;
        }
        if (!IsEastTilesSafe(grid, tile))
        {
            return false;
        }
       if (!IsWestTilesSafe(grid, tile))
        {
            return false;
        }
        return true;
    }

    private static bool IsNorthTilesSafe(int[,] grid, int[] tile)
    {

        if (tile[1] + 1 < grid.GetLength(1) && (grid[tile[0], tile[1] + 1] == 4 || grid[tile[0], tile[1] + 1] == 5)) //se bomba em (x, y+1)
        {
            return false;
        }

        if (tile[1] + 2 < grid.GetLength(1) && (grid[tile[0], tile[1] + 2] == 4 || grid[tile[0], tile[1] + 2] == 5) ) //se bomba em (x, y+2)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (grid[tile[0], tile[1] + 1] != 2 && grid[tile[0], tile[1] + 1] != 3)
            {
                return false;
            }

        }
        return true;
    }
    
    private static bool IsSouthTilesSafe(int[,] grid, int[] tile)
    {
        if (tile[1] -1 >=0 && (grid[tile[0], tile[1] - 1] == 4 || grid[tile[0], tile[1] - 1] == 5))//se bomba em (x, y-1)
        {
            return false;
        }
        if (tile[1] - 2 >= 0 && (grid[tile[0], tile[1] - 2] == 4 || grid[tile[0], tile[1] - 2] == 5))//se bomba em (x, y-2)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (grid[tile[0], tile[1] - 1] != 2 && grid[tile[0], tile[1] - 1] != 3)
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsWestTilesSafe(int[,] grid,int[] tile)
    {
        if (tile[0] - 1 >= 0 && (grid[tile[0] - 1, tile[1]] == 4 || grid[tile[0] - 1, tile[1]] == 5)) // se bomba em (x-1, y)
        {
            return false;
        }
        if (tile[0] - 2 >= 0 && (grid[tile[0] - 2, tile[1]] == 4 || grid[tile[0] - 2, tile[1]] == 5)) // se bomba em (x-2, y)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (grid[tile[0] - 1, tile[1]] != 2 && grid[tile[0] - 1, tile[1]] != 3)
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsEastTilesSafe(int[,] grid, int[] tile)
    {
        
        if (tile[0] + 1 < grid.GetLength(0) && (grid[tile[0] + 1, tile[1]] == 4 || grid[tile[0] + 1, tile[1]] == 5)) // se bomba em (x+1, y)
        {
            return false;
        }
        if (tile[0] + 2 < grid.GetLength(0) && (grid[tile[0] + 2, tile[1]] == 4 || grid[tile[0] + 2, tile[1]] == 5)) // se bomba em (x+2, y)
        {
            //se a tile entre tile atual e agente nao é algo que tapou a explosao da bomba
            if (grid[tile[0] + 1, tile[1]] != 2 && grid[tile[0] + 1, tile[1]] != 3)
            {
                return false;
            }
        }
        return true;
    }

    public static bool [,] dangerMap(int[,] grid)
    {
        bool[,] dangerMap = new bool[grid.GetLength(0), grid.GetLength(1)];
        for (int i=0; i < grid.GetLength(0); i++)
        {
            for (int j=0; j < grid.GetLength(1); j++)
            {
                if (!IsTileSafe(grid, new int[2] { i, j}))
                {
                    dangerMap[i, j] = true;
                }
                else
                {
                    dangerMap[i, j] = false;
                }
            }
        }
        
        return dangerMap;
    }

    public static List<int[]> dangerTiles(bool[,] dangerMap, bool reverse) //reverse indica se devolve dangerTiles(false) ou safeTiles(true)
    {
        List<int[]> dangerTiles = new List<int[]>();
        for (int i=0; i < dangerMap.GetLength(0); i++)
        {
            for (int j=0; j < dangerMap.GetLength(1); j++)
            {
                if (reverse)
                {
                    if (!dangerMap[i, j])
                    {
                        dangerTiles.Add(new int[2] { i, j });
                    }

                }
                else
                {
                    if (dangerMap[i, j])
                    {
                        dangerTiles.Add(new int[2] { i, j });
                    }
                }
               
            }
        }
        return dangerTiles;
    }


    /**
    public static List<int[]> GetTilesInBounds(Grid grid, int x, int y)
    {
        List<int[]> tiles = new List<int[]>();
        if (y + 1 < grid.Array.GetLength(1))
        {
            if (.IsTileAffected(x, y + 1))
            {
                tiles.Add(new int[] { x, y + 1 });
            }
            //se (x,y+2) nao está fora do mapa, e (x, y+1) nao é algo que tapou o radio da explosao
            if (y + 2 < grid.Array.GetLength(1) && grid.Array[x, y + 1] != 2 && grid.Array[x, y + 1] != 3)
            {
                if (IsTileAffected(x, y + 2))
                {
                    tTiles.Add(new int[] { x, y + 2 });
                }
            }
        }
        return tiles;
    }*/


    #region BernardoUtils
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
        foreach (GameAgent a in aList)
        {
            if (string.Compare(a.typeName, aType) == 0) return a;
        }
        return null;
    }

    //Receives Vector2Int (agentPos), Vector2Int (relativePos), and Grid (grid)
    //Returns a Vector2Int
    //The grid loops horizontally and vertically such as the cells at one extreme have the cells at the other extreme as neighbors
    //This function receives a relative position to the Agent and returns the real position on the grid 
    public static Vector2Int GetRealPos(Vector2Int agentPos, Vector2Int relativePos, Grid grid)
    {
        Vector2Int realPos = agentPos + relativePos;

        //Constraining the position on both axis
        realPos.x = Utils.LoopInt(0, grid.width, realPos.x);
        realPos.y = Utils.LoopInt(0, grid.height, realPos.y);

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
        sprite.sprite = Resources.Load<Sprite>("um_pixel");

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
        for (int i = list.Count - 1; i > 0; i--)
        {
            //number greater or equal to 0 and lesser than i
            int j = prng.Next(0, i + 1);
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
        for (int y = agentGrid.GetLength(1) - 1; y >= 0; y--)
        {
            string line = "";
            for (int x = 0; x < agentGrid.GetLength(0); x++)
            {
                if (agentGrid[x, y].Count != 0)
                {
                    line += agentGrid[x, y][0].typeName + " | ";
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


    //public static List<Vector2Int> PatternBox(int width, int height, bool fill, float fillPercent)
    //{
    //    List<Vector2Int> pattern = new List<Vector2Int>();


    //    return pattern;e
    //}

    //Receives int (size)
    //Returns a list of relative positions on a cross pattern in which each "hand" extends size units
    public static List<Vector2Int> PatternCross(int size)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();
        pattern.Add(new Vector2Int(0, 0));
        for (int i = 1; i <= size; i++)
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
            List<Vector2Int> hand = DirectionalLine(size, new Vector2Int(xDirection[i], yDirection[i]), center + new Vector2Int(xDirection[i], yDirection[i]), grid, colliderTypes, stoppingTypes);
            hand = OffsetPattern(hand, new Vector2Int(xDirection[i], yDirection[i]));
            pattern = MergePatterns(hand, pattern);
        }
        return pattern;
    }


    //Receives a int (size), Vector2Int (direction)
    //Returns a a list of relative positions on a straight line acording with the given direction (ex.: (1,0) for right and (-1,1) for left and up) and size
    public static List<Vector2Int> DirectionalLine(int size, Vector2Int direction)
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
    public static List<Vector2Int> DirectionalLine(int size, Vector2Int direction, Vector2Int center, Grid grid, List<string> colliderTypes, List<string> stoppingTypes)
    {
        List<Vector2Int> pattern = new List<Vector2Int>();
        for (int i = 0; i < size; i++)
        {
            Vector2Int newPos = direction * i;
            Vector2Int realPos = GetRealPos(center, newPos, grid);
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
    //There are no duplicate Vector2Int in the returned List<Vector2Int> 
    public static List<Vector2Int> MergePatterns(List<Vector2Int> pattern1, List<Vector2Int> pattern2)
    {
        List<Vector2Int> merged_pattern = new List<Vector2Int>(pattern1);
        merged_pattern.AddRange(pattern2);
        //remove duplicate positions
        merged_pattern = new HashSet<Vector2Int>(merged_pattern).ToList();

        return merged_pattern;
    }
    #endregion
}
