using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private Inventory inventory;                 // auto-find if left empty
    [SerializeField] private GameObject panelRoot;                // the whole InventoryUI panel (for show/hide)

    [Header("Database")]
    [SerializeField] private List<CollectibleItemData> items;     // assign 3 assets here (Pen, Document, Key)

    [Header("Left: Slots")]
    [SerializeField] private Transform slotsParent;               // the grid container (InventorySlots)
    [SerializeField] private InventorySlotUI slotPrefab;          // the UI prefab for one slot
    [SerializeField] private int maxVisibleSlots = 16;            // grid capacity (your design shows many cells)

    [Header("Right: Description Panel")]
    [SerializeField] private RectTransform detailImageHost;       // the small square above
    [SerializeField] private Image detailFallbackImage;           // used if no prefab
    [SerializeField] private TMP_Text detailText;                 // the big text box below

    // runtime
    readonly Dictionary<ItemType, CollectibleItemData> _db = new();
    readonly List<InventorySlotUI> _slots = new();
    GameObject _detailSpawned;

    void Awake()
    {
        if (!inventory) inventory = FindObjectOfType<Inventory>();
        BuildDatabase();
        EnsureSlotPool();
        ClearDetail();
        if (panelRoot) panelRoot.SetActive(false); // start hidden
    }

    void OnEnable()
    {
        if (inventory) inventory.OnChanged += OnInventoryChanged;
        OnInventoryChanged(inventory);
    }
    void OnDisable()
    {
        if (inventory) inventory.OnChanged -= OnInventoryChanged;
    }

    void BuildDatabase()
    {
        _db.Clear();
        foreach (var d in items)
        {
            if (!d) continue;
            _db[d.itemType] = d;
        }
    }

    void EnsureSlotPool()
    {
        _slots.Clear();
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            var s = slotsParent.GetChild(i).GetComponent<InventorySlotUI>();
            if (s) _slots.Add(s);
        }
        // top up if not enough
        while (_slots.Count < maxVisibleSlots && slotPrefab)
        {
            var inst = Instantiate(slotPrefab, slotsParent);
            _slots.Add(inst);
        }
        // start hidden
        foreach (var s in _slots) s.Clear();
    }

    void OnInventoryChanged(Inventory inv)
    {
        // compute owned items (Pen, Document, Key)
        var owned = new List<ItemType>(3);
        if (inv != null)
        {
            if (inv.HasPen) owned.Add(ItemType.Pen);
            if (inv.HasDocument) owned.Add(ItemType.Document);
            if (inv.HasKey) owned.Add(ItemType.Key);
        }

        // fill slots [0..owned.Count), hide the rest
        int i = 0;
        for (; i < owned.Count && i < _slots.Count; i++)
        {
            var type = owned[i];
            if (_db.TryGetValue(type, out var data))
                _slots[i].Bind(data, OnSlotClicked);
            else
                _slots[i].Clear();
        }
        for (; i < _slots.Count; i++)
            _slots[i].Clear();

        // if nothing selected anymore, clear detail; else keep current
        if (owned.Count == 0) ClearDetail();
    }

    void OnSlotClicked(ItemType type)
    {
        if (!_db.TryGetValue(type, out var data)) return;
        ShowDetail(data);
    }

    void ShowDetail(CollectibleItemData data)
    {
        // clear previous
        if (_detailSpawned) Destroy(_detailSpawned);
        _detailSpawned = null;
        if (detailFallbackImage)
        {
            detailFallbackImage.sprite = null;
            detailFallbackImage.enabled = false;
            detailFallbackImage.preserveAspect = true;
        }

        // spawn prefab OR show fallback icon
        if (data.uiPrefab && detailImageHost)
        {
            _detailSpawned = Instantiate(data.uiPrefab, detailImageHost);
        }
        else if (detailFallbackImage)
        {
            detailFallbackImage.sprite = data.icon;
            detailFallbackImage.enabled = data.icon != null;
        }

        // compose description; add live status (e.g., Signed?)
        string extra = "";
        if (inventory)
        {
            if (data.itemType == ItemType.Document)
                extra = $"\n\nSigned: {(inventory.DocumentSigned ? "Yes" : "No")}";
        }

        if (detailText)
            detailText.text = $"<b>{data.displayName}</b>\n{data.description}{extra}";
    }

    void ClearDetail()
    {
        if (_detailSpawned) Destroy(_detailSpawned);
        _detailSpawned = null;
        if (detailFallbackImage)
        {
            detailFallbackImage.sprite = null;
            detailFallbackImage.enabled = false;
        }
        if (detailText) detailText.text = string.Empty;
    }

    // ===== Public API for your toggle button =====
    public void TogglePanel()
    {
        if (!panelRoot) return;
        panelRoot.SetActive(!panelRoot.activeSelf);
        // refresh contents when opening
        if (panelRoot.activeSelf) OnInventoryChanged(inventory);
    }

    public void ShowPanel()
    {
        if (!panelRoot) return;
        if (!panelRoot.activeSelf)
        {
            panelRoot.SetActive(true);
            OnInventoryChanged(inventory);
        }
    }

    public void HidePanel()
    {
        if (!panelRoot) return;
        panelRoot.SetActive(false);
    }
}
