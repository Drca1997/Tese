using UnityEngine;

public class bagingus : MonoBehaviour
{
    //seed used to initiate System.Random used in the simulation
    public string seed;
    //if a random seed should be used instead of the given one
    public bool useRandomSeed;
    //number of times the simulation will be run
    public int numberOfEpisodes = 1;
    //number of times the simulation has be run
    private int episodeNumber = 0;
    //reference to the System.Random used in the simulation
    private System.Random prng;
    //reference to the Grid object used in the simulation
    private Grid grid;

    //Interface used to set up the Grid object
    ISetup SetupInterface;
    //Interface used to update the agents contained on the agentGrid component of the Grid object
    IUpdate UpdateInterface;
    //Interface used to update the GameObjects contained on the objectGrid component of the Grid object (visual representation of the agentGrid)
    IVisualize VisualizeInterface;

    private void Start()
    {
        SetupInterface = GetComponent<ISetup>();
        UpdateInterface = GetComponent<IUpdate>();
        VisualizeInterface = GetComponent<IVisualize>();

        // creating a new System.Random with the given or a random seed
        if (useRandomSeed)
        {
            //System.DateTime.Now is used as the random seed
            seed = System.DateTime.Now.ToString();
        }
        prng = new System.Random(seed.GetHashCode());

        //Initializing the grid acording with the ISetup Interface
        grid = SetupInterface.SetupGrid(prng);

        //CAMERA SET UP
        Camera.main.transform.position = new Vector3(grid.width * grid.cellSize / 2, grid.height * grid.cellSize / 2, Camera.main.transform.position.z);
        Camera.main.orthographicSize = grid.height * grid.cellSize / 2;
        Camera.main.orthographicSize = (grid.width * grid.cellSize + 20) * Screen.height / Screen.width * 0.5f;
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = (grid.width * grid.cellSize) / (grid.height * grid.cellSize);
        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = grid.height * grid.cellSize / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = grid.height * grid.cellSize / 2 * differenceInSize;
        }
        //CAMERA SET UP

        UpdateInterface.SetupSimulation(grid, prng);

        //Updating the visuals acording with the IVisualize Interface and the initial state of the grid
        VisualizeInterface.VisualizeGrid(grid);

    }

    private void Update()
    {
        //Every frame the IUpdate Interface function UpdateGrid is run
        UpdateInterface.UpdateGrid(grid, prng);

        //If the updated component of the Grid object is true than the visuals are updated
        if (grid.updated)
        {
            //Utils.PrintIntGrid(grid.ConvertAgentGrid());
            VisualizeInterface.VisualizeGrid(grid);
            grid.updated = false;
        }

        //if the simulation is over, a new one will be setted up if the number of required episodes hasn't been met
        if (grid.simOver)
        {
            episodeNumber++;

            if (episodeNumber < numberOfEpisodes)
            {
                Debug.Log("starting new episode (" + episodeNumber + ")");
                grid.deleteContainer();
                //Initializing the grid acording with the ISetup Interface
                grid = SetupInterface.SetupGrid(prng);
                UpdateInterface.SetupSimulation(grid, prng);

                //Updating the visuals acording with the IVisualize Interface and the initial state of the grid
                VisualizeInterface.VisualizeGrid(grid);
            }
        }
    }
}