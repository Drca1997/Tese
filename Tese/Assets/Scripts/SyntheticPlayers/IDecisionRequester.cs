using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDecisionRequester
{
    void GetWorld(Grid grid, int x, int y);
    int RequestDecision(); 
}
