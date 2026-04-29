using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentController : Agent
{
    public override void OnActionReceived(ActionBuffers actions)
    {
        // 获取连续动作（取值范围 -1 到 1）
        float moveX = actions.ContinuousActions[0];
        
        // 模拟按下/释放空格（离散动作：0=释放，1=按下）
        int spacePressed = actions.DiscreteActions[0];
        
        // 应用控制
        if (moveX != 0)
        {
           GameController.Move = moveX;
        }
        else
        {
            GameController.Move = 0;
        }
        
        if (spacePressed == 1 && !GameController.isSpacePressed)
        {
            // 模拟按下空格
            GameController.OnSpacePressed();
        }
        else if (spacePressed == 0 && GameController.isSpacePressed)
        {
            // 模拟释放空格
            GameController.OnSpaceReleased();
        }
        
        // 获取当前跳跃蓄力时间（用于奖励计算）
        float chargeTime = GameController.GetJumpChargeTime();
        
        // 在这里添加奖励逻辑
        // AddReward(...);
        
        // 获取滚轮输入（如果需要）
        float scrollDelta = GameController.GetScrollDelta();
    }
    
public override void Heuristic(in ActionBuffers actionsOut)
{
    var continuousActions = actionsOut.ContinuousActions;
    var discreteActions = actionsOut.DiscreteActions;
    
    // 改为只响应 A/D 键
    float moveX = 0f;
    if (Input.GetKey(KeyCode.A))
        moveX = -1f;
    if (Input.GetKey(KeyCode.D))
        moveX = 1f;
    
    continuousActions[0] = moveX;
    discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    
    Debug.Log($"Heuristic - MoveX: {continuousActions[0]}, SpacePressed: {discreteActions[0]}");
}
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // 收集观察信息
        // sensor.AddObservation(GameController.GetJumpChargeTime());
        // sensor.AddObservation(GameController.GetScrollDelta());
        // sensor.AddObservation(transform.position);
    }
    
    public override void OnEpisodeBegin()
    {
        // 回合开始时重置
    }
}