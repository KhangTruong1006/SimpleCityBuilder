using UnityEngine;

public class DemographicsManager : MonoBehaviour
{
    // Children (0-18) - Young Adults (19 - 35) - Adutls (36 - 60) - Seniors (60+)
    public int children;
    public int youngAdults;
    public int adults;
    public int seniors;

    private float preciseChildren;
    private float preciseYoungAdults;
    private float preciseAdults;
    private float preciseSeniors;

    public float birthRate = 0.02f; 
    public float agingRate = 0.05f; 
    public float deathRate = 0.05f; 


    public void initializeDemographics(int startingPopulation)
    {
        preciseYoungAdults = startingPopulation * 0.4f;
        preciseAdults = startingPopulation * 0.4f;
        preciseChildren = startingPopulation * 0.15f;
        preciseSeniors = startingPopulation * 0.05f;

        updatePopulationValues();
    }


    public int handleDemographics(float globalFactors, int populationCap)
    {
        int currentPopulation = getTotalPopulation();

        float births = (preciseYoungAdults + preciseAdults) * birthRate;
        float deaths = preciseSeniors * deathRate;

        calculatePreciseAgeChanges(births, deaths);


        // Immegration and Emigration
        if (globalFactors > 0f && currentPopulation < populationCap)
        {
            preciseYoungAdults += globalFactors * 0.6f;
            preciseAdults += globalFactors * 0.4f;
        }

        // Leaving when the city is in poor conditions
        else if (globalFactors < 0f)
        {
            float negativeFactor = globalFactors / Mathf.Max(1, currentPopulation);
            
            float childrenChange = preciseChildren * negativeFactor;
            float youngAdultsChange = preciseYoungAdults * negativeFactor;
            float adultsChange = preciseAdults * negativeFactor;
            float seniorsChange = preciseSeniors * negativeFactor;

            updatePreciseAgeChanges(childrenChange, youngAdultsChange, adultsChange, seniorsChange);
        }

        // Prevent negative population values
        preciseChildren = Mathf.Max(0f, preciseChildren);
        preciseYoungAdults = Mathf.Max(0f, preciseYoungAdults);
        preciseAdults = Mathf.Max(0f, preciseAdults);
        preciseSeniors = Mathf.Max(0f, preciseSeniors);

        updatePopulationValues();

        return getTotalPopulation();
    }


    private void calculatePreciseAgeChanges(float births, float deaths)
    {
        float childrenToYoungAdults = calculateMovingAgeGroup(preciseChildren);
        float youngAdultsToAdults = calculateMovingAgeGroup(preciseYoungAdults);
        float adultsToSeniors = calculateMovingAgeGroup(preciseAdults);

        float childrenChange = births - childrenToYoungAdults;
        float youngAdultsChange = childrenToYoungAdults - youngAdultsToAdults;
        float adultsChange = youngAdultsToAdults - adultsToSeniors;
        float seniorsChange = adultsToSeniors - deaths;

        updatePreciseAgeChanges(childrenChange, youngAdultsChange, adultsChange, seniorsChange);
    }


    private void updatePreciseAgeChanges(float childrenChange, float youngAdultsChange, float adultsChange, float seniorsChange)
    {
        preciseChildren += childrenChange;
        preciseYoungAdults += youngAdultsChange;
        preciseAdults += adultsChange;
        preciseSeniors += seniorsChange;
    }

    
    
    // Helpers
    public void updatePopulationValues()
    {
        children = convertFloatToInt(preciseChildren);
        youngAdults = convertFloatToInt(preciseYoungAdults);
        adults = convertFloatToInt(preciseAdults);
        seniors = convertFloatToInt(preciseSeniors);
    }

    private int convertFloatToInt(float value)
    {
        return Mathf.FloorToInt(value);
    }

    private float calculateMovingAgeGroup(float preciseNum)
    {
        return preciseNum * agingRate;
    }

    private int getTotalPopulation()
    {
        return children + youngAdults + adults + seniors;
    }   
    public int getWorkForce()
    {
        return youngAdults + adults;
    }
}
