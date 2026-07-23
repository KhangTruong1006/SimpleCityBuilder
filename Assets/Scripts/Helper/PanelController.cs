using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject zonePanel, servicePanel, statsPanel;
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
            closeAllPanel();
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive);
        }
    }

    public void closeAllPanel()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }
}
