using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour

{
    [SerializeField] private GameSettings settings;
    public EconomyManager economyManager;

    public Slider taxSlider, timerSlider, residentialSlider, commercialSlider, industrialSlider;
    [Header("Text")]
    public TextMeshProUGUI taxTMP;

    private void Start()
    {
        taxSlider.onValueChanged.AddListener((value)=> updateTaxOnSlider(value));
    }


    public void updateTaxOnSlider(float value)
    {
        economyManager.updateTax(value/100);
        OnSliderChanged(taxTMP, value, "%");
    }

    public void OnSliderChanged(TextMeshProUGUI text, float value, string textString) {
        text.text = value.ToString() + textString;
    }

    public void updateProgress(Slider slider, float progress)
    {
        slider.value = progress;
    }
}
