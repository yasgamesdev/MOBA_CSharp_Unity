using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    static float span;

    Vector3 previousPosition, latestPosition;
    float previousAngle, latestAngle;
    bool warped;

    void Update()
    {
        Interpolate(Time.deltaTime);
    }

    public static void SetFrameRate(int frameRate)
    {
        span = 1.0f / frameRate;
    }

    public void Init(Vector3 position, float angle)
    {
        transform.position = previousPosition = latestPosition = new Vector3(position.x, position.y, position.z);
        previousAngle = latestAngle = angle;
        transform.rotation = Quaternion.Euler(0, angle, 0);
        warped = false;
    }

    public void SetPosition(Vector3 latestPosition, float latestAngle, bool warped)
    {
        if (!warped)
        {
            previousPosition = this.latestPosition;
            this.latestPosition = new Vector3(latestPosition.x, latestPosition.y, latestPosition.z);

            previousAngle = this.latestAngle;
            this.latestAngle = latestAngle;

            this.warped = warped;
        }
        else
        {
            transform.position = previousPosition = this.latestPosition = new Vector3(latestPosition.x, latestPosition.y, latestPosition.z);
            previousAngle = this.latestAngle = latestAngle;
            transform.rotation = Quaternion.Euler(0, latestAngle, 0);

            this.warped = warped;
        }
    }

    void Interpolate(float deltaTime)
    {
        if (!warped)
        {
            transform.position = Vector3.Lerp(transform.position, latestPosition, deltaTime / span);
            transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(transform.rotation.eulerAngles.y, latestAngle, deltaTime / span), 0);
        }
        else
        {
            transform.position = latestPosition;
            transform.rotation = Quaternion.Euler(0, latestAngle, 0);
        }
    }
}
