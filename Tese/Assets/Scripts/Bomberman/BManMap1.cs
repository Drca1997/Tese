using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BManMap1 : MonoBehaviour, IGenerateMap
{
    public List<int>[,] GenerateMap(System.Random prng) {

        List<int>[,] setup_grid = new List<int>[15,13];

        for (int x = 0; x < setup_grid.GetLength(0); x++)
        {
            for (int y = 0; y < setup_grid.GetLength(1); y++)
            {
                setup_grid[x, y] = new List<int> { };
                
            }
        }

        int[,] setup_setup_grid = new int[15, 13];

        List<Vector2Int> randomWall = Utils.OffsetPattern(Utils.RandomizePattern(Utils.PatternFilledBox(13, 11), 60, prng), new Vector2Int(1,1));

        Utils.PutOnGrid(setup_setup_grid, randomWall, 1);

        List<Vector2Int> freeSpace = Utils.OffsetPattern(Utils.PatternCross(2), new Vector2Int(1, 1));
        freeSpace = Utils.MergePatterns(freeSpace, Utils.OffsetPattern(Utils.PatternCross(2), new Vector2Int(1, 11)));
        freeSpace = Utils.MergePatterns(freeSpace, Utils.OffsetPattern(Utils.PatternCross(2), new Vector2Int(13, 1)));
        freeSpace = Utils.MergePatterns(freeSpace, Utils.OffsetPattern(Utils.PatternCross(2), new Vector2Int(13, 11)));

        Utils.PutOnGrid(setup_setup_grid, freeSpace, 0);

        List<Vector2Int> strongWall = Utils.OffsetPattern(Utils.PatternFilledBox(12,10,1,1,1,1), new Vector2Int(2, 2));

        Utils.PutOnGrid(setup_setup_grid, strongWall, 2);

        List<Vector2Int> border = Utils.PatternBox(15,13);

        Utils.PutOnGrid(setup_setup_grid, border, 2);


        //One of the 4 initial positions will be chosen for the Agent Player, the other ones will be filled by Non-Player Bomberman
        List <Vector2Int> startPos = new List<Vector2Int> { new Vector2Int(1, 1), new Vector2Int(13, 1), new Vector2Int(13, 11), new Vector2Int(1, 11) };
        Utils.Shuffle<Vector2Int>(startPos, prng);
        for(int i =0; i < 4; i++)
        {
            setup_setup_grid[startPos[i].x, startPos[i].y] = 3+i;
        }

        Utils.PutOnGrid(setup_grid, setup_setup_grid);

        return setup_grid;
    }
}
