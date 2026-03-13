using UnityEngine;
using UnityEngine.UI;
public class ScanningHandsTest : MonoBehaviour
{
    [Header("Path points (required)")]
    public RectTransform pointA;
    public RectTransform pointB;
    public RectTransform imageToMove;       
    public Sprite       spriteToMove;       
    public Vector2      newImageSize = new Vector2(100, 100);

    [Header("Timing")]
    [Tooltip("Seconds it takes to go from A to B (and back again).")]
    public float cycleDuration = 2f;
    RectTransform rt;                       

    void Awake()
    {
        if (imageToMove != null)
        {
            rt = imageToMove;               
        }
        else
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError($"{name}: ScanningHands needs to be under a Canvas to auto-create an image.");
                enabled = false;
                return;
            }

            GameObject go = new GameObject("MovingImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(canvas.transform, false);

            Image img = go.GetComponent<Image>();
            img.sprite = spriteToMove;
            img.SetNativeSize();

            rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = newImageSize;
        }
        if (pointA) rt.anchoredPosition = pointA.anchoredPosition;
    }

    void Update()
    {
        if (!pointA || !pointB || !rt) return;      

        float t = Mathf.PingPong(Time.unscaledTime / cycleDuration, 1f);
        rt.anchoredPosition = Vector2.Lerp(pointA.anchoredPosition, pointB.anchoredPosition, t);
    }
}