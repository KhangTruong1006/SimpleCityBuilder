using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject zonePanel, servicePanel, statsPanel;
    public GameObject warningPowerPanel, warningSewagePanel, warningWaterPanel;
    public Button zoneBtn, serviceBtn, statsBtn;


    List<GameObject> panels;
    private void Start()
    {
        panels = new List<GameObject>{ zonePanel, servicePanel, statsPanel };

        zoneBtn.onClick.AddListener(() => togglePanel(zonePanel));
        serviceBtn.onClick.AddListener(()=> togglePanel(servicePanel));
        statsBtn.onClick.AddListener(()=> togglePanel(statsPanel));

    }

    public void togglePanel(GameObject panel)
    {
        if(panel != null)
        {
            closeAllPanel(panel);
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive);
        }
    }

    public void closeAllPanel(GameObject currentPanel = null)
    {
        foreach (GameObject panel in panels)
        {
            if(panel != currentPanel)
                panel.SetActive(false);
        }
    }

    public void toggleWarningPanel(GameObject panel)
    {
        if (panel != null)
        {
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive);
        }
    }
}
