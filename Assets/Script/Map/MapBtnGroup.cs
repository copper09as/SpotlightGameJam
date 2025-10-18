using System.Collections;
using System.Collections.Generic;
using Global.Data;
using Global.Data.BattleConfig;
using UnityEngine;
using UnityEngine.UI;

public class MapBtnGroup : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    private readonly List<Button> mapSelectButtonGroup = new List<Button>();

    private void Awake()
    {
        StartCoroutine(LoadMapButtonsCoroutine());
    }

    private IEnumerator LoadMapButtonsCoroutine()
    {

        //var bossDatas = GameConfig.Instance.bossCollection.bossDataList;
        int unlockedIndex = 20;//SaveManager.Instance.userProgressData.levelIndex;
        var levelDataList = GameConfig.Instance.LevtlDC.levelDataList;
        for (int i = 0; i < levelDataList.Count; i++)
        {
            var levelData = levelDataList[i];

            var btn = CreateButton(levelData.Id, levelData.SpritePath);

            btn.interactable = false;


            /*if (i > unlockedIndex)
            {
                btn.GetComponent<Image>().sprite =
                    ResManager.LoadSprite(StringResource.GetImagePath(levelData.levelLockPath));
            }*/

            yield return null;
        }

        for (int i = 0; i < mapSelectButtonGroup.Count; i++)
        {
            if (i <= unlockedIndex)
                mapSelectButtonGroup[i].interactable = true;
            else
                mapSelectButtonGroup[i].interactable = false;
        }
    }

    private Button CreateButton(int id, string spritePath)
    {
        var btnPrefab = buttonPrefab;
        var btnInstance = Instantiate(btnPrefab, transform);
        var btn = btnInstance.GetComponent<Button>();
        if(spritePath!=null&&spritePath.Length != 0)
        {
            var image = btn.GetComponent<Image>();
            image.sprite = ResManager.LoadSprite(StringResource.GetImagePath(spritePath));
        }
        int captureId = id;
        btn.onClick.AddListener(() => EnterByIndex(captureId));

        mapSelectButtonGroup.Add(btn);
        return btn;
    }

    private void EnterByIndex(int id)
    {
        BattleConfig.Instance.levelId = id; 
        //GlobalConfig.Instance.bossIndex = index;
        SceneChangeManager.Instance.LoadScene("BattleTest");
    }
}
