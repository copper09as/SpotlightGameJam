using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Battle.Entity;
using System;

public class EntityPreview : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject entityButtonPrefab;  // 实体按钮预制体
    [SerializeField] private Transform entityButtonContainer; // 按钮容器（如HorizontalLayoutGroup）
    [SerializeField] private GameObject contentPanel;         // 内容面板
    [SerializeField] private Button confirmBtn;
    [Header("Preview Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    [Header("Input Fields")]
    [SerializeField] private InputField effectIdInput;
    [SerializeField] private InputField gridWidthInput;
    [SerializeField] private InputField gridHeightInput;
    private int currentId;
    // 存储所有按钮
    private List<GameObject> entityButtons = new List<GameObject>();
    private List<Image> buttonImages = new List<Image>();
    
    // 当前选中的实体ID
    private int selectedEntityId = -1;
    
    // 事件：当选择实体时触发
    public System.Action<GameObject,int,int,int> OnEntitySelected;
    private EditorRepository editorRepository;
    
    // 加载实体到UI
    public void LoadEntities(EditorRepository repository)
    {
        // 清除现有按钮
        ClearButtons();
        this.editorRepository = repository;
        
        if (repository == null || repository.EntityPrefab == null)
        {
            Debug.LogError("EditorRepository is null or has no prefabs!");
            return;
        }
        
        // 遍历所有预制体
        for (int i = 0; i < repository.GetEntityCount(); i++)
        {
            GameObject prefab = repository.GetEntityPrefab(i);
            if (prefab == null) continue;
            
            // 获取预制体的Sprite
            Sprite entitySprite = GetPrefabSprite(prefab);
            
            // 创建按钮
            CreateEntityButton(i, entitySprite, prefab.name);
        }
        confirmBtn.onClick.AddListener(ConfirmPlace);
    }

    private void ConfirmPlace()
    {
        int effectId;
        int gridWidth;
        int gridHeight;
        if(!int.TryParse(effectIdInput.text,out effectId))
        {
            return;
        }
        if(!int.TryParse(gridHeightInput.text,out gridHeight))
        {
            return;
        }
        if(!int.TryParse(gridWidthInput.text,out gridWidth))
        {
             return;
        }
           
        
        OnEntitySelected?.Invoke(editorRepository.GetEntityPrefab(currentId),effectId,gridHeight,gridWidth);
    }

    // 获取预制体的Sprite
    private Sprite GetPrefabSprite(GameObject prefab)
    {
        // 尝试获取SpriteRenderer
        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            return sr.sprite;
        }
        
        // 尝试获取Image组件（如果预制体是UI元素）
        Image image = prefab.GetComponent<Image>();
        if (image != null)
        {
            return image.sprite;
        }
        
        // 尝试从子物体获取
        sr = prefab.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            return sr.sprite;
        }
        
        Debug.LogWarning($"Prefab {prefab.name} has no SpriteRenderer or Image component!");
        return null;
    }
    
    // 创建实体按钮
    private void CreateEntityButton(int entityId, Sprite sprite, string entityName)
    {
        if (entityButtonPrefab == null)
        {
            Debug.LogError("Entity button prefab is not assigned!");
            return;
        }
        
        // 实例化按钮
        GameObject buttonObj = Instantiate(entityButtonPrefab, entityButtonContainer);
        
        // 设置按钮名称
        buttonObj.name = $"EntityButton_{entityId}_{entityName}";
        
        // 获取或添加Image组件显示Sprite
        Image buttonImage = buttonObj.GetComponent<Image>();
        if (buttonImage != null && sprite != null)
        {
            buttonImage.sprite = sprite;
            buttonImage.preserveAspect = true;
        }
        
        // 获取Button组件并添加点击事件
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            int capturedId = entityId; // 捕获变量避免闭包问题
            button.onClick.AddListener(() => SelectEntity(capturedId));
        }
        
        // 添加EventTrigger用于悬停效果
        AddHoverEffect(buttonObj, entityId);
        
        // 存储引用
        entityButtons.Add(buttonObj);
        buttonImages.Add(buttonImage);
    }
    
    // 选择实体
    public void SelectEntity(int entityId)
    {
        // 取消之前的选择
        DeselectAll();
        
        // 设置新的选择
        selectedEntityId = entityId;
        
        // 高亮选中的按钮
        if (entityId >= 0 && entityId < buttonImages.Count)
        {
            if (buttonImages[entityId] != null)
            {
                buttonImages[entityId].color = selectedColor;
            }
            
            // 可以添加边框或其他选中效果
            AddSelectionBorder(entityButtons[entityId]);
        }
        currentId = entityId;
        // 触发选择事件
        //OnEntitySelected?.Invoke(editorRepository.GetEntityPrefab(entityId));
        
        Debug.Log($"Selected entity: {entityId}");
    }
    
    // 取消所有选择
    private void DeselectAll()
    {
        foreach (var image in buttonImages)
        {
            if (image != null)
            {
                image.color = normalColor;
            }
        }
        
        // 移除所有选中边框
        foreach (var button in entityButtons)
        {
            RemoveSelectionBorder(button);
        }
        
        selectedEntityId = -1;
    }
    
    // 添加选中边框
    private void AddSelectionBorder(GameObject buttonObj)
    {
        // 可以添加边框效果，比如改变边框图片
        Outline outline = buttonObj.GetComponent<Outline>();
        if (outline == null)
        {
            outline = buttonObj.AddComponent<Outline>();
        }
        outline.effectColor = selectedColor;
        outline.effectDistance = new Vector2(3, 3);
        outline.enabled = true;
    }
    
    // 移除选中边框
    private void RemoveSelectionBorder(GameObject buttonObj)
    {
        Outline outline = buttonObj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
    
    // 添加悬停效果
    private void AddHoverEffect(GameObject buttonObj, int entityId)
    {
        EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = buttonObj.AddComponent<EventTrigger>();
        }
        
        // 鼠标进入
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            if (entityId != selectedEntityId && buttonImages[entityId] != null)
            {
                buttonImages[entityId].color = new Color(0.8f, 0.8f, 1f);
            }
        });
        trigger.triggers.Add(enterEntry);
        
        // 鼠标离开
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            if (entityId != selectedEntityId && buttonImages[entityId] != null)
            {
                buttonImages[entityId].color = normalColor;
            }
        });
        trigger.triggers.Add(exitEntry);
    }
    
    // 清除所有按钮
    private void ClearButtons()
    {
        foreach (var button in entityButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }
        
        entityButtons.Clear();
        buttonImages.Clear();
        selectedEntityId = -1;
    }
    
    // 获取当前选中的实体ID
    public int GetSelectedEntityId()
    {
        return selectedEntityId;
    }
    
    // 显示/隐藏面板
    public void ShowPanel()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(true);
        }
    }
    
    public void HidePanel()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(false);
        }
    }
    
    public void TogglePanel()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(!contentPanel.activeSelf);
        }
    }
}