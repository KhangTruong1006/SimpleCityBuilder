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
    public TextMeshProUGUI expenseIndustrial, expensePower, expenseWater,expenseSewage;

    [Header("Summary")]
    public TextMeshProUGUI totalIncomeText;
    public TextMeshProUGUI totalExpensesText, netIncomeText;

    //General Function
    private void displayStat(TextMeshProUGUI textElement, string text)
    {
        textElement.text = text;
    } 

    //
    public void displayPopulationStats(int children, int youngAdult, int adult, int senior, int population)
    {
        displayStat(childrenText, $"{children}");
        displayStat(youngAdultText, $"{youngAdult}");
        displayStat(adultText, $"{adult}");
        displayStat(seniorText, $"{senior}");
        displayStat(totalPopulation, $"{population}");
    }

   public void displayEmploymentStats(float unemployedRate, int employed, int employable, int jobs)
    {
        displayStat(unemployedText, $"{unemployedRate:N2}%");
        displayStat(employedText, $"{employed}");
        displayStat(employableText, $"{employable}");
        displayStat(jobsText, $"{jobs}");
    }

    public void displayServiceGeneratingStats(float water, float sewage, float power)
    {
        displayStat(waterText, $"{water:N2}");
        displayStat(sewageText, $"{sewage:N2}");
        displayStat(powerText, $"{power:N2}");
    }

    public void displayServiceUsageStats(float waterUsage, float sewageUsage, float powerUsage)
    {
        displayStat(waterUsageText, $"{waterUsage:N2}");
        displayStat(sewageUsageText, $"{sewageUsage:N2}");
        displayStat(powerUsageText, $"{powerUsage:N2}");
    }

   public void displayIncomeStats(float incomeResidential, float incomeCommercial, float incomeIndustrial)
    {
        displayStat(this.incomeResidential, $"{incomeResidential:N2}");
        displayStat(this.incomeCommercial, $"{incomeCommercial:N2}");
        displayStat(this.incomeIdustrial, $"{incomeIndustrial:N2}");
    }

    public void displayExpenseStats(float expenseCommercial, float expenseIndustrial, float expensePower, float expenseWater, float expenseSewage)
    {
        displayStat(this.expensesCommercial, $"{expenseCommercial:N2}");
        displayStat(this.expenseIndustrial, $"{expenseIndustrial:N2}");
        displayStat(this.expensePower, $"{expensePower:N2}");
        displayStat(this.expenseWater, $"{expenseWater:N2}");
        displayStat(this.expenseSewage, $"{expenseSewage:N2}");
    }

     public void displaySummaryStats(float totalIncome, float totalExpenses, float netIncome)
    {
        displayStat(totalIncomeText, $"{totalIncome:N2}");
        displayStat(totalExpensesText, $"{totalExpenses:N2}");
        displayStat(netIncomeText, $"{netIncome:N2}");
    }
}
