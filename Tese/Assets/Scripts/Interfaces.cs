using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISetup
{
    Grid SetupGrid(System.Random prng);
}

public interface IUpdate
{
    Grid UpdateGrid(Grid g, System.Random prng);
}

public interface IVisualize
{
    void VisualizeGrid(Grid g);
}