using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TexturePainter : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Color paintColor = Color.red;
    public int brushSize = 5;

    public bool isLookingAtHand = false;

    public Texture2D maskTexture;
    private Texture2D drawTexture;
    private RawImage rawImage;

    private Vector2? lastPaintPos = null;
    private bool needsApply = false;

    public float penaltyPoints = 0;

    private float initialPenalty;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        Texture2D mainTex = (rawImage != null) ? (Texture2D)rawImage.texture : (Texture2D)GetComponent<Image>().mainTexture;
        drawTexture = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGBA32, false);
        drawTexture.SetPixels(mainTex.GetPixels());
        drawTexture.Apply();
        if (rawImage != null)
            rawImage.texture = drawTexture;
        else GetComponent<Image>().sprite = Sprite.Create(drawTexture, new Rect(0, 0, drawTexture.width, drawTexture.height), new Vector2(0.5f, 0.5f));
        CalculateInitialPenalty();
    }

    void CalculateInitialPenalty()
    {
        if (maskTexture == null) return;
        Color[] maskPixels = maskTexture.GetPixels();
        int nailPixelsCount = 0;
        foreach (Color c in maskPixels)
        {
            if (c.grayscale > 0.9f) nailPixelsCount++;
        }
        penaltyPoints = nailPixelsCount * 0.001f;
        initialPenalty = penaltyPoints;
        Debug.Log("Стартовый штраф: " + penaltyPoints);
    }

    void Update()
    {
        if (needsApply && drawTexture != null)
        {
            drawTexture.Apply();
            needsApply = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isLookingAtHand) return;
        lastPaintPos = null;
        TryPaint(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLookingAtHand) TryPaint(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        lastPaintPos = null;
        Debug.Log("Текущий штраф: " + penaltyPoints.ToString("F2"));
    }

    void TryPaint(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float x = (localPoint.x - rectTransform.rect.x) / rectTransform.rect.width;
            float y = (localPoint.y - rectTransform.rect.y) / rectTransform.rect.height;
            Vector2 currentPos = new Vector2(x * drawTexture.width, y * drawTexture.height);
            if (lastPaintPos == null)
                PaintAt((int)currentPos.x, (int)currentPos.y);
            else 
                DrawLine(lastPaintPos.Value, currentPos);
            lastPaintPos = currentPos;
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        float step = 1f / Mathf.Max(distance, 1f);
        for (float lerp = 0; lerp <= 1; lerp += step)
        {
            Vector2 point = Vector2.Lerp(start, end, lerp);
            PaintAt((int)point.x, (int)point.y);
        }
    }

    void PaintAt(int centerX, int centerY)
    {
        for (int x = -brushSize; x < brushSize; x++)
        {
            for (int y = -brushSize; y < brushSize; y++)
            {
                if (x * x + y * y <= brushSize * brushSize)
                {
                    int px = centerX + x;
                    int py = centerY + y;

                    if (px >= 0 && px < drawTexture.width && py >= 0 && py < drawTexture.height)
                    {
                        ProcessPixel(px, py);
                    }
                }
            }
        }
    }

    void ProcessPixel(int x, int y)
    {
        Color currentColor = drawTexture.GetPixel(x, y);
        if (currentColor == paintColor)
            return;
        Color maskColor = maskTexture.GetPixel(x, y);
        bool isNail = maskColor.grayscale > 0.9f;

        if (isNail)
        {
            penaltyPoints -= 0.001f;
        }
        if (penaltyPoints < 0) penaltyPoints = 0;
        else
        {
            penaltyPoints += 0.00008f;
        }
        drawTexture.SetPixel(x, y, paintColor);
        needsApply = true;
    }

    public float GetSuccessPercentage()
    {
        if (initialPenalty <= 0) return 0f;
        float ratio = Mathf.Clamp01(1f - (penaltyPoints / initialPenalty));
        return ratio * 100f;
    }

    public void TransferTextureToFinalScreen(RawImage targetRawImage)
    {
        if (targetRawImage != null && drawTexture != null)
        {
            drawTexture.Apply();
            targetRawImage.texture = drawTexture;
        }
    }
}