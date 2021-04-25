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
        return action;
    }

    //usa para algo que querias que o agente faça ao ser removido da grid
    //de momento meti codigo para o agente avisar a interface de update que "morreu", para se saber quando a simulação deve ser parada
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        //na função AgentCall a interface vai lidar com decrementar a sua variável que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }
}
