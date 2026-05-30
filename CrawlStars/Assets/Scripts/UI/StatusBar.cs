using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour {
    [SerializeField] private Image barBg;
    [SerializeField] private Image barImage;
    [SerializeField] private TextMeshProUGUI progressText;

    private int maxValue;

    public void Initialize(int maxValue) {
        this.maxValue = maxValue;
        barBg.fillAmount = 1f;
        barImage.fillAmount = 1f;
        progressText.text = maxValue.ToString();
    }

    public void SetValue(int from, int to) {
        progressText.text = to.ToString();
        barBg.fillAmount = from / (float)maxValue;

        float toPercent = to / (float)maxValue;
        barImage.fillAmount = toPercent;

        barBg.DOFillAmount(toPercent, 0.5f);
    }
}
