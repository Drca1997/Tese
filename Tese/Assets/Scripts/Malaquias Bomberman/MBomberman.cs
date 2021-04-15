using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBomberman : Agent
{
    //Constructor
    //Receives List<int> (states), int (x), and int (y)
    public MBomberman(List<int> states, int x, int y, IUpdate updateInterface)
    {
        //This agent cannot be placed in the same position of the agentGrid with the Agent types on this list
        this.colliderTypes.Add("Agent_Weak_Wall");
        this.colliderTypes.Add("Agent_Strong_Wall");
        this.colliderTypes.Add("Agent_Bomb");
        this.colliderTypes.Add("Player_Bomberman");
        this.colliderTypes.Add("Agent_Bomberman");

        this.states = states;
        this.position = new Vector2Int(x, y);
        this.typeName = "Malaquias_Bomberman";
        this.updateInterface = updateInterface;
    }

    //chamado para atualizar o agente em cada time step
    public override void UpdateAgent(Grid g, int step_stage, System.Random prng)
    {
        //podes usar g.agentGrid para obter a matriz de List<Agent>
        //g.ConvertAgentGrid() devolve-te uma List<int>[,] correspondente
        //tem em conta que algumas das listas podem estar vazias ou ter multiplos elementos, visto que h� sitios na grelha sem ou com multiplos agentes
        //Os inteiros correspondem aos indices que os tipos de agente ocupam em g.agentTypes - podes modific�-los na interface de setup, na cria��o da nova Grid

        //Para mover o agente e criar uma bomba podes consultar o meu codigo em PBomberman.cs na fun��o Logic

    }

    //usa para algo que querias que o agente fa�a ao ser removido da grid
    //de momento meti codigo para o agente avisar a interface de update que "morreu", para se saber quando a simula��o deve ser parada
    public override void Epitaph(Grid g, int step_stage, System.Random prng)
    {
        //na fun��o AgentCall a interface vai lidar com decrementar a sua vari�vel que indica o numero de jogadores
        updateInterface.AgentCall(this, g, prng);
    }
}
