using UnityEngine;

public class DocumentSigner : MonoBehaviour
{
    public void OnSignButtonPressed()
    {
        if (!GameManager.Instance || !GameManager.Instance.inventory) return;
        var inv = GameManager.Instance.inventory;

        if (inv.HasPen && inv.HasDocument && !inv.DocumentSigned)
        {
            inv.SignDocument();
            GameManager.Instance.ui.SetSigningButtonVisible(false);
            GameManager.Instance.OnDocumentSigned();
        }
        else
        {
            GameManager.Instance.ui.ShowHint("You need the digital pen and the document to sign.");
        }
    }
}