using UnityEngine;
using System.Collections;
using BlockPuzzleGameToolkit.Scripts.Map.ScrollableMap;
using UnityEngine.UI;
using System.Threading.Tasks;

[RequireComponent(typeof(RectTransform))]
public class ContentStretchController : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    public void HandleLastLevelPositionUpdate(Vector2 lastLevelPos)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0f);

        Vector2 localPos = rectTransform.InverseTransformPoint(lastLevelPos);

        float requiredHeight = localPos.y + 200f;
        requiredHeight = Mathf.Max(0f, requiredHeight);

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, requiredHeight);
    }
}