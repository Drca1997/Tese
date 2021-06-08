using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorProto : MonoBehaviour
{

    //seed used to initiate System.Random used in the simulation
    public string seed;
    //if a random seed should be used instead of the given one
    public bool useRandomSeed;
    //reference to the System.Random used in the simulation
    private System.Random prng;
    //reference to the Grid object used in the simulation
    private Grid mainGrid;

    private Grid wfcGrid;

    private GameObject selection;
    private Vector2Int selectionPosition1 = new Vector2Int(0, 0);
    private Vector2Int selectionPosition2 = new Vector2Int(0, 0);
    private Grid gridSelected = null;
    private int selectingStage = 0;

    public int cellSize = 10;
    public int wfcWidth=6;
    public int wfcHeight=5;
    public int mainWidth=20;
    public int mainHeight=10;

    public int wallRegen = 100;

    public int pattern_size = 1;
    public int maxWFCIterations = 100;
    public bool loopWFCNeighbours = true;

    public bool WFCRotate90 = false;
    public bool WFCRotate180 = false;
    public bool WFCRotate270 = false;
    public bool WFCMirrorVert = false;
    public bool WFCMirrorHor = false;

    private string[] agentTypes = new string[] { "Agent_Weak_Wall", "Agent_Strong_Wall", "Player_Bomberman", "Agent_Bomberman", "Agent_Bomb", "Agent_Fire", "Agent_Bush", "Agent_Bushman" };

    private bool editing = true;
    private bool selecting = false;

    //Interface used to set up the Grid object
    ISetup SetupInterface;
    //Interface used to update the agents contained on the agentGrid component of the Grid object
    IUpdate UpdateInterface;
    //Interface used to update the GameObjects contained on the objectGrid component of the Grid object (visual representation of the agentGrid)
    IVisualize VisualizeInterface;

    IWFC WFCInterface;

    private void Start()
    {
        SetupInterface = GetComponent<ISetup>();
        UpdateInterface = GetComponent<IUpdate>();
        VisualizeInterface = GetComponent<IVisualize>();
        WFCInterface = GetComponent<IWFC>();

        // creating a new System.Random with the given or a random seed
        if (useRandomSeed)
        {
            //System.DateTime.Now is used as the random seed
            seed = System.DateTime.Now.ToString();
        }
        prng = new System.Random(seed.GetHashCode());

        //Initializing the grid acording with the ISetup Interface
        //grid = SetupInterface.SetupGrid(prng);
        mainGrid = Utils.SetupEmptyGrid(mainWidth, mainHeight, 10, agentTypes);
        wfcGrid = Utils.SetupEmptyGrid(wfcWidth, wfcHeight, 10, agentTypes);

        wfcGrid.container.transform.position = new Vector3(wfcGrid.container.transform.position.x + 10, wfcGrid.container.transform.position.y, wfcGrid.container.transform.position.z);
        mainGrid.container.transform.position = new Vector3(mainGrid.container.transform.position.x + wfcGrid.width * wfcGrid.cellSize + 30, mainGrid.container.transform.position.y, mainGrid.container.transform.position.z);

        //CAMERA SET UP
        //Utils.SetupCameraToGrid(Camera.main, grid);
        float sceneWidth = 10 + wfcGrid.width * wfcGrid.cellSize + 20 + mainGrid.width * mainGrid.cellSize + 10;
        float sceneHeight = mainGrid.height * mainGrid.cellSize;
        Utils.SetupCameraToMinDimensions(Camera.main, sceneWidth, sceneHeight);
        //CAMERA SET UP

        //Updating the visuals acording with the IVisualize Interface and the initial state of the grid
        VisualizeInterface.VisualizeGrid(wfcGrid);
        VisualizeInterface.VisualizeGrid(mainGrid);

        selection = Utils.CreateSquare(new Color(0,0.75f,1f,0.5f), "selection", new Vector3(0+cellSize/2, 0+cellSize/2, 0), cellSize);
        selection.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (editing)
        {
            if (Input.anyKey)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector2Int mainPos = mainGrid.GetXY(Utils.GetMouseWorldPosition(Camera.main));
                Vector2Int wfcPos = wfcGrid.GetXY(Utils.GetMouseWorldPosition(Camera.main));
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    editing = false;
                    selecting = false;
                    selection.SetActive(false);
                    selectingStage = 0;
                    UpdateInterface.SetupSimulation(mainGrid, prng);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    bool includeInput = false;
                    int[,] input_grid = null;
                    //Input for WFC
                    if (selecting && selectingStage == 2 && Mathf.Abs(selectionPosition1.x-selectionPosition2.x)>1 && Mathf.Abs(selectionPosition1.y - selectionPosition2.y) > 1)
                    {
                        int[,] aux_grid = gridSelected.ConvertAgentGrid2();
                        int width = 1 + Mathf.Abs(selectionPosition1.x - selectionPosition2.x);
                        int heigth = 1 + Mathf.Abs(selectionPosition1.y - selectionPosition2.y);
                        Vector2Int start_pos = new Vector2Int(Mathf.Min(selectionPosition1.x, selectionPosition2.x), Mathf.Min(selectionPosition1.y, selectionPosition2.y));
                        input_grid = new int[width, heigth];
                        for(int x = 0; x < width; x++)
                        {
                            for(int y = 0; y < heigth; y++)
                            {
                                input_grid[x, y] = aux_grid[start_pos.x + x, start_pos.y + y];
                            }
                        }
                        if(gridSelected==mainGrid) includeInput = true; 
                    }
                    else
                    {
                        //convert the wfcGrid into a int matrix
                        input_grid = wfcGrid.ConvertAgentGrid2();

                    }
                    //WFC
                    int max_tries = 5;
                    bool ups = false;
                    int[,] output_grid = new int[mainGrid.width, mainGrid.height];
                    if (includeInput)
                    {
                        while (!WFCInterface.WFC(input_grid, output_grid, pattern_size, maxWFCIterations, loopWFCNeighbours, true, new Vector2Int(Mathf.Min(selectionPosition1.x, selectionPosition2.x), Mathf.Min(selectionPosition1.y, selectionPosition2.y)), WFCRotate90, WFCRotate180, WFCRotate270, WFCMirrorVert, WFCMirrorHor))
                        {
                            output_grid = new int[mainGrid.width, mainGrid.height];
                            max_tries--;
                            if (max_tries < 0)
                            {
                                Debug.Log("WFC ups");
                                ups = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        while (!WFCInterface.WFC(input_grid, output_grid, pattern_size, maxWFCIterations, loopWFCNeighbours, false, Vector2Int.zero, WFCRotate90, WFCRotate180, WFCRotate270, WFCMirrorVert, WFCMirrorHor))
                        {
                            output_grid = new int[mainGrid.width, mainGrid.height];
                            max_tries--;
                            if (max_tries < 0)
                            {
                                Debug.Log("WFC ups");
                                ups = true;
                                break;
                            }
                        }
                    }
                    //WFC
                    if (!ups)
                    {
                        SetupGrid(mainGrid, output_grid);
                        VisualizeInterface.VisualizeGrid(mainGrid);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    selecting = !selecting;
                    if (!selecting) { 
                    
                        selection.SetActive(false);
                    }
                    selectingStage = 0;
                }
                else if (mainGrid.PosInGrid(mainPos))
                {
                    EditGrid(mainGrid, mainPos);
                }
                else if (wfcGrid.PosInGrid(wfcPos))
                {
                    EditGrid(wfcGrid, wfcPos);
                }
            }
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                editing = true;
            }
            //Every frame the IUpdate Interface function UpdateGrid is run
            UpdateInterface.UpdateGrid(mainGrid, prng);

            //If the updated component of the Grid object is true than the visuals are updated
            if (mainGrid.updated)
            {
                //Utils.PrintIntGrid(grid.ConvertAgentGrid());
                VisualizeInterface.VisualizeGrid(mainGrid);
                mainGrid.updated = false;
            }
        }
    }


    private void SetupGrid(Grid grid, int[,] setup_grid)
    {
        for (int x = 0; x < setup_grid.GetLength(0); x++)
        {
            for (int y = 0; y < setup_grid.GetLength(1); y++)
            {
                grid.agentGrid[x, y] = new List<GameAgent> { };
                switch (setup_grid[x, y])
                {
                    case 0:
                        grid.agentGrid[x, y].Add(new AWeakWall2(new List<int> { wallRegen, wallRegen }, x, y));
                        break;
                    case 1:
                        grid.agentGrid[x, y].Add(new AStrongWall2(new List<int> { wallRegen, wallRegen }, x, y));
                        break;
                    case 2:
                        grid.agentGrid[x, y].Add(new PBomberman(new List<int> { }, x, y, this, GetComponent<IUpdate>()));
                        break;
                    case 3:
                        grid.agentGrid[x, y].Add(new ABomberman(new List<int> { }, x, y, GetComponent<IUpdate>()));
                        break;
                    case 4:
                        grid.agentGrid[x, y].Add(new ABomb(new List<int> { }, x, y, null));
                        break;
                    case 5:
                        grid.agentGrid[x, y].Add(new AFire(new List<int> { }, x, y, null));
                        break;
                    case 6:
                        grid.agentGrid[x, y].Add(new ABush(new List<int> { }, x, y));
                        break;
                    case 7:
                        grid.agentGrid[x, y].Add(new ABushman(new List<int> { }, x, y, GetComponent<IUpdate>()));
                        break;
                }
            }
        }
    }

    private void EditGrid(Grid grid, Vector2Int gridPos)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && selecting)
        {
            Debug.Log(selectingStage);
            if (selectingStage != 1 || gridSelected != grid)
            {
                Debug.Log("selection1");
                selectingStage = 1;
                selectionPosition1 = gridPos;
                gridSelected = grid;
                selection.SetActive(true);
                Vector3 pos = grid.GetWorldPosition(gridPos.x, gridPos.y);
                selection.transform.position = new Vector3(pos.x + cellSize / 2, pos.y + cellSize / 2, pos.z);
                selection.transform.localScale = new Vector3(cellSize,cellSize);
            }
            else if (selectingStage == 1 && selectionPosition1 != gridPos)
            {
                Debug.Log("selection2");
                selectingStage = 2;
                selectionPosition2 = gridPos;
                float width = cellSize * (1 + Mathf.Abs(selectionPosition1.x- selectionPosition2.x));
                float heigth = cellSize * (1 + Mathf.Abs(selectionPosition1.y - selectionPosition2.y));
                Vector3 pos1 = grid.GetWorldPosition(selectionPosition1.x, selectionPosition1.y);
                Vector3 pos2 = grid.GetWorldPosition(selectionPosition2.x, selectionPosition2.y);
                selection.transform.position = new Vector3(Mathf.Min(pos1.x, pos2.x) + width / 2, Mathf.Min(pos1.y, pos2.y) + heigth / 2, pos1.z);
                selection.transform.localScale = new Vector3(width, heigth);
            }
        }
        else if (Input.GetKey(KeyCode.Alpha0))
        {
            InsertAgent(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AWeakWall2(new List<int> { wallRegen, wallRegen }, x, y)); });
            VisualizeInterface.VisualizeGrid(grid);
        }
        else if (Input.GetKey(KeyCode.Alpha1))
        {
            InsertAgent(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AStrongWall2(new List<int> { wallRegen, wallRegen }, x, y)); });
            VisualizeInterface.VisualizeGrid(grid);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            InsertAgent(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new PBomberman(new List<int> { }, x, y, this, GetComponent<IUpdate>())); });  
            VisualizeInterface.VisualizeGrid(grid);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            InsertAgent(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABomberman(new List<int> { }, x, y, GetComponent<IUpdate>())); });
            VisualizeInterface.VisualizeGrid(grid);
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            InsertAgent(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABomb(new List<int> { }, x, y, null)); }); 
            VisualizeInterface.VisualizeGrid(grid);
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            InsertAgent(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AFire(new List<int> { }, x, y, null)); });
            VisualizeInterface.VisualizeGrid(grid);
        }
        else if (Input.GetKey(KeyCode.Alpha6))
        {
            InsertAgent(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABush(new List<int> { }, x, y)); });
            VisualizeInterface.VisualizeGrid(grid);
        }
        else if (Input.GetKey(KeyCode.Alpha7))
        {
            InsertAgent(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABushman(new List<int> { }, x, y, GetComponent<IUpdate>())); });
            VisualizeInterface.VisualizeGrid(grid);
        }
        else if (Input.GetKey(KeyCode.Backspace))
        {
            InsertAgent(grid, gridPos, (x, y) => { });
            VisualizeInterface.VisualizeGrid(grid);
        }
    }

    public void InsertAgent(Grid grid, Vector2Int gridPos, System.Action<int, int> action)
    {
        if (selecting && selectingStage != 0)
        {
            if (selectingStage == 1)
            {
                grid.agentGrid[selectionPosition1.x, selectionPosition1.y] = new List<GameAgent> { };
                action(selectionPosition1.x, selectionPosition1.y);
            }
            else if (selectingStage == 2)
            {
                for (int x = Mathf.Min(selectionPosition1.x, selectionPosition2.x); x <= Mathf.Max(selectionPosition1.x, selectionPosition2.x); x++)
                {
                    for (int y = Mathf.Min(selectionPosition1.y, selectionPosition2.y); y <= Mathf.Max(selectionPosition1.y, selectionPosition2.y); y++)
                    {
                        Debug.Log(x + " " + y);
                        grid.agentGrid[x, y] = new List<GameAgent> { };
                        action(x, y);
                    }
                }
            }
        }
        else
        {
            grid.agentGrid[gridPos.x, gridPos.y] = new List<GameAgent> { };
            action(gridPos.x, gridPos.y);
        }
    }
}
