using UnityEngine;
using UnityEngine.UI;

public class RecallUI : MonoBehaviour
{
    [SerializeField] Slider slider;

    float timer;
    public static float RecallTime;

    void Start()
    {

    }

    public void SetActive(bool active)
    {
        if(!gameObject.activeSelf && active)
        {
            timer = RecallTime;
            gameObject.SetActive(true);
        }
        else if(!active)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeSelf)
        {
            timer -= Time.deltaTime;
            slider.value = timer / RecallTime;
        }
    }
}
