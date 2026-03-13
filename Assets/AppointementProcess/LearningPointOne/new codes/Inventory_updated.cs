using System;
using UnityEngine;

public class Inventory_updated : MonoBehaviour
{
    public bool HasPen { get; private set; }
    public bool HasDocument { get; private set; }
    public bool HasKey { get; private set; }
    public bool DocumentSigned { get; private set; }

    public event Action OnChanged;

    public void Add(ItemType_updated type)
    {
        switch (type)
        {
            case ItemType_updated.Pen: HasPen = true; break;
            case ItemType_updated.Document: HasDocument = true; break;
            case ItemType_updated.Key: HasKey = true; break;
        }
        OnChanged?.Invoke();
    }

    public void SignDocument()
    {
        if (HasPen && HasDocument)
        {
            DocumentSigned = true;
            OnChanged?.Invoke();
        }
    }

    public void ResetAll()
    {
        HasPen = HasDocument = HasKey = DocumentSigned = false;
        OnChanged?.Invoke();
    }
}