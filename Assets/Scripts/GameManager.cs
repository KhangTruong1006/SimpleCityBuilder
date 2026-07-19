using SVS;
using System;
using System.Resources;
using Unity.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameSettings settings;

    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;
    public UIController uiController;
    public StructureManager structureManager;
    public PopulationManager populationManager;
    public EconomyManager economyManager;
    public WaterAndPowerService waterAndPowerService;

    [Header("Simulation Settings")]
    public float tickRateInSeconds;
    public int counter;

    public int hour;
    public int day;


    [ReadOnly]
    public float tickTimer = 0.0f;

    private void Awake()
    {
        tickRateInSeconds = settings.masterSettings.speed_3;
    }

    private void Start()
    {
        uiController.onRoadPlacement += RoadPlacementHandler;
        uiController.onResidentialPlacement  += HousePlacementHandler;
        uiController.onCommercialPlacement += CommercialPlacementHandler;
        uiController.onIndustrialPlacement += IndustrialPlacementHandler;

        uiController.onBigStructurePlacement += BigStructureHandler;
        uiController.onWaterPlantPlacement += WaterPlantPlacementHandler;
        uiController.onSewagePlacement += SewagePlantPlacementHandler;
        uiController.onPowerPlacement += PowerPlantPlacementHandler;
    }

    
    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));
        
        
        // Central clock
        // 1 day in game = 24 mins (24 in-games hours)
        // 1 mins (1 in-game hour) = 20 counters ( 1 per 3 seconds (speed 1))
        
        // REMEMBER TO UPDATE COUNTER
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickRateInSeconds)
        {
            //counter += 1;
            //runSimulationTick(counter);
            //runSimulationTick();

            updateCounter();
            updateHour();
            updateDay();
            
            tickTimer = 0.0f;
        }
    }

    private void runSimulationTick()
    {
        waterAndPowerService.runSimulationTick();
        populationManager.runSimulationTick();
        economyManager.runSimulationTick();
    }

    // Timer Functions
    private void updateCounter()
    {
        counter += 1;
    }

    private void updateHour()
    {
        if (counter == settings.timers.countsToHour)
        {
            hour += 1;
            counter = 0;
        }
    }

    private void updateDay()
    {
        if (hour == settings.timers.hoursToDay)
        {
            day += 1;
            hour = 0;
            counter = 0;
        }
    }

    // Handler Functions
    private void HousePlacementHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placeResidential;
    }

    private void BigStructureHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placeBigStructure;
    }

    private void CommercialPlacementHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placeCommercial;
    }

    private void IndustrialPlacementHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placeIndustrial;
    }

    private void WaterPlantPlacementHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placeWaterPlant;
    }

    private void PowerPlantPlacementHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placePowerPlant;
    }

    private void SewagePlantPlacementHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placeSewagePlant;
    }

    private void RoadPlacementHandler()
    {
        clearInputActions();

        inputManager.OnMouseClick += roadManager.PlaceRoad;
        inputManager.OnMouseHold += roadManager.PlaceRoad;
        inputManager.OnMouseUp += roadManager.finishPlacingRoad;
    }

    // Other
    private void clearInputActions()
    {
        inputManager.OnMouseClick = null ;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }
}
