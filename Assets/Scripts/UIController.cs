using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Action onRoadPlacement, onHousePlacement, onCommercialPlacement, onSpecialPlacement;
    public Button placeRoadButton, placeHouseButton, placeCommercialButton, placeSpecialButton;

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
        placeSpecialButton.onClick.AddListener(() => handleButtonClick(placeSpecialButton, onSpecialPlacement));
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
}
