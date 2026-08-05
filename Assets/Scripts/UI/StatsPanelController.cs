using TMPro;
using UnityEngine;

public class StatsPanelController : MonoBehaviour
{
    [Header("Population")]
    public TextMeshProUGUI childrenText; 
    public TextMeshProUGUI youngAdultText, adultText, seniorText, totalPopulation;

    [Header("Employment")]
    public TextMeshProUGUI unemployedText;
    public TextMeshProUGUI employedText, employableText, jobsText;

    [Header("Service Generating")]
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI sewageText, powerText;

    [Header("Service Usage")]
    public TextMeshProUGUI waterUsageText;
    public TextMeshProUGUI  sewageUsageText, powerUsageText;

    [Header("Income")]
    public TextMeshProUGUI incomeResidential;
    public TextMeshProUGUI incomeCommercial, incomeIdustrial;

    [Header("Expenses")]
    public TextMeshProUGUI expensesCommercial;
}
