using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using Global.Data;
using Global.Data.BattleConfig;
using UnityEngine;

public class BattleStreaming : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject tileMapPre;
    [Header("Entities")]
    [SerializeField] private List<GameObject> entitiesInCanvas = new List<GameObject>();
    [SerializeField] private List<GameObject> entitiesInWorld = new List<GameObject>();

    [Header("UI")]
    [SerializeField] private GameObject loadingPanel;
    [Header("父物体")]
    [SerializeField] private Transform decorationTransform;
    [SerializeField] private Transform canvaTransform;
    [SerializeField] private Transform worldTransform;
    [Header("场景装饰")]
    [SerializeField] private List<GameObject> decorations;
    
    //[SerializeField] private EntityUIManager entityUIManager;
    private EntityManager entityManager;

    private GameObject currentTileMap;
    private bool isLoading = false;

    void Start()
    {
        entityManager = new EntityManager();
        AudioManager.Instance.PlayBGM(StringResource.BattleBgmPath);
        Time.timeScale = 1f;
        StartCoroutine(LoadBattle(BattleConfig.Instance.levelId));
    }



    /// <summary>
    /// 异步加载战斗场景
    /// </summary>
    public IEnumerator LoadBattle(int levelId)
    {
        if (isLoading) yield break;
        isLoading = true;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            var text = loadingPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
            {
                text.text = GameConfig.Instance.LevtlDC.levelDataList.Find(i=>i.Id == levelId).SceneName;
            }
        }
           

        yield return new WaitForSeconds(0.2f);
        //加载tileMap
        currentTileMap = Instantiate(tileMapPre);
        yield return new WaitForSeconds(0.2f);
        GameObject settingMenu = null;
        GameObject audioMenu = null;
        GameObject cameraMenu = null;
        foreach (var entityPrefabParents in entitiesInCanvas)
        {
            var parentObj = Instantiate(entityPrefabParents, canvaTransform);
            var objName = parentObj.name;

            if (objName.Contains("SettingMenu"))
                settingMenu = parentObj;
            else if (objName.Contains("AudioMenu"))
                audioMenu = parentObj;
            else if (objName.Contains("CameraMenu"))
                cameraMenu = parentObj;
            foreach (var entity in parentObj.GetComponentsInChildren<Entity>())
            {
                entityManager.Register(entity);
            }
        }
        EntityUIManager.Instance.Init(settingMenu, audioMenu, cameraMenu);
        yield return new WaitForSeconds(0.2f);
        foreach (var decorate in decorations)
        {
            Instantiate(decorate, decorationTransform);
        }
        yield return new WaitForSeconds(0.2f);
        //加载在世界空间中的entity
        foreach (var entityPrefab in entitiesInWorld)
        {
            if (entityPrefab != null)
            {
                var entity = entityManager.InstantiateEnityty(entityPrefab, worldTransform);
            }
        }
        // 加载完成

        foreach (var entity in entityManager.GetAllEntities())
        {
            entity.Init(entityManager);
        }
       StartCoroutine(FadeOutLoadingPanel(1f));
        isLoading = false;
        Debug.Log("Battle loaded!");
    }
    private IEnumerator FadeOutLoadingPanel(float duration = 1f)
    {
        if (loadingPanel == null) yield break;

        var canvasGroup = loadingPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        loadingPanel.SetActive(false); // 淡出完成后再隐藏
    }
}
