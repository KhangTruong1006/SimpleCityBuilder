using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public StructurePrefab structurePrefab;

    public Action onRoadPlacement, onResidentialPlacement, onCommercialPlacement, onIndustrialPlacement,  onWaterPlantPlacement, onSewagePlacement, onPowerPlacement, changeSpeed, pauseGame;

    public Button placeRoadButton;

    [Header("Zone Buttons")]
    public Button placeResidentialButton;
    public Button placeCommercialButton, placeIndustrialButton;
    
    [Header("Service Buttons")]
    public Button placeWaterPlantButton;
    public Button placeSewagePlantButton, placePowerPlantButton;

    [Header("Game Controll Button")]
    public Button speedButton;
    public Button pauseBtn;

    [Header("TMP Stats Text")]
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI budgetText, dayText, hourText;

    [Header("Normal Text")]
    public Text speedBtnText;
    public Text gamePauseBtnText;

    public Color outlineColor;
    List<Button> buttonList;

    [Header("Service Info")]
    public TextMeshProUGUI powerInfoText;
    public TextMeshProUGUI waterInfoText, sewageInfoText;

    private void Start()
    {
        buttonList = new List<Button> { placeRoadButton, placeResidentialButton, placeCommercialButton, placeIndustrialButton,  placeWaterPlantButton, placeSewagePlantButton, placePowerPlantButton };

        initializServiceInfo();
        initializeControlButton();
        initializeZoneButtons();
        initializeServiceButtons();

    }

    private void initializeControlButton()
    {
        placeRoadButton.onClick.AddListener(() => handleButtonClick(placeRoadButton, onRoadPlacement));
        speedButton.onClick.AddListener(() => changeGameSpeed(changeSpeed));
        pauseBtn.onClick.AddListener(() => changeGameSpeed(pauseGame));
    }

    private void initializeZoneButtons()
    {
        placeResidentialButton.onClick.AddListener(() => handleButtonClick(placeResidentialButton, onResidentialPlacement));
        placeCommercialButton.onClick.AddListener(() => handleButtonClick(placeCommercialButton, onCommercialPlacement));
        placeIndustrialButton.onClick.AddListener(() => handleButtonClick(placeIndustrialButton, onIndustrialPlacement));
    }

    private void initializeServiceButtons()
    {
        placeWaterPlantButton.onClick.AddListener(() => handleButtonClick(placeWaterPlantButton, onWaterPlantPlacement));
        placeSewagePlantButton.onClick.AddListener(() => handleButtonClick(placeSewagePlantButton, onSewagePlacement));
        placePowerPlantButton.onClick.AddListener(() => handleButtonClick(placePowerPlantButton, onPowerPlacement));
    }

    private void initializServiceInfo()
    {
        displayServiceInfo(waterInfoText, structurePrefab.waterPrefabs, "m³/h");
        displayServiceInfo(sewageInfoText, structurePrefab.sewagePrefab, "m³/h");
        displayServiceInfo(powerInfoText, structurePrefab.powerPrefabs, "kWh");
    }

    public void displayPopulation(int population)
    {
        displayStat(populationText, $"{population}");
    }

    public void displayBudget(float budget)
    {
        displayStat(budgetText, $"{budget:N2}");
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

    private void changeGameSpeed(System.Action action)
    {
        action?.Invoke();
    }

    private void displayServiceInfo(TextMeshProUGUI text, IServicesPrefab prefab, string unit)
    {
        string infoText = $"Cost: ${prefab.Cost:N2} \r\nMaint.: ${prefab.ExpensePerTimeUnit * 20f:N2}/h\r\nGen: {prefab.GeneratingCapacityPerTick:N2} {unit}";
        text.text = infoText;
    }

    public void updateSpeedBtnText(int newSpeed)
    {
        speedBtnText.text = $"{newSpeed}";
    }

    public void updatePauseBtnText(bool isPaused)
    {
        gamePauseBtnText.text = isPaused ? "Resume" : "Pause";
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
