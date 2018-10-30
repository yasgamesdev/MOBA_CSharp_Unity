using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour {
    Vector3 previousPosition, latestPosition;
    float previousAngle, latestAngle;
    bool warped;
    // Use this for initialization
    static float span;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Interpolate(Time.deltaTime);
    }

    public static void SetFrameRate(int frameRate)
    {
        span = 1.0f / frameRate;
    }

    public void Init(Vector2 position, float angle)
    {
        transform.position = previousPosition = latestPosition = new Vector3(position.x, 0, position.y);
        previousAngle = latestAngle = angle;
        transform.rotation = Quaternion.Euler(0, angle, 0);
        warped = false;
    }

    public void SetPosition(Vector2 latestPosition, float latestAngle, bool warped)
    {
        previousPosition = this.latestPosition;
        this.latestPosition = new Vector3(latestPosition.x, 0, latestPosition.y);

        previousAngle = this.latestAngle;
        this.latestAngle = latestAngle;

        this.warped = warped;
    }

    void Interpolate(float deltaTime)
    {
        if(!warped)
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
