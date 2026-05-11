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
    [SerializeField]private bool cycleTrainingScenes;
    private const int MaxEpisodeSteps = 1000;
    private static readonly string[] TrainingSceneNames = { "LEVELTEST01", "LEVELTEST02", "LEVELTEST03" };
    private BehaviorParameters behaviorParameters;
    private bool previousJumpPressed;
    private bool episodeEnding;
    private int stompedEnemyCount;
    private int jumpCount;
    private int unneededJumpCount;
    private int backwardJumpCount;
    private int lastJumpStep = -1000;
    private int lastJumpRequestStep = -1000;
    private readonly List<GameObject> disabledEnemies = new();
    private bool hasPreviousPlayerBounds;
    private Bounds previousPlayerBounds;
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
         if (!IsValidWinPosition())
         {
             return;
         }

         AddReward(10f);
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

    if (!ShouldUseModelOrTrainerPolicy())
    {
        SyncGameController(0f, false);
        return;
    }

    SyncGameController(moveX, jumpPressed);
    ApplyAgentPhysics(moveX, jumpPressed);
    RewardObstacleJump(moveX, jumpPressed);
    RewardActionDiscipline(moveX, jumpPressed);
    CheckTrainingOverlaps();

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

        float targetSpeed = moveIntent * targetMoveSpeed;
        float speedDiff = targetSpeed - entity.rb.velocity.x;
        entity.rb.AddForce(new Vector2(speedDiff * acceleration, 0f), ForceMode2D.Force);

        if (jumpPressed)
        {
            lastJumpRequestStep = StepCount;
        }

        bool hasBufferedJump = StepCount - lastJumpRequestStep <= 6;
        if (hasBufferedJump && IsGrounded() && StepCount - lastJumpStep > 8)
        {
            jumpCount++;
            bool needsJumpAhead = NeedsJumpTowardGoal();
            bool movingTowardGoal = IsMovingTowardGoal(moveX);
            if (!needsJumpAhead)
            {
                unneededJumpCount++;
                if (Academy.Instance.IsCommunicatorOn)
                {
                    AddReward(-0.04f);
                }
            }

            if (!movingTowardGoal && Mathf.Abs(moveX) > 0.05f)
            {
                backwardJumpCount++;
                if (Academy.Instance.IsCommunicatorOn)
                {
                    AddReward(-0.05f);
                }
            }

            entity.rb.velocity = new Vector2(entity.rb.velocity.x, jumpVelocity);
            lastJumpStep = StepCount;
            lastJumpRequestStep = -1000;
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

        int rayMask = GetGroundRayMask();
        float[] offsets = { -halfWidth * 0.5f, 0f, halfWidth * 0.5f };
        foreach (float offset in offsets)
        {
            Vector2 origin = new Vector2(center.x + offset, bottomY + 0.02f);
            if (Physics2D.Raycast(origin, Vector2.down, 0.45f, rayMask).collider != null)
            {
                return true;
            }
        }

        Vector2 boxCenter = new Vector2(center.x, bottomY - 0.04f);
        Vector2 boxSize = new Vector2(Mathf.Max(halfWidth * 1.4f, 0.2f), 0.12f);
        foreach (var hit in Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, rayMask))
        {
            if (hit != null && hit != col)
            {
                return true;
            }
        }

        return false;
    }

    private int GetGroundRayMask()
    {
        int mask = LayerMask.GetMask("Ground");
        return mask == 0 ? Physics2D.DefaultRaycastLayers : mask;
    }

    private void RewardObstacleJump(float moveX, bool jumpPressed)
    {
        if (!Academy.Instance.IsCommunicatorOn || StepCount % 10 != 0 || Mathf.Abs(moveX) <= 0.05f || entity == null || entity.rb == null)
        {
            return;
        }

        Collider2D col = entity.col != null ? entity.col : GetComponent<Collider2D>();
        if (col == null)
        {
            return;
        }

        bool grounded = IsGrounded();
        float direction = Mathf.Sign(moveX);
        Vector2 origin = new Vector2(col.bounds.center.x + direction * col.bounds.extents.x, col.bounds.center.y);
        bool obstacleAhead = Physics2D.Raycast(origin, new Vector2(direction, 0f), 0.65f, GetGroundRayMask()).collider != null;
        bool almostStopped = Mathf.Abs(entity.rb.velocity.x) < 0.05f;
        bool enemyAhead = HasEnemyAhead(direction);
        bool needsJumpAhead = obstacleAhead || HasGapAhead(direction) || enemyAhead;
        bool movingTowardGoal = IsMovingTowardGoal(moveX);
        bool nearGoal = IsNearGoal();

        if (!movingTowardGoal)
        {
            return;
        }

        if (grounded && needsJumpAhead)
        {
            AddReward(jumpPressed ? (enemyAhead ? 0.18f : 0.08f) : (enemyAhead ? -0.06f : -0.018f));
        }
        else if (grounded && jumpPressed)
        {
            AddReward(nearGoal ? -0.08f : -0.04f);
        }
        else if (grounded && almostStopped)
        {
            AddReward(-0.006f);
        }
        else if (grounded)
        {
            AddReward(0.002f);
        }
    }

    private void RewardActionDiscipline(float moveX, bool jumpPressed)
    {
        if (!Academy.Instance.IsCommunicatorOn)
        {
            return;
        }

        if (IsMovingTowardGoal(moveX) && Mathf.Abs(moveX) > 0.05f)
        {
            AddReward(0.003f * Mathf.Abs(moveX));
        }
        else if (Mathf.Abs(moveX) > 0.05f)
        {
            AddReward(-0.03f * Mathf.Abs(moveX));
        }

        bool grounded = IsGrounded();
        bool needsJumpAhead = NeedsJumpTowardGoal();
        if (jumpPressed && !needsJumpAhead)
        {
            AddReward(grounded ? -0.03f : -0.008f);
        }

        if (jumpPressed && IsNearGoal())
        {
            AddReward(-0.04f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contactCount > 0 ? collision.GetContact(0).normal : Vector2.zero;
        HandleTrainingCollision(collision.collider, normal);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrainingCollision(other, Vector2.zero);
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

        if (!IsJumpStomp(other, normal))
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

            if (IsJumpStomp(hit, Vector2.zero))
            {
                RegisterEnemyStomp(hit);
            }
        }

        CheckEnemyStompBelow(bounds, hasPreviousPlayerBounds ? previousPlayerBounds : bounds);
        previousPlayerBounds = bounds;
        hasPreviousPlayerBounds = true;

        if (GetPrimaryTrainingTarget() == goalTransform && goalTransform != null && transform.position.x >= goalTransform.position.x - 0.5f)
        {
            Win();
        }
    }

    private bool IsJumpStomp(Collider2D enemyCollider, Vector2 contactNormal)
    {
        if (enemyCollider == null || entity == null || entity.col == null || entity.rb == null)
        {
            return false;
        }

        Bounds playerBounds = entity.col.bounds;
        Bounds enemyBounds = enemyCollider.bounds;
        bool horizontallyOverlapping = playerBounds.max.x > enemyBounds.min.x + 0.04f &&
            playerBounds.min.x < enemyBounds.max.x - 0.04f;
        bool feetAboveEnemy = playerBounds.min.y >= enemyBounds.center.y &&
            playerBounds.min.y >= enemyBounds.max.y - 0.35f;
        bool fallingOntoEnemy = entity.rb.velocity.y <= 0.2f || contactNormal.y > 0.65f;
        bool recentlyJumped = StepCount - lastJumpStep >= 0 && StepCount - lastJumpStep <= 90;

        return horizontallyOverlapping && feetAboveEnemy && fallingOntoEnemy && recentlyJumped;
    }

    private void CheckEnemyStompBelow(Bounds playerBounds, Bounds previousBounds)
    {
        if (entity == null || entity.rb == null || entity.col == null)
        {
            return;
        }

        bool recentlyJumped = StepCount - lastJumpStep >= 0 && StepCount - lastJumpStep <= 90;
        if (!recentlyJumped)
        {
            return;
        }

        float minX = Mathf.Min(playerBounds.min.x, previousBounds.min.x) - 0.18f;
        float maxX = Mathf.Max(playerBounds.max.x, previousBounds.max.x) + 0.18f;
        float highestFoot = Mathf.Max(playerBounds.min.y, previousBounds.min.y);
        float lowestFoot = Mathf.Min(playerBounds.min.y, previousBounds.min.y);

        Vector2 stompCenter = new Vector2((minX + maxX) * 0.5f, lowestFoot - 0.7f);
        Vector2 stompSize = new Vector2(Mathf.Max(maxX - minX, 0.45f), 1.7f);
        foreach (var hit in Physics2D.OverlapBoxAll(stompCenter, stompSize, 0f))
        {
            if (hit == null || hit == entity.col || !HasTagInHierarchy(hit.transform, "Enemy"))
            {
                continue;
            }

            if (IsTopStompCandidate(playerBounds, previousBounds, hit.bounds, minX, maxX, highestFoot, lowestFoot))
            {
                RegisterEnemyStomp(hit);
                return;
            }
        }

        float[] offsets = { -playerBounds.extents.x * 0.65f, 0f, playerBounds.extents.x * 0.65f };
        foreach (float offset in offsets)
        {
            Vector2 origin = new Vector2(playerBounds.center.x + offset, playerBounds.min.y + 0.02f);
            foreach (var hit in Physics2D.RaycastAll(origin, Vector2.down, 2.8f))
            {
                if (hit.collider == null || hit.collider == entity.col || !HasTagInHierarchy(hit.collider.transform, "Enemy"))
                {
                    continue;
                }

                if (IsTopStompCandidate(playerBounds, previousBounds, hit.collider.bounds, minX, maxX, highestFoot, lowestFoot))
                {
                    RegisterEnemyStomp(hit.collider);
                    return;
                }
            }
        }
    }

    private bool IsTopStompCandidate(Bounds playerBounds, Bounds previousBounds, Bounds enemyBounds, float minX, float maxX, float highestFoot, float lowestFoot)
    {
        bool sweptAcrossEnemy = maxX > enemyBounds.min.x + 0.03f &&
            minX < enemyBounds.max.x - 0.03f;
        bool playerBodyAboveEnemy = Mathf.Max(playerBounds.center.y, previousBounds.center.y) >= enemyBounds.center.y;
        bool enemyTopUnderFeet = enemyBounds.max.y <= highestFoot + 0.18f &&
            enemyBounds.max.y >= lowestFoot - 1.7f;
        bool centerPassedOverEnemy = Mathf.Max(playerBounds.center.x, previousBounds.center.x) >= enemyBounds.min.x - 0.1f &&
            Mathf.Min(playerBounds.center.x, previousBounds.center.x) <= enemyBounds.max.x + 0.1f;
        bool fallingOrPastJumpApex = entity.rb.velocity.y <= 1.5f || StepCount - lastJumpStep > 16 || previousBounds.min.y >= playerBounds.min.y;

        return sweptAcrossEnemy && playerBodyAboveEnemy && enemyTopUnderFeet && centerPassedOverEnemy && fallingOrPastJumpApex;
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
        string nextTrainingScene = cycleTrainingScenes && Academy.Instance.IsCommunicatorOn ? GetNextTrainingSceneName() : null;
        Debug.Log($"MLAGENTS_EPISODE_RESULT scene={SceneManager.GetActiveScene().name} result={result} steps={StepCount} reward={GetCumulativeReward():0.000} x={transform.position.x:0.000} goalX={goalX:0.000} distance={distanceToGoal:0.000} stomps={stompedEnemyCount} jumps={jumpCount} unneededJumps={unneededJumpCount} backwardJumps={backwardJumpCount} stompFirst=False");
        EndEpisode();
        if (!string.IsNullOrEmpty(nextTrainingScene) && nextTrainingScene != SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene(nextTrainingScene);
        }
    }

    private string GetNextTrainingSceneName()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        for (int i = 0; i < TrainingSceneNames.Length; i++)
        {
            if (TrainingSceneNames[i] == currentScene)
            {
                return TrainingSceneNames[(i + 1) % TrainingSceneNames.Length];
            }
        }

        return null;
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
        if (SceneManager.GetActiveScene().name == "LEVELTEST03" && stompedEnemyCount <= 0)
        {
            return false;
        }

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

        stompedEnemyCount++;
        if (Academy.Instance.IsCommunicatorOn)
        {
            AddReward(2f);
        }

        Debug.Log($"MLAGENTS_ENEMY_STOMP scene={SceneManager.GetActiveScene().name} step={StepCount} count={stompedEnemyCount} x={transform.position.x:0.000}");

        DisableEnemy(enemyCollider);

        if (entity != null && entity.rb != null)
        {
            entity.rb.velocity = new Vector2(entity.rb.velocity.x, 6f);
        }

        lastDistanceToGoal = goalTransform != null ? Vector2.Distance(transform.position, goalTransform.position) : 0f;
    }

    private Transform GetPrimaryTrainingTarget()
    {
        return goalTransform;
    }

    private bool IsMovingTowardGoal(float moveX)
    {
        if (goalTransform == null || Mathf.Abs(moveX) <= 0.05f)
        {
            return true;
        }

        float targetDirection = Mathf.Sign(goalTransform.position.x - transform.position.x);
        if (Mathf.Abs(targetDirection) <= 0.01f)
        {
            return true;
        }

        return moveX * targetDirection >= -0.05f;
    }

    private bool HasGapAhead(float direction)
    {
        Collider2D col = entity != null ? entity.col : null;
        if (col == null)
        {
            col = GetComponent<Collider2D>();
        }

        if (col == null)
        {
            return false;
        }

        Bounds bounds = col.bounds;
        Vector2 origin = new Vector2(bounds.center.x + direction * (bounds.extents.x + 0.55f), bounds.min.y + 0.1f);
        return Physics2D.Raycast(origin, Vector2.down, 1.0f, GetGroundRayMask()).collider == null;
    }

    private bool NeedsJumpTowardGoal()
    {
        if (goalTransform == null)
        {
            return false;
        }

        float direction = Mathf.Sign(goalTransform.position.x - transform.position.x);
        if (Mathf.Abs(direction) <= 0.01f)
        {
            return false;
        }

        Collider2D col = entity != null ? entity.col : null;
        if (col == null)
        {
            col = GetComponent<Collider2D>();
        }

        if (col == null)
        {
            return false;
        }

        Vector2 wallOrigin = new Vector2(col.bounds.center.x + direction * col.bounds.extents.x, col.bounds.center.y);
        bool obstacleAhead = Physics2D.Raycast(wallOrigin, new Vector2(direction, 0f), 0.65f, GetGroundRayMask()).collider != null;
        return obstacleAhead || HasGapAhead(direction) || HasEnemyAhead(direction);
    }

    private bool HasEnemyAhead(float direction)
    {
        if (Mathf.Abs(direction) <= 0.01f)
        {
            return false;
        }

        Collider2D col = entity != null ? entity.col : null;
        if (col == null)
        {
            col = GetComponent<Collider2D>();
        }

        if (col == null)
        {
            return false;
        }

        Bounds bounds = col.bounds;
        Vector2 center = new Vector2(bounds.center.x + direction * (bounds.extents.x + 0.75f), bounds.center.y);
        Vector2 size = new Vector2(1.5f, Mathf.Max(bounds.size.y * 1.5f, 1.0f));
        foreach (var hit in Physics2D.OverlapBoxAll(center, size, 0f))
        {
            if (hit != null && hit != col && HasTagInHierarchy(hit.transform, "Enemy"))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsNearGoal()
    {
        if (goalTransform == null)
        {
            return false;
        }

        return Vector2.Distance(transform.position, GetTargetPosition(goalTransform)) <= 2.8f;
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

    private bool ShouldUseModelOrTrainerPolicy()
    {
        if (behaviorParameters == null)
        {
            behaviorParameters = GetComponent<BehaviorParameters>();
        }

        if (behaviorParameters == null || behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly)
        {
            return false;
        }

        return behaviorParameters.Model != null || Academy.Instance.IsCommunicatorOn;
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

        if (entity.col == null)
        {
            entity.col = GetComponent<Collider2D>();
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
        Vector3 dir = goalTransform != null ? goalTransform.position - transform.position : Vector3.zero;
        sensor.AddObservation(Mathf.Clamp(dir.x / 10f, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(dir.y / 5f, -1f, 1f));
        sensor.AddObservation(NeedsJumpTowardGoal() ? 1f : 0f);
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
    jumpCount = 0;
    unneededJumpCount = 0;
    backwardJumpCount = 0;
    lastJumpStep = -1000;
    lastJumpRequestStep = -1000;
    hasPreviousPlayerBounds = false;
    Transform trainingTarget = GetPrimaryTrainingTarget();
    lastDistanceToGoal = trainingTarget != null ? Vector2.Distance(transform.position, GetTargetPosition(trainingTarget)) : 0f;

    entity.rb.velocity = Vector3.zero;
    entity.rb.angularVelocity = 0;
}
}
