using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bagingus : MonoBehaviour
{
    public string seed;
    public bool useRandomSeed;
    private System.Random prng;
    private Grid grid;

    ISetup SetupInterface;
    IUpdate UpdateInterface;
    IVisualize VisualizeInterface;

    private void Start()
    {
        SetupInterface = GetComponent<ISetup>();
        UpdateInterface = GetComponent<IUpdate>();
        VisualizeInterface = GetComponent<IVisualize>();
        if (useRandomSeed)
        {
            seed = System.DateTime.Now.ToString();
        }
        prng = new System.Random(seed.GetHashCode());

        grid = SetupInterface.SetupGrid(prng);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            grid = UpdateInterface.UpdateGrid(grid, prng);
            VisualizeInterface.VisualizeGrid(grid);
        }
    }
}