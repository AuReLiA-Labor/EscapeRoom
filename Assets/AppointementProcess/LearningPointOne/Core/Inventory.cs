using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool HasPen { get; private set; } = false;
    public bool HasDocument { get; private set; } = false;
    public bool HasKey { get; private set; } = false;
    public bool DocumentSigned { get; private set; } = false;
    public event Action<Inventory> OnChanged;
    public void Add(ItemType type)
    {
        switch (type)
        {
            case ItemType.Pen: HasPen = true; break;
            case ItemType.Document: HasDocument = true; break;
            case ItemType.Key: HasKey = true; break;
        }
    }

    public void SignDocument()
    {
        if (HasPen && HasDocument)
        {
            DocumentSigned = true;
        }
    }

    public void UseKey()
    {
        if (HasKey) HasKey = false; // consume key if desired
    }

    public void ResetAll()
    {
        HasPen = HasDocument = HasKey = DocumentSigned = false;
    }
}