using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBar : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI hpText;
    public void UpdateBar(int current, int max)
    {
        if (fill != null)
            fill.fillAmount = (float)current / max;
        if (hpText != null)
            hpText.text = $"{current}/{max} HP";
    }
}