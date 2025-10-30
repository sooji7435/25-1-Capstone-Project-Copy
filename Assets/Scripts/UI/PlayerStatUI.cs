using UnityEngine;
using UnityEngine.UI;
public class PlayerStatUI : MonoBehaviour
{

    [SerializeField] private RectTransform cooldownUI;
    [SerializeField] private Vector3 uiOffset = new Vector3(0.6f, 0.8f, 0f);

    [SerializeField] private Slider hpBar;
    [SerializeField] private Image cooldownImage;

    public void UI_HPBarUpdate(int currentHealth, int maxHealth)
    {
        hpBar.value = (float)currentHealth / (float)maxHealth;
    }
  
    public void UI_ParryCooldownUpdate()
    {
        cooldownImage.fillAmount = PlayerScript.Instance.ParryCooldownRatio;
        Vector3 worldPos = PlayerScript.Instance.transform.position + uiOffset;
        cooldownUI.position = Camera.main.WorldToScreenPoint(worldPos);
    }

}
