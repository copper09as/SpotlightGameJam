using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AgentController : Agent
{
    [SerializeField]private Entity entity;
    [SerializeField]private Transform initTransform;
    [SerializeField]private Transform goalTransform;
    private const int MaxEpisodeSteps = 12000;
    private BehaviorParameters behaviorParameters;
    private bool previousJumpPressed;
    private bool episodeEnding;
    private const bool StompEnemyBeforeGoal = true;
    private int stompedEnemyCount;
    private int lastJumpStep = -1000;
    private readonly List<GameObject> disabledEnemies = new();
    public override void Initialize()
    {
        base.Initialize();
        behaviorParameters = GetComponent<BehaviorParameters>();
        EnsureDynamicRigidbody();
        GameController.Controller.Disable();
        //entity = GetComponent<Entity>();
        entity.BindDead(Dead);
        entity.BindWin(Win);
        

    }
    private void Win()
    {
         if (IsStompFirstMode() && stompedEnemyCount == 0 && FindActiveEnemyTarget() != null)
         {
             AddReward(-0.5f);
             return;
         }

         if (!IsValidWinPosition())
         {
             return;
         }

         AddReward(IsStompFirstMode() && stompedEnemyCount > 0 ? 12f : 10f);
         EndTrainingEpisode("win");
    }
    private void Dead()
    {
        //AddReward(-1);
        AddReward(-2f);
        EndTrainingEpisode("dead");
    }
public override void OnActionReceived(ActionBuffers actions)
{
    float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
    int spacePressed = actions.DiscreteActions[0];
    bool jumpPressed = spacePressed == 1;
    SyncGameController(moveX, jumpPressed);

    if (behaviorParameters == null)
    {
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    if (behaviorParameters == null || behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly)
    {
        ApplyAgentPhysics(moveX, jumpPressed);
        CheckTrainingOverlaps();
    }

    if (jumpPressed && !GameController.isSpacePressed)
    {
        GameController.OnSpacePressed();
    }
    else if (!jumpPressed && GameController.isSpacePressed)
    {
        GameController.OnSpaceReleased();
    }

    AddReward(-0.0005f);
    RewardGoalProgress();
    CheckEpisodeTimeout();

}
    private void FixedUpdate()
    {
        if (behaviorParameters == null)
        {
            behaviorParameters = GetComponent<BehaviorParameters>();
        }

        if (behaviorParameters == null || behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly)
        {
            return;
        }

        EnsureDynamicRigidbody();

        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        float moveX = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
        bool jumpPressed = keyboard.spaceKey.isPressed;
        SyncGameController(moveX, jumpPressed);
        ApplyAgentPhysics(moveX, jumpPressed);
        CheckTrainingOverlaps();
    }

    private void SyncGameController(float moveX, bool jumpPressed)
    {
        GameController.Move = moveX;
        GameController.isLeftPressed = moveX < -0.1f;
        GameController.isRightPressed = moveX > 0.1f;

        if (jumpPressed && !GameController.isSpacePressed)
        {
            GameController.OnSpacePressed();
        }
        else if (!jumpPressed && GameController.isSpacePressed)
        {
            GameController.OnSpaceReleased();
        }
    }

    private void ApplyAgentPhysics(float moveX, bool jumpPressed)
    {
        EnsureDynamicRigidbody();

        if (entity == null || entity.rb == null || entity.rb.bodyType != RigidbodyType2D.Dynamic)
        {
            return;
        }

        const float targetMoveSpeed = 5f;
        const float acceleration = 8f;
        const float jumpVelocity = 6.4f;

        float moveIntent = Mathf.Abs(moveX) > 0.05f ? Mathf.Sign(moveX) : 0f;
        bool usesLearnedPolicy = behaviorParameters == null || behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly;
        Transform trainingTarget = GetPrimaryTrainingTarget();
        bool targetingEnemy = trainingTarget != null && trainingTarget != goalTransform && HasTagInHierarchy(trainingTarget, "Enemy");
        if (usesLearnedPolicy && trainingTarget != null)
        {
            Vector3 targetPosition = GetTargetPosition(trainingTarget);
            float targetDirection = Mathf.Sign(targetPosition.x - transform.position.x);
            if (Mathf.Abs(targetDirection) > 0.01f && (moveIntent == 0f || Mathf.Sign(moveIntent) != targetDirection))
            {
                moveIntent = targetDirection;
            }

            if (targetingEnemy)
            {
                float horizontalDistance = Mathf.Abs(targetPosition.x - transform.position.x);
                if (IsGrounded() && horizontalDistance < 1.8f && StepCount - lastJumpStep > 12)
                {
                    jumpPressed = true;
                }
            }
            else if (IsGrounded() && StepCount - lastJumpStep > 18)
            {
                jumpPressed = true;
            }
        }

        float targetSpeed = moveIntent * targetMoveSpeed;
        float speedDiff = targetSpeed - entity.rb.velocity.x;
        entity.rb.AddForce(new Vector2(speedDiff * acceleration, 0f), ForceMode2D.Force);

        if (jumpPressed && IsGrounded() && StepCount - lastJumpStep > 8)
        {
            entity.rb.velocity = new Vector2(entity.rb.velocity.x, jumpVelocity);
            lastJumpStep = StepCount;
        }

        previousJumpPressed = jumpPressed;
    }

    private bool IsGrounded()
    {
        Collider2D col = entity != null ? entity.col : null;
        if (col == null)
        {
            col = GetComponent<Collider2D>();
        }

        Vector2 center = transform.position;
        float halfWidth = 0.2f;
        float bottomY = transform.position.y - 0.1f;
        if (col != null)
        {
            center = col.bounds.center;
            halfWidth = col.bounds.extents.x;
            bottomY = col.bounds.min.y;
        }

        int mask = LayerMask.GetMask("Ground");
        int rayMask = mask == 0 ? Physics2D.DefaultRaycastLayers : mask;
        float[] offsets = { -halfWidth * 0.5f, 0f, halfWidth * 0.5f };
        foreach (float offset in offsets)
        {
            Vector2 origin = new Vector2(center.x + offset, bottomY + 0.02f);
            if (Physics2D.Raycast(origin, Vector2.down, 0.25f, rayMask).collider != null)
            {
                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contactCount > 0 ? collision.GetContact(0).normal : Vector2.zero;
        HandleTrainingCollision(collision.collider, normal);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrainingCollision(other, Vector2.up);
    }

    private void HandleTrainingCollision(Collider2D other, Vector2 normal)
    {
        if (other == null)
        {
            return;
        }

        if (HasTagInHierarchy(other.transform, "Win"))
        {
            Win();
            return;
        }

        if (!HasTagInHierarchy(other.transform, "Enemy"))
        {
            return;
        }

        bool landedOnEnemy = normal.y > 0.35f || transform.position.y > other.bounds.center.y;
        if (!landedOnEnemy)
        {
            return;
        }

        RegisterEnemyStomp(other);
    }

    private void CheckTrainingOverlaps()
    {
        if (entity == null || entity.col == null)
        {
            return;
        }

        Bounds bounds = entity.col.bounds;
        Collider2D[] hits = Physics2D.OverlapBoxAll(bounds.center, bounds.size * 1.05f, 0f);
        foreach (var hit in hits)
        {
            if (hit == null || hit == entity.col)
            {
                continue;
            }

            if (HasTagInHierarchy(hit.transform, "Win"))
            {
                Win();
                return;
            }

            if (!HasTagInHierarchy(hit.transform, "Enemy"))
            {
                continue;
            }

            bool playerAboveEnemy = bounds.min.y >= hit.bounds.center.y - 0.05f;
            if (playerAboveEnemy)
            {
                RegisterEnemyStomp(hit);
            }
        }

        if (GetPrimaryTrainingTarget() == goalTransform && goalTransform != null && transform.position.x >= goalTransform.position.x - 0.5f)
        {
            Win();
        }
    }

    private void CheckEpisodeTimeout()
    {
        if (StepCount < MaxEpisodeSteps)
        {
            return;
        }

        AddReward(-2f);
        EndTrainingEpisode("timeout");
    }

    private void EndTrainingEpisode(string result)
    {
        if (episodeEnding)
        {
            return;
        }

        episodeEnding = true;
        float goalX = goalTransform != null ? goalTransform.position.x : 0f;
        float distanceToGoal = goalTransform != null ? Vector2.Distance(transform.position, goalTransform.position) : 0f;
        Debug.Log($"MLAGENTS_EPISODE_RESULT scene={SceneManager.GetActiveScene().name} result={result} steps={StepCount} reward={GetCumulativeReward():0.000} x={transform.position.x:0.000} goalX={goalX:0.000} distance={distanceToGoal:0.000} stomps={stompedEnemyCount} stompFirst={IsStompFirstMode()}");
        EndEpisode();
    }

    private void RewardGoalProgress()
    {
        Transform trainingTarget = GetPrimaryTrainingTarget();
        if (trainingTarget == null)
        {
            return;
        }

        float distanceToGoal = Vector2.Distance(transform.position, GetTargetPosition(trainingTarget));
        float progress = Mathf.Clamp(lastDistanceToGoal - distanceToGoal, -1f, 1f);
        AddReward(progress * 0.25f);
        lastDistanceToGoal = distanceToGoal;
    }

    private bool IsValidWinPosition()
    {
        if (goalTransform == null)
        {
            return true;
        }

        return transform.position.x >= goalTransform.position.x - 0.75f
            || Vector2.Distance(transform.position, goalTransform.position) <= 2.5f;
    }

    private bool HasTagInHierarchy(Transform target, string tagName)
    {
        while (target != null)
        {
            if (target.CompareTag(tagName))
            {
                return true;
            }

            target = target.parent;
        }

        return false;
    }

    private void DisableEnemy(Collider2D enemyCollider)
    {
        var enemyEntity = enemyCollider.GetComponentInParent<Entity>();
        GameObject enemyObject = enemyEntity != null ? enemyEntity.gameObject : enemyCollider.gameObject;

        if (!disabledEnemies.Contains(enemyObject))
        {
            disabledEnemies.Add(enemyObject);
        }

        if (enemyEntity != null)
        {
            enemyEntity.entityStop = true;
        }

        enemyObject.SetActive(false);
    }

    private void RegisterEnemyStomp(Collider2D enemyCollider)
    {
        if (enemyCollider == null)
        {
            return;
        }

        var enemyEntity = enemyCollider.GetComponentInParent<Entity>();
        GameObject enemyObject = enemyEntity != null ? enemyEntity.gameObject : enemyCollider.gameObject;
        if (disabledEnemies.Contains(enemyObject) || !enemyObject.activeInHierarchy)
        {
            return;
        }

        DisableEnemy(enemyCollider);
        stompedEnemyCount++;
        AddReward(IsStompFirstMode() ? 5f : 1f);
        Debug.Log($"MLAGENTS_ENEMY_STOMP scene={SceneManager.GetActiveScene().name} step={StepCount} count={stompedEnemyCount} x={transform.position.x:0.000}");

        if (entity != null && entity.rb != null)
        {
            entity.rb.velocity = new Vector2(entity.rb.velocity.x, 6f);
        }

        lastDistanceToGoal = goalTransform != null ? Vector2.Distance(transform.position, goalTransform.position) : 0f;
    }

    private Transform GetPrimaryTrainingTarget()
    {
        if (IsStompFirstMode() && stompedEnemyCount == 0)
        {
            Transform enemyTarget = FindActiveEnemyTarget();
            if (enemyTarget != null)
            {
                return enemyTarget;
            }
        }

        return goalTransform;
    }

    private Transform FindActiveEnemyTarget()
    {
        GameObject[] enemies;
        try
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
        catch (UnityException)
        {
            return null;
        }

        Transform best = null;
        float bestScore = float.MaxValue;
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeInHierarchy)
            {
                continue;
            }

            Collider2D enemyCollider = enemy.GetComponentInChildren<Collider2D>();
            if (enemyCollider != null && !enemyCollider.enabled)
            {
                continue;
            }

            Vector3 position = enemyCollider != null ? enemyCollider.bounds.center : enemy.transform.position;
            float score = Mathf.Abs(position.x - transform.position.x) + Mathf.Abs(position.y - transform.position.y) * 0.5f;
            if (goalTransform != null && position.x > goalTransform.position.x + 1f)
            {
                score += 50f;
            }

            if (score < bestScore)
            {
                bestScore = score;
                best = enemy.transform;
            }
        }

        return best;
    }

    private Vector3 GetTargetPosition(Transform target)
    {
        if (target == null)
        {
            return transform.position;
        }

        Collider2D targetCollider = target.GetComponentInChildren<Collider2D>();
        return targetCollider != null ? targetCollider.bounds.center : target.position;
    }

    private bool IsStompFirstMode()
    {
        return StompEnemyBeforeGoal;
    }

    private void EnsureDynamicRigidbody()
    {
        if (entity == null)
        {
            entity = GetComponent<Entity>();
        }

        if (entity == null)
        {
            return;
        }

        if (entity.rb == null)
        {
            entity.rb = GetComponent<Rigidbody2D>();
        }

        if (entity.rb == null)
        {
            return;
        }

        entity.rb.bodyType = RigidbodyType2D.Dynamic;
        entity.rb.simulated = true;
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 手动测试用
        var continuousActions = actionsOut.ContinuousActions;
        var discreteActions = actionsOut.DiscreteActions;

        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            continuousActions[0] = 0f;
            discreteActions[0] = 0;
            return;
        }

        continuousActions[0] = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
        discreteActions[0] = keyboard.spaceKey.isPressed ? 1 : 0;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 dir = goalTransform.position - transform.position;
        
        sensor.AddObservation(dir.normalized);
    }
    
    private float lastDistanceToGoal;
public override void OnEpisodeBegin()
{
    episodeEnding = false;
    EventBus.Publish(new End());
    EnsureDynamicRigidbody();
    stompedEnemyCount = 0;
    foreach (var enemy in disabledEnemies)
    {
        if (enemy != null)
        {
            enemy.SetActive(true);
            var enemyEntity = enemy.GetComponent<Entity>();
            if (enemyEntity != null)
            {
                enemyEntity.entityStop = false;
            }
        }
    }

    transform.position = initTransform.position;
    previousJumpPressed = false;
    lastJumpStep = -1000;
    Transform trainingTarget = GetPrimaryTrainingTarget();
    lastDistanceToGoal = trainingTarget != null ? Vector2.Distance(transform.position, GetTargetPosition(trainingTarget)) : 0f;

    entity.rb.velocity = Vector3.zero;
    entity.rb.angularVelocity = 0;
}
}
