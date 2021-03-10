using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveToGoal : Agent
{
    public Transform targetPosition;
    public float moveSpeed;
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        Debug.Log(actions.ContinuousActions);
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveY) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetReward(1f);
        EndEpisode();
    }
}
