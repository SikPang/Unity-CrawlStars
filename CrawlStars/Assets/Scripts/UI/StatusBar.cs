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

    public void MoveValue(int to) {
        progressText.text = to.ToString();

        float toPercent = to / (float)maxValue;
        barImage.fillAmount = toPercent;

        barBg.DOFillAmount(toPercent, 0.5f);
    }
}
