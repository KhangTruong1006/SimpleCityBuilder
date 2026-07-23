using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Action onRoadPlacement, onResidentialPlacement, onCommercialPlacement, onIndustrialPlacement, onBigStructurePlacement, onWaterPlantPlacement, onSewagePlacement, onPowerPlacement;
    public Button placeRoadButton, placeResidentialButton, placeCommercialButton, placeIndustrialButton, placeBigStructureButton, placeWaterPlantButton, placeSewagePlantButton, placePowerPlantButton;
    public TextMeshProUGUI populationText, budgetText, dayText, hourText;

    public Color outlineColor;
    List<Button> buttonList;

    private void Start()
    {
        buttonList = new List<Button> { placeRoadButton, placeResidentialButton, placeCommercialButton, placeIndustrialButton, placeBigStructureButton, placeWaterPlantButton, placeSewagePlantButton, placePowerPlantButton };

        placeRoadButton.onClick.AddListener(() => handleButtonClick(placeRoadButton, onRoadPlacement));

        // Zones
        placeResidentialButton.onClick.AddListener(() => handleButtonClick(placeResidentialButton, onResidentialPlacement));
        placeCommercialButton.onClick.AddListener(() => handleButtonClick(placeCommercialButton, onCommercialPlacement));
        placeIndustrialButton.onClick.AddListener(() => handleButtonClick(placeIndustrialButton, onIndustrialPlacement));

        //Big Structure Button (Remove Later)
        placeBigStructureButton.onClick.AddListener(() => handleButtonClick(placeBigStructureButton, onBigStructurePlacement));

        // Service
        placeWaterPlantButton.onClick.AddListener(() => handleButtonClick(placeWaterPlantButton, onWaterPlantPlacement));
        placeSewagePlantButton.onClick.AddListener(() => handleButtonClick(placeSewagePlantButton, onSewagePlacement));
        placePowerPlantButton.onClick.AddListener(() => handleButtonClick(placePowerPlantButton, onPowerPlacement));
    }


    public void displayPopulation(int population)
    {
        displayStat(populationText, $"{population}");
    }

    public void displayBudget(float budget)
    {
        displayStat(budgetText, $"{budget}");
    }

    private void modifyOutline(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }

    public void resetButtonColor()
    {
        foreach(var button in buttonList)
        {
            var outline = button.GetComponent<Outline>();
            outline.enabled = false;
        }
    }

    private void handleButtonClick(Button button, System.Action action)
    {
        resetButtonColor();
        modifyOutline(button);
        action?.Invoke();
    }

    // UPDATE CHANGING GAME SPEED FUNCTION
    private void changeGameSpeed()
    {
        
    }

    public void updateHourText(int hour)
    {
        displayStat(hourText, $"{hour} : 00");
    }

    public void updateDayText(int day)
    {
        displayStat(dayText, $"Day:{day}");
    }

    private void displayStat(TextMeshProUGUI textElement, string text)
    {
        textElement.text = text;
    }
}
