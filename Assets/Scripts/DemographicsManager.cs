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



    // Helpers
    public void updateTest()
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

    private int getTotalPopulation()
    {
        return children + youngAdults + adults + seniors;
    }   
    public int getWorkForce()
    {
        return youngAdults + adults;
    }
}
