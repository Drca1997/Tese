using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class used as a basis for agents that receive input from the player
public abstract class AgentPlayer : Agent
{
    //boolean that indicates wether the Agent has finnished its update cycle (may or may not require multiple inputs from the palyer)
    public bool updated = true;
    //reference to the KeyCode last inputed by the player
    public KeyCode input = KeyCode.None;

    //Receives KeyCode[] (codes)
    //Returns IEnumerator
    //Ienumerator function used for coroutines waiting for the player's input
    //In each update loop all the given codes will be checked
    //If the player is pressing one of them, the input component will be updated and the function will finish
    public IEnumerator WaitForKeyDown(KeyCode[] codes)
    {
        bool pressed = false;
        while (!pressed)
        {
            foreach (KeyCode k in codes)
            {
                if (Input.GetKeyDown(k))
                {
                    pressed = true;
                    input = k;
                    break;
                }
            }
            yield return null;
        }
    }
}
