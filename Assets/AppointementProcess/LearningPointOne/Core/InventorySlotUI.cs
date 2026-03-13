using UnityEngine;
using UnityEngine.UI;
using System;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI")]
    public Button button;                 // click area
    public Image iconImage;               // if you use sprite instead of prefab
    public RectTransform contentRoot;     // where to instantiate uiPrefab (can be the same as iconImage's parent)

    [HideInInspector] public ItemType itemType;
    GameObject spawned;

    public void Bind(CollectibleItemData data, Action<ItemType> onClick)
    {
        itemType = data.itemType;
        Clear();
        // prefab path
        if (data.uiPrefab && contentRoot)
        {
            spawned = Instantiate(data.uiPrefab, contentRoot);
            if (iconImage) iconImage.enabled = false; // prefab takes over
        }
        else
        {
            if (iconImage)
            {
                iconImage.enabled = true;
                iconImage.sprite = data.icon;
                iconImage.preserveAspect = true;
            }
        }

        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(itemType));
        }

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        if (spawned) Destroy(spawned);
        spawned = null;
        if (iconImage)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
        gameObject.SetActive(false);
    }
}