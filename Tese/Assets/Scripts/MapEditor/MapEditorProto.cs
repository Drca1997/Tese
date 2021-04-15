using System.Collections;
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
    private Grid grid;

    //Interface used to update the agents contained on the agentGrid component of the Grid object
    IUpdate UpdateInterface;
    //Interface used to update the GameObjects contained on the objectGrid component of the Grid object (visual representation of the agentGrid)
    IVisualize VisualizeInterface;

    // Start is called before the first frame update
    void Start()
    {

        UpdateInterface = GetComponent<IUpdate>();
        VisualizeInterface = GetComponent<IVisualize>();

        // creating a new System.Random with the given or a random seed
        if (useRandomSeed)
        {
            //System.DateTime.Now is used as the random seed
            seed = System.DateTime.Now.ToString();
        }
        prng = new System.Random(seed.GetHashCode());


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
