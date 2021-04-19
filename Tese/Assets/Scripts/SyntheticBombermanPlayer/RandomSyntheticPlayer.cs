using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSyntheticPlayer : SyntheticBombermanPlayer
{
    public RandomSyntheticPlayer(List<int> states, int x, int y, IUpdate updateInterface): base(states, x, y, updateInterface)
    {

    }

    public override int TakeAction()
    {
        int action = Random.Range(0, 6);
        while (!SyntheticPlayerUtils.IsValidAction(GridArray, this, action))
        {
            action = Random.Range(0, 6);

        }
        Debug.Log(action);
        return action;
    }
}
