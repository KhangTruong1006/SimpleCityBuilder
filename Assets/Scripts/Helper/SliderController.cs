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
        taxSlider.onValueChanged.AddListener((value) => updateTaxOnSlider(value));
    }


    public void updateTaxOnSlider(float value)
    {
        economyManager.updateTax(value / 100);
        updateTextOnSliderChanged(taxTMP, value, "%");
    }

    public void fillTimerBar(float tickTimer)
    {
        fillSliderBasedOnValue(timerSlider, tickTimer);
    }

    // Udpate sliders
    public void updateTimerBarMaxValue(float value)
    {
        timerSlider.maxValue = value;
    }

    public void updateZoneDemandBars(float residential, float commercial, float industrial)
    {
        fillSliderBasedOnValue(residentialSlider, residential);
        fillSliderBasedOnValue(commercialSlider, commercial);
        fillSliderBasedOnValue(industrialSlider, industrial);
    }


    // Helpers
    private void updateTextOnSliderChanged(TextMeshProUGUI text, float value, string textString)
    {
        text.text = value.ToString() + textString;
    }

    public void fillSliderBasedOnValue(Slider slider, float value)
    {
        slider.value = value;
    }
}
