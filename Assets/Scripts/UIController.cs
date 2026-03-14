using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Action onRoadPlacement, onHousePlacement, onSpecialPlacement;
    public Button placeRoadButton, placeHouseButton, placeSpecialButton;

    public Color outlineColor;
    List<Button> buttonList;

    private void Start()
    {
        buttonList = new List<Button> { placeRoadButton, placeHouseButton, placeSpecialButton };

        //Road Button
        placeRoadButton.onClick.AddListener(() => {
            resetButtonColor();
            modifyOutline(placeRoadButton);
            onRoadPlacement?.Invoke();
        });

        //House Button
        placeHouseButton.onClick.AddListener(() => {
            resetButtonColor();
            modifyOutline(placeHouseButton);
            onHousePlacement?.Invoke();
        });

        //Special Button
        placeSpecialButton.onClick.AddListener(() => {
            resetButtonColor();
            modifyOutline(placeSpecialButton);
            onSpecialPlacement?.Invoke();
        });
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
}
