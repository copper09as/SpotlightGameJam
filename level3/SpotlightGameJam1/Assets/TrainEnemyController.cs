using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using UnityEngine;

public class TrainEnemyController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;  // 改成预制体
    [SerializeField] private Transform enemySpawnPoint;  // 出生点
    [SerializeField] private Entity currentEnemy;
    [SerializeField]private BattleStreaming streaming;
    
    private GameObject currentEnemyObj;
    private bool isSpawning;
    
    void Start()
    {
        EventBus.Subscribe<End>(End);
        SpawnEnemy();  // 初始生成
    }
    
    void End(End eve)
    {
        // 回合结束时重新生成敌人
        RespawnEnemy();
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            // 生成新敌人
            Vector3 spawnPos = enemySpawnPoint != null ? enemySpawnPoint.position : transform.position;
            currentEnemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            currentEnemyObj.tag = "Enemy";
            currentEnemy = currentEnemyObj.GetComponent<Entity>();
            if (currentEnemy != null && IsReadyToSpawn())
            {
                streaming.entityManager.Register(currentEnemy);
            }
            else
            {
                PrepareFallbackEnemy(currentEnemyObj);
                StartCoroutine(RegisterEnemyWhenReady());
            }
        }
    }

    private IEnumerator RegisterEnemyWhenReady()
    {
        if (isSpawning)
        {
            yield break;
        }

        isSpawning = true;
        while (currentEnemyObj != null && currentEnemy != null && !IsReadyToSpawn())
        {
            yield return null;
        }

        if (currentEnemyObj != null && currentEnemy != null)
        {
            streaming.entityManager.Register(currentEnemy);
        }

        isSpawning = false;
    }

    private void PrepareFallbackEnemy(GameObject enemyObj)
    {
        foreach (var transformInChild in enemyObj.GetComponentsInChildren<Transform>(true))
        {
            transformInChild.gameObject.tag = "Enemy";
        }

        var rb = enemyObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }

        var colliders = enemyObj.GetComponentsInChildren<Collider2D>(true);
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
    }

    private bool IsReadyToSpawn()
    {
        return streaming != null
            && streaming.entityManager != null
            && LuaManager.Instance != null
            && LuaManager.Instance._luaEnv != null
            && Global.Data.GameConfig.Instance != null
            && Global.Data.GameConfig.Instance.CommonEDC != null
            && Global.Data.GameConfig.Instance.EntitySDC != null;
    }
    
    void RespawnEnemy()
    {
        // 先销毁旧敌人
        if (currentEnemyObj != null)
        {
            Destroy(currentEnemyObj);
        }
        
        // 生成新敌人
        SpawnEnemy();
    }
    
    void OnDestroy()
    {
        EventBus.Unsubscribe<End>(End);
    }
}

public struct End
{
    
}
