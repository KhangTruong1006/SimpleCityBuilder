using UnityEngine;

public class DemandManager : MonoBehaviour
{
    public float residentialDemand = 1.0f;
    public float commercialDemand = 0.0f;
    public float industrialDemand = 0.0f;

    // Update is called once per frame
    //void Update()
    //{

    //}


    private float calculateHousingVacancy(int population, int capcity)
    {
        float vancancyRate = 1.0f - ((float)population / capcity);
        return vancancyRate;
    }

    private float calculateJobVacancy(int employedPopulation, int jobCapacity)
    {
        float vancancyRate = 1.0f - ((float)employedPopulation / jobCapacity);
        return vancancyRate;
    }

    private void calculateResidentialDemand(int population, int capcity, int employedPopulation, int jobCapacity)
    {
        float housingVacancy = calculateHousingVacancy(population, capcity);
        float jobVacancy = calculateJobVacancy(employedPopulation, jobCapacity);
        
    }

}
