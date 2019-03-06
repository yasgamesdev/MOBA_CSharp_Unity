using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    [SerializeField] float minCameraDistance, maxCameraDistance, screenEdgeBorderThickness, panSpeed;
    public static bool IsScreenEdgeMovement;
    Transform playerTransform;
    Vector3 focusPosition;
    bool autoFollow;
    Vector3 cameraDirection;
    float cameraDistance;
    Quaternion rotation;
    // Use this for initialization
    void Start()
    {
        focusPosition = Vector3.zero;
        autoFollow = true;
        cameraDirection = new Vector3(0, 1f, -1f);
        cameraDistance = (minCameraDistance + maxCameraDistance) * 0.5f;
        rotation = Quaternion.Euler(45f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            if (autoFollow)
            {
                focusPosition = playerTransform.position;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                autoFollow = true;
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow) || (IsScreenEdgeMovement && Input.mousePosition.y >= Screen.height - screenEdgeBorderThickness))
                {
                    focusPosition += Vector3.forward * panSpeed * Time.deltaTime;
                    autoFollow = false;
                }
                if (Input.GetKey(KeyCode.DownArrow) || (IsScreenEdgeMovement && Input.mousePosition.y <= screenEdgeBorderThickness))
                {
                    focusPosition -= Vector3.forward * panSpeed * Time.deltaTime;
                    autoFollow = false;
                }
                if (Input.GetKey(KeyCode.LeftArrow) || (IsScreenEdgeMovement && Input.mousePosition.x <= screenEdgeBorderThickness))
                {
                    focusPosition += Vector3.left * panSpeed * Time.deltaTime;
                    autoFollow = false;
                }
                if (Input.GetKey(KeyCode.RightArrow) || (IsScreenEdgeMovement && Input.mousePosition.x >= Screen.width - screenEdgeBorderThickness))
                {
                    focusPosition += Vector3.right * panSpeed * Time.deltaTime;
                    autoFollow = false;
                }
            }

            float x = Mathf.Clamp(focusPosition.x, -MinimapUI.MapWidth * 0.5f, MinimapUI.MapWidth * 0.5f);
            float z = Mathf.Clamp(focusPosition.z, -MinimapUI.MapHeight * 0.5f, MinimapUI.MapHeight * 0.5f);
            focusPosition = new Vector3(x, 0, z);

            transform.position = Vector3.Lerp(transform.position, focusPosition + cameraDirection * cameraDistance, 0.2f);
            transform.rotation = rotation;

            cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * 20.0f;
            cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
        }
    }

    public void SetTarget(Transform playerTransform)
    {
        if(this.playerTransform == null && playerTransform != null)
        {
            focusPosition = playerTransform.position;
            autoFollow = true;
        }
        else if (this.playerTransform != null && playerTransform == null)
        {
            autoFollow = false;
        }

        this.playerTransform = playerTransform;
    }

    public void SetFocus(Vector2 position)
    {
        focusPosition = new Vector3(position.x, 0, position.y);
        autoFollow = false;
    }
}
