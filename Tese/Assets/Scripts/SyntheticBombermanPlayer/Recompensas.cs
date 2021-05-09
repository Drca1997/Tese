using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

//Adapted from https://answers.unity.com/questions/1284988/custom-inspector-2.html

public class Recompensas : MonoBehaviour
{
    [HideInInspector]
    public bool OnWinReward;

    [HideInInspector]
    public float OnWinRewardValue;

    [HideInInspector]
    public bool OnDeathPenalty;

    [HideInInspector]
    public float OnDeathPenaltyValue;

    [HideInInspector]
    public bool IterationPenalty;

    [HideInInspector]
    public float IterationPenaltyValue;

    [HideInInspector]
    public bool ExplodeBlockReward;

    [HideInInspector]
    public float ExplodeBlockRewardValue;
    [HideInInspector]
    public bool KillEnemyReward;

    [HideInInspector]
    public float KillEnemyRewardValue;

    [HideInInspector]
    public bool NotSafePenalty;

    [HideInInspector]
    public float NotSafePenaltyValue;

    [HideInInspector]
    public bool GetCloserToEnemyReward;

    [HideInInspector]
    public float GetCloserToEnemyRewardValue;

    [HideInInspector]
    public bool GetAwayFromEnemyPenalty;

    [HideInInspector]
    public float GetAwayFromEnemyPenaltyValue;

    // ... Update(), Awake(), etc
}

#if UNITY_EDITOR
[CustomEditor(typeof(Recompensas))]
public class Recompensas_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        Recompensas script = (Recompensas)target;

        // draw checkbox for the bool
        script.OnWinReward = EditorGUILayout.Toggle("On Win Reward", script.OnWinReward);
        if (script.OnWinReward) // if bool is true, show other fields
        {
            script.OnWinRewardValue = EditorGUILayout.FloatField(script.OnWinRewardValue);
            
        }

        script.OnDeathPenalty = EditorGUILayout.Toggle("On Death Penalty", script.OnDeathPenalty);
        if (script.OnDeathPenalty) // if bool is true, show other fields
        {
            script.OnDeathPenaltyValue = EditorGUILayout.FloatField(script.OnDeathPenaltyValue);

        }
        script.IterationPenalty = EditorGUILayout.Toggle("Iteration Penalty", script.IterationPenalty);
        if (script.IterationPenalty) // if bool is true, show other fields
        {
            script.IterationPenaltyValue = EditorGUILayout.FloatField(script.IterationPenaltyValue);

        }

        script.ExplodeBlockReward = EditorGUILayout.Toggle("Explode Block Reward", script.ExplodeBlockReward);
        if (script.ExplodeBlockReward) // if bool is true, show other fields
        {
            script.ExplodeBlockRewardValue = EditorGUILayout.FloatField(script.ExplodeBlockRewardValue);

        }

        script.KillEnemyReward = EditorGUILayout.Toggle("Kill Enemy Reward", script.KillEnemyReward);
        if (script.KillEnemyReward) // if bool is true, show other fields
        {
            script.KillEnemyRewardValue = EditorGUILayout.FloatField(script.KillEnemyRewardValue);

        }

        script.NotSafePenalty = EditorGUILayout.Toggle("Not Safe Penalty", script.NotSafePenalty);
        if (script.NotSafePenalty) // if bool is true, show other fields
        {
            script.NotSafePenaltyValue = EditorGUILayout.FloatField(script.NotSafePenaltyValue);

        }

        script.GetCloserToEnemyReward = EditorGUILayout.Toggle("Get Closer To Enemy Reward", script.GetCloserToEnemyReward);
        if (script.GetCloserToEnemyReward) // if bool is true, show other fields
        {
            script.GetCloserToEnemyRewardValue = EditorGUILayout.FloatField(script.GetCloserToEnemyRewardValue);

        }

        script.GetAwayFromEnemyPenalty = EditorGUILayout.Toggle("Get Away From Enemy Penalty", script.GetAwayFromEnemyPenalty);
        if (script.GetAwayFromEnemyPenalty) // if bool is true, show other fields
        {
            script.GetAwayFromEnemyPenaltyValue = EditorGUILayout.FloatField(script.GetAwayFromEnemyPenaltyValue);

        }


    }
}
#endif
