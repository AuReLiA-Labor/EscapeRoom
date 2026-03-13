using UnityEngine;

[CreateAssetMenu(menuName = "Game/Collectible Item Data")]
public class CollectibleItemData : ScriptableObject
{
    public ItemType itemType;                 // Pen, Document, Key
    public string displayName;                // e.g., "Digital Pen"
    public Sprite icon;                       // fallback icon for slots/detail
    [TextArea(3, 8)] public string description;

    [Header("Optional UI Prefab")]
    [Tooltip("Optional prefab to instantiate in slot/detail (must have RectTransform; usually contains an Image). If null, 'icon' is used.")]
    public GameObject uiPrefab;
}