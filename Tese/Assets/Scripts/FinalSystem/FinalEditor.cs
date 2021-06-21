using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FinalEditor : MonoBehaviour
{
    private Grid grid;

    private string[] gameScenarios = new string[] { "Game of Life", "Bomberman" };
    private List<string[]> agentTypes = new List<string[]> { new string[] {  "Life_Agent_Dead", "Life_Agent_Alive", "Player_Movement_Agent", "Random_Move_Agent" },
        new string[]{ "Agent_Weak_Wall", "Agent_Strong_Wall", "Player_Bomberman", "Agent_Bomberman", "Agent_Bomb", "Agent_Fire", "Agent_Bush", "Agent_Bushman" }};
    private List<string[]> setups = new List<string[]> { new string[] { "Blank", "Random" },
        new string[] {"Blank", "Random", "Classic"} };

    int width = 50;
    int height = 50;
    int widthMin = 5;
    int heightMin = 5;
    int widthMax = 200;
    int heightMax = 200;

    //reference to the System.Random used in the simulation
    private System.Random prng;

    //Interfaces
    private ISetup[] setupInterfaces;
    private IUpdate[] updateInterfaces;
    private IVisualize[] visualizeInterfaces;
    int updateIndex = 0;
    int visualizeIndex = 0;
    int scenarioIndex = 0;
    private IWFC WFCInterface;

    //UI
    public Dropdown scenarioDropdown;
    public Dropdown setupDropdown;
    public Button generateButt;
    public InputField widthInput;
    public InputField heightInput;
    public GameObject golPanel;
    public GameObject bmPanel;
    public Toggle[] golToggles;
    public Toggle[] bmToggles;
    public Button[] golButtons;
    public Button[] bmButtons;
    public Toggle selectingToggle;
    public Slider patternSize;
    public Toggle[] wfcToggles;
    public Button wfcButt;
    public Slider speedSlider;
    public Toggle debugMode;
    public Button playButt;


    private GameObject selection;
    private Vector2Int selectionPosition1 = new Vector2Int(0, 0);
    private Vector2Int selectionPosition2 = new Vector2Int(0, 0);
    private int selectingStage = 0;

    //states
    private bool editing = true;
    private bool selecting = false;

    void Start()
    {
        //Interfaces
        setupInterfaces = GetComponents<ISetup>();
        updateInterfaces = GetComponents<IUpdate>();
        visualizeInterfaces = GetComponents<IVisualize>();
        WFCInterface = GetComponent<IWFC>();

        //Initialize the grid with default values (empty 50x50) and the main camera
        grid = Utils.SetupEmptyGrid(width, height, 10, agentTypes[0], (x, y, grid) => { grid[x, y].Add(new LifeAgentDead(new List<int> { 0 }, x, y)); });
        SetupMainCamera(); 

        ////prng setup
        string seed = System.DateTime.Now.ToString();
        prng = new System.Random(seed.GetHashCode());

        ////UI setup
        //Generate Button
        generateButt.onClick.AddListener(new UnityAction(GenerateGrid));

        //Play Button
        playButt.onClick.AddListener(new UnityAction(PlayStop));

        //Selection Toggle
        selectingToggle.onValueChanged.AddListener(delegate { ToggleSelect(); });

        //WFC Button
        wfcButt.onClick.AddListener(new UnityAction(WFC));

        patternSize.onValueChanged.AddListener(delegate { OnPatternSizeChange(); });

        //Setup Agent Buttons
        ToggleButtons(golButtons, false);
        ToggleButtons(bmButtons, false);
        SetupAgentButtons();

        //Game Scenario Dropdown
        scenarioDropdown.options.Clear();
        foreach(string item in gameScenarios)
        {
            scenarioDropdown.options.Add(new Dropdown.OptionData() {text = item });
        }
        scenarioDropdown.value = 0;
        scenarioDropdown.RefreshShownValue();
        scenarioDropdown.onValueChanged.AddListener(delegate { ScenarioDropdownItemSelected(); });
        ScenarioDropdownItemSelected();

        //Agent Panels
        bmPanel.SetActive(false);
        golPanel.SetActive(true);

        //WFC initially deactivated
        wfcButt.interactable = false;

        ////Selection object setup
        selection = Utils.CreateSquare(new Color(0, 0.75f, 1f, 0.5f), "selection", new Vector3(0 + grid.cellSize / 2, 0 + grid.cellSize / 2, 0), grid.cellSize);
        selection.SetActive(false);

        GenerateGrid();
    }

    void Update()
    {
        if (editing)
        {
            if (Input.anyKey)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector2Int mainPos = grid.GetXY(Utils.GetMouseWorldPosition(Camera.main));
                if (grid.PosInGrid(mainPos))
                {
                    EditGrid(grid, mainPos);
                }
            }
        }
        else
        {
            //Every frame the IUpdate Interface function UpdateGrid is run
            updateInterfaces[updateIndex].UpdateGrid(grid, prng, speedSlider.value, debugMode.isOn);
            
            //If the updated component of the Grid object is true than the visuals are updated
            if (grid.updated)
            {
                //Utils.PrintIntGrid(grid.ConvertAgentGrid());
                visualizeInterfaces[visualizeIndex].VisualizeGrid(grid);
                grid.updated = false;
            }
        }

    }

    //Function called when the Generate button is pressed
    private void GenerateGrid()
    {
        width = Mathf.Clamp(int.Parse(widthInput.text),widthMin, widthMax);
        widthInput.text = width.ToString();
        height = Mathf.Clamp(int.Parse(heightInput.text), heightMin, heightMax);
        heightInput.text = height.ToString();
        ResetupGrid();

        //Return to editing mode
        if (!editing) PlayStop();
        //Activate the UI panel correspondent with the selected game scenario
        switch (scenarioDropdown.value)
        {
            case 0:
                bmPanel.SetActive(false);
                golPanel.SetActive(true);
                break;
            case 1:
                bmPanel.SetActive(true);
                golPanel.SetActive(false);
                break;
        }
        //Toggle off selection
        if (selecting) selectingToggle.isOn = false;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    private void WFC()
    {
        int[,] input_grid = null;
        //Input for WFC
        if (selecting && selectingStage == 2 && Mathf.Abs(selectionPosition1.x - selectionPosition2.x) > 1 && Mathf.Abs(selectionPosition1.y - selectionPosition2.y) > 1)
        {
            int[,] aux_grid = grid.ConvertAgentGrid2();
            int width = 1 + Mathf.Abs(selectionPosition1.x - selectionPosition2.x);
            int heigth = 1 + Mathf.Abs(selectionPosition1.y - selectionPosition2.y);
            Vector2Int start_pos = new Vector2Int(Mathf.Min(selectionPosition1.x, selectionPosition2.x), Mathf.Min(selectionPosition1.y, selectionPosition2.y));
            input_grid = new int[width, heigth];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < heigth; y++)
                {
                    input_grid[x, y] = aux_grid[start_pos.x + x, start_pos.y + y];
                }
            }
        }
        //WFC
        int max_tries = 12;

        bool ups = false;
        int[,] output_grid = new int[grid.width, grid.height];
        while (!WFCInterface.WFC(input_grid, output_grid, (int)patternSize.value, false, true, new Vector2Int(Mathf.Min(selectionPosition1.x, selectionPosition2.x), Mathf.Min(selectionPosition1.y, selectionPosition2.y)), wfcToggles[2].isOn, wfcToggles[3].isOn, wfcToggles[4].isOn, wfcToggles[1].isOn, wfcToggles[0].isOn))
        {
            output_grid = new int[grid.width, grid.height];
            max_tries--;
            if (max_tries < 0)
            {
                Debug.Log("WFC ups");
                ups = true;
                break;
            }
        }
        if (!ups)
        {
            SetupGrid(grid, output_grid);
            visualizeInterfaces[visualizeIndex].VisualizeGrid(grid);
        }
    }

    private void PlayStop()
    {
        if (editing)
        {
            playButt.GetComponentInChildren<Text>().text = "STOP";
            editing = false;
            if (selecting) selectingToggle.isOn = false;
            updateInterfaces[updateIndex].SetupSimulation(grid, prng);
        }
        else
        {
            playButt.GetComponentInChildren<Text>().text = "PLAY";
            editing = true;
        }
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    private void ToggleSelect()
    {
        selecting = !selecting;
        if (!selecting)
        {
            selection.SetActive(false);
            wfcButt.interactable = false;
        }
        selectingStage = 0;
        ToggleButtons(golButtons, false);
        ToggleButtons(bmButtons, false);
    }

    //Toggle a list of buttons interactable
    private void ToggleButtons(Button[] butts, bool interactable)
    {
        foreach(Button butt in butts)
        {
            butt.interactable = interactable;
        }
    }

    private void SetupAgentButtons()
    {
        Vector2Int gridPos = new Vector2Int(0, 0);

        golButtons[0].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new LifeAgentAlive(new List<int> { 0 }, x, y)); }); });
        golButtons[1].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new LifeAgentDead(new List<int> { 0 }, x, y)); }); });
        
        bmButtons[0].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AStrongWall2(new List<int> { 100, 100 }, x, y)); }); });
        bmButtons[1].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AWeakWall2(new List<int> { 100, 100 }, x, y)); }); });
        bmButtons[2].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABush(new List<int> { }, x, y)); }); });
        bmButtons[3].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABomb(new List<int> { }, x, y, null)); }); });
        bmButtons[4].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AFire(new List<int> { }, x, y, null)); }); });
        bmButtons[5].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABomberman(new List<int> { }, x, y, GetComponent<IUpdate>())); }); });
        bmButtons[6].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABushman(new List<int> { }, x, y, GetComponent<IUpdate>())); }); });
        bmButtons[7].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new PBomberman(new List<int> { }, x, y, this, GetComponent<IUpdate>())); }); });
        bmButtons[8].onClick.AddListener(delegate { InsertOnGrid(grid, gridPos, (x, y) => { }); });

    }

    private void SetupMainCamera()
    {
        int widthGutter = 10;
        int heightGutter = 10;
        grid.container.transform.position = new Vector3(grid.container.transform.position.x + widthGutter, grid.container.transform.position.y + heightGutter, grid.container.transform.position.z);

        //CAMERA SET UP
        //Utils.SetupCameraToGrid(Camera.main, grid);
        float sceneWidth = widthGutter + grid.width * grid.cellSize + widthGutter;
        float sceneHeight = heightGutter + grid.height * grid.cellSize + heightGutter;
        Utils.SetupCameraToMinDimensions(Camera.main, sceneWidth, sceneHeight);
        //CAMERA SET UP
    }

    private void ResetupGrid()
    {
        scenarioIndex = scenarioDropdown.value;
        int setupIndex = setupDropdown.value;
        bool created = false;
        grid.deleteContainer();
        if (setups[scenarioIndex][setupIndex] != "Blank")
        {
            int index = 0;
            for (index = 0; index < setupInterfaces.Length; index++)
            {
                if (setupInterfaces[index].ReturnSet() == gameScenarios[scenarioIndex] && setupInterfaces[index].ReturnName() == setups[scenarioIndex][setupIndex])
                {
                    grid = setupInterfaces[index].SetupGrid(prng, width, height);
                    created = true;
                    break;
                }
            }
        }
        if(created == false){
            switch (scenarioIndex)
            {
                case 0:
                    grid = Utils.SetupEmptyGrid(width, height, 10, agentTypes[0], (x, y, grid) => { grid[x, y].Add(new LifeAgentDead(new List<int> { 0 }, x, y)); });
                    break;
                case 1:
                    grid = Utils.SetupEmptyGrid(width, height, 10, agentTypes[1], (x, y, grid) => { });
                    break;
            }
        }

        for(int i =0; i<updateInterfaces.Length; i++)
        {
            foreach (string set in updateInterfaces[i].ReturnSet()) {
                if (set == gameScenarios[scenarioIndex])
                {
                    updateIndex = i;
                    break;
                }
            }
        }

        for (int i = 0; i < visualizeInterfaces.Length; i++)
        {
            if (visualizeInterfaces[i].ReturnSet() == gameScenarios[scenarioIndex])
            {
                visualizeIndex = i;
                break;
            }
        }

        SetupMainCamera();
        visualizeInterfaces[visualizeIndex].VisualizeGrid(grid);
    }

    private void ScenarioDropdownItemSelected()
    {
        int scenarioIndex = scenarioDropdown.value;
        setupDropdown.options.Clear();
        foreach (string item in setups[scenarioIndex])
        {
            setupDropdown.options.Add(new Dropdown.OptionData() { text = item });
        }
        setupDropdown.value = 0;
        setupDropdown.RefreshShownValue();
    }

    private void SetupGrid(Grid grid, int[,] setup_grid)
    {
        for (int x = 0; x < setup_grid.GetLength(0); x++)
        {
            for (int y = 0; y < setup_grid.GetLength(1); y++)
            {
                grid.agentGrid[x, y] = new List<GameAgent> { };
                switch (scenarioIndex)
                {
                    case 0:
                        switch (setup_grid[x, y])
                        {
                            case 0:
                                grid.agentGrid[x, y].Add(new LifeAgentDead(new List<int> { 0 }, x, y));
                                break;
                            case 1:
                                grid.agentGrid[x, y].Add(new LifeAgentAlive(new List<int> { 0 }, x, y));
                                break;
                        }
                        break;
                    case 1:
                        switch (setup_grid[x, y])
                        {
                            case 0:
                                grid.agentGrid[x, y].Add(new AWeakWall2(new List<int> { 100, 100 }, x, y));
                                break;
                            case 1:
                                grid.agentGrid[x, y].Add(new AStrongWall2(new List<int> { 100, 100 }, x, y));
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
                        break;
                }
                
            }
        }
    }

    private void EditGrid(Grid grid, Vector2Int gridPos)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && selecting)
        {
            float cellSize = grid.cellSize;
            if (selectingStage != 1)
            {
                wfcButt.interactable = false;
                if (selectingStage == 0)
                {
                    switch (scenarioIndex)
                    {
                        case 0:
                            ToggleButtons(golButtons, true);
                            break;
                        case 1:
                            ToggleButtons(bmButtons, true);
                            break;
                    }
                }
                selectingStage = 1;
                selectionPosition1 = gridPos;
                selection.SetActive(true);
                Vector3 pos = grid.GetWorldPosition(gridPos.x, gridPos.y);
                selection.transform.position = new Vector3(pos.x + cellSize / 2, pos.y + cellSize / 2, pos.z);
                selection.transform.localScale = new Vector3(cellSize, cellSize);
            }
            else if (selectingStage == 1 && selectionPosition1 != gridPos)
            {
                selectingStage = 2;
                selectionPosition2 = gridPos;
                float width = cellSize * (1 + Mathf.Abs(selectionPosition1.x - selectionPosition2.x));
                float heigth = cellSize * (1 + Mathf.Abs(selectionPosition1.y - selectionPosition2.y));
                Vector3 pos1 = grid.GetWorldPosition(selectionPosition1.x, selectionPosition1.y);
                Vector3 pos2 = grid.GetWorldPosition(selectionPosition2.x, selectionPosition2.y);
                selection.transform.position = new Vector3(Mathf.Min(pos1.x, pos2.x) + width / 2, Mathf.Min(pos1.y, pos2.y) + heigth / 2, pos1.z);
                selection.transform.localScale = new Vector3(width, heigth);
                ToggleWFC();
            }
        }
        else {
            switch (scenarioIndex)
            {
                case 0:
                    if (Input.GetKey(KeyCode.Alpha1) || Input.GetMouseButton(0) && golToggles[0].isOn)
                    {
                        //Alive Cell
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new LifeAgentAlive(new List<int> { 0 }, x, y)); });
                    }
                    else if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Backspace) || Input.GetMouseButton(0) && golToggles[1].isOn)
                    {
                        //Dead Cell
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new LifeAgentDead(new List<int> { 0 }, x, y)); });
                    }
                    break;
                case 1:
                    if (Input.GetKey(KeyCode.Alpha1) || Input.GetMouseButton(0) && bmToggles[0].isOn)
                    {
                        //Steel Wall
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AStrongWall2(new List<int> { 100, 100 }, x, y)); });
                    }
                    else if (Input.GetKey(KeyCode.Alpha2) || Input.GetMouseButton(0) && bmToggles[1].isOn)
                    {
                        //Brick Wall
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AWeakWall2(new List<int> { 100, 100 }, x, y)); });
                    }
                    else if (Input.GetKey(KeyCode.Alpha3) || Input.GetMouseButton(0) && bmToggles[2].isOn)
                    {
                        //Bush Wall
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABush(new List<int> { }, x, y)); });
                    }
                    else if (Input.GetKey(KeyCode.Alpha4) || Input.GetMouseButton(0) && bmToggles[3].isOn)
                    {
                        //Bomb
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABomb(new List<int> { }, x, y, null)); });
                    }
                    else if (Input.GetKey(KeyCode.Alpha5) || Input.GetMouseButton(0) && bmToggles[4].isOn)
                    {
                        //Fire
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new AFire(new List<int> { }, x, y, null)); });
                    }
                    else if (Input.GetKey(KeyCode.Alpha6) || Input.GetMouseButton(0) && bmToggles[5].isOn)
                    {
                        //Bomberman
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABomberman(new List<int> { }, x, y, GetComponent<IUpdate>())); });
                    }
                    else if (Input.GetKey(KeyCode.Alpha7) || Input.GetMouseButton(0) && bmToggles[6].isOn)
                    {
                        //Bushman
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new ABushman(new List<int> { }, x, y, GetComponent<IUpdate>())); });
                    }
                    else if (Input.GetKey(KeyCode.Alpha8) || Input.GetMouseButton(0) && bmToggles[7].isOn)
                    {
                        //Player
                        InsertOnGrid(grid, gridPos, (x, y) => { grid.agentGrid[x, y].Add(new PBomberman(new List<int> { }, x, y, this, GetComponent<IUpdate>())); });
                    }
                    else if (Input.GetKey(KeyCode.Backspace) || Input.GetMouseButton(0) && bmToggles[8].isOn)
                    {
                        //Erase
                        InsertOnGrid(grid, gridPos, (x, y) => { });
                    }
                    break;
            }

        }
        
    }

    public void ToggleWFC()
    {
        float width = 1 + Mathf.Abs(selectionPosition1.x - selectionPosition2.x);
        float height = 1 + Mathf.Abs(selectionPosition1.y - selectionPosition2.y);
        int pSize = (int)patternSize.value;
        int minSize = 4;
        if (width >= pSize + 2 && height >= pSize + 2 && width >= minSize && height >= minSize) { 
            wfcButt.interactable = true; 
        }
    }

    public void OnPatternSizeChange()
    {
        wfcButt.interactable = false;
        ToggleWFC();
    }

    public void InsertOnGrid(Grid grid, Vector2Int gridPos, System.Action<int, int> action)
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

        visualizeInterfaces[visualizeIndex].VisualizeGrid(grid);
    }

}
