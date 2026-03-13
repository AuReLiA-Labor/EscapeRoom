using UnityEngine;

public class DocumentSigner_updated : MonoBehaviour
{
    public void OnSignButtonPressed()
    {
        var gm = GameManager_updated.Instance;
        if (!gm || !gm.Inventory) return;

        var inv = gm.Inventory;

        if (inv.HasPen && inv.HasDocument && !inv.DocumentSigned)
        {
            inv.SignDocument();
            gm.OnDocumentSigned();
        }
        else
        {
            gm.UI?.ShowHint("You need the digital pen and the document to sign.");
        }
    }
}