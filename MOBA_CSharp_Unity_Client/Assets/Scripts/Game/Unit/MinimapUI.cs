using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] RTSCamera rtsCamera;

    public static float MapWidth, MapHeight;
    public const float MinimapWidth = 250.0f;
    public const float MinimapHeight = 250.0f;

    public void PointerDown(BaseEventData data)
    {
        rtsCamera.SetFocus(GetWorldPosition(((PointerEventData)data).position));
    }

    public void OnDrag(BaseEventData data)
    {
        if (((PointerEventData)data).dragging)
        {
            rtsCamera.SetFocus(GetWorldPosition(((PointerEventData)data).position));
        }
    }

    Vector2 GetWorldPosition(Vector2 screenPosition)
    {
        Vector2 localPosition = transform.InverseTransformPoint(screenPosition);
        
        float x = (localPosition.x + MinimapWidth * 0.5f) / (MinimapWidth * 0.5f) * MapWidth * 0.5f;
        float y = (localPosition.y - MinimapHeight * 0.5f) / (MinimapHeight * 0.5f) * MapHeight * 0.5f;

        float xx = (localPosition.x) / (MinimapWidth * 0.5f) * MapWidth * 0.5f;
        float yx = (localPosition.y) / (MinimapHeight * 0.5f) * MapHeight * 0.5f;
        Debug.Log(screenPosition + ", " + localPosition + ", " + new Vector2(xx, yx));
        return new Vector2(xx, yx);
    }
}
