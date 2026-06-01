using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Action onRoadPlacement, onHousePlacement, onCommercialPlacement, onSpecialPlacement;
    public Button placeRoadButton, placeHouseButton, placeCommercialButton, placeSpecialButton;
    public TextMeshProUGUI populationText, budgetText, jobsText, satisfactionText;

    public Color outlineColor;
    List<Button> buttonList;

    private void Start()
    {
        buttonList = new List<Button> { placeRoadButton, placeHouseButton, placeCommercialButton, placeSpecialButton };

        //Road Button
        placeRoadButton.onClick.AddListener(() => handleButtonClick(placeRoadButton, onRoadPlacement));

        //House Button
        placeHouseButton.onClick.AddListener(() => handleButtonClick(placeHouseButton, onHousePlacement));

        // Commercial Button
        placeCommercialButton.onClick.AddListener(() => handleButtonClick(placeCommercialButton, onCommercialPlacement));

        //Special Button
        //placeSpecialButton.onClick.AddListener(() => handleButtonClick(placeSpecialButton, onSpecialPlacement));
    }


    public void displayPopulation(int population)
    {
        displayStat(populationText, $"{population}");
    }

    public void displayBudget(int budget)
    {
        displayStat(budgetText, $"${budget}");
    }

    public void displayJobs(int jobs)
    {
        displayStat(jobsText, $"{jobs}");
    }

    public void displaySatisfaction(float satisfaction)
    {
        displayStat(satisfactionText, $"{satisfaction:F1}%");
    }

    private void modifyOutline(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.enabled = true;
    }

    private void resetButtonColor()
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

    private void displayStat(TextMeshProUGUI textElement, string text)
    {
        textElement.text = text;
    }
}
