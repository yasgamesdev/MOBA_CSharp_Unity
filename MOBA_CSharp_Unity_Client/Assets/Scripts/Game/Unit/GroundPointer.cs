using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundPointer : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] RectTransform root;
    float timer;
    Transform target;
    bool move;

    public void Init(Vector2 position)
    {
        move = true;

        image.color = new Color(0, 1, 0, 0.25f);

        transform.position = new Vector3(position.x, 0, position.y);

        timer = 1.0f;
    }

    public void Init(Transform target)
    {
        move = false;

        image.color = new Color(1, 0, 0, 0.25f);
        this.target = target;

        transform.position = new Vector3(target.position.x, 0, target.position.z);

        timer = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!move)
        {
            if(target != null)
            {
                transform.position = new Vector3(target.position.x, 0, target.position.z);
            }
        }

        root.localScale = new Vector3(timer, timer, timer);

        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
