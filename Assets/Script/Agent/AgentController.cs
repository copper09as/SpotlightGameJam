using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentController : Agent
{
    [SerializeField]private Entity entity;
    [SerializeField]private Transform initTransform;
    [SerializeField]private Transform goalTransform;
    public override void Initialize()
    {
        base.Initialize();
        GameController.Controller.Disable();
        //entity = GetComponent<Entity>();
        entity.BindDead(Dead);
        entity.BindWin(Win);
        

    }
    private void Win()
    {
         AddReward(10);
         EndEpisode();
    }
    private void Dead()
    {
        //AddReward(-1);
        EndEpisode();
    }
public override void OnActionReceived(ActionBuffers actions)
{
    float moveX = actions.ContinuousActions[0];
    GameController.Move = moveX;

    int spacePressed = actions.DiscreteActions[0];

    if (spacePressed == 1 && !GameController.isSpacePressed)
    {
        GameController.OnSpacePressed();
    }
    else if (spacePressed == 0 && GameController.isSpacePressed)
    {
        GameController.OnSpaceReleased();
    }

    AddReward(-0.0005f);
    

}
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 手动测试用
        var continuousActions = actionsOut.ContinuousActions;
        var discreteActions = actionsOut.DiscreteActions;
        
        continuousActions[0] = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 dir = goalTransform.position - transform.position;
        
        sensor.AddObservation(dir.normalized);
    }
    
    private float lastDistanceToGoal;
public override void OnEpisodeBegin()
{
    EventBus.Publish(new End());
    transform.position = initTransform.position;

    entity.rb.velocity = Vector3.zero;
    entity.rb.angularVelocity = 0;
}
}