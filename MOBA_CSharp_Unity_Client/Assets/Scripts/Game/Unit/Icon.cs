using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    [SerializeField] KeyCode key;
    [SerializeField] PlayerController playerController;
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI cooldownText, stackText;
    [SerializeField] Material grayscaleMaterial;

    CombatObj combatObj;

    public void SetCombatObj(CombatObj combatObj)
    {
        this.combatObj = combatObj;

        CombatModel data = CombatTable.Instance.GetCombatData(combatObj.Type);

        //Sprite
        iconImage.sprite = data.Sprite;

        //Timer
        if (combatObj.Timer <= 0)
        {
            cooldownText.text = "";
        }
        else
        {
            cooldownText.text = combatObj.Timer.ToString("F0");
        }

        //Stack
        stackText.text = combatObj.Stack.ToString();

        if (combatObj.IsActive)
        {
            iconImage.material = null;
        }
        else
        {
            iconImage.material = grayscaleMaterial;
        }
    }

    public void IconPressed()
    {
        if(playerController != null)
        {
            playerController.IconPressed(key);
        }
    }
}
