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
            currentEnemy = currentEnemyObj.GetComponent<Entity>();
            currentEnemy.Init(streaming.entityManager);
        }
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