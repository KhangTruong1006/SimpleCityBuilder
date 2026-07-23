using SVS;
using System;
using System.Resources;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameSettings settings;

    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;
    
    public StructureManager structureManager;
    public PopulationManager populationManager;
    public EconomyManager economyManager;
    public WaterAndPowerService waterAndPowerService;

    public UIController uiController;
    public PanelController panelController;
    public SliderController sliderController;

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
        sliderController.updateTimerBarMaxValue(tickRateInSeconds);
    }

    private void Start()
    {
        uiController.onRoadPlacement += RoadPlacementHandler;
        uiController.onBigStructurePlacement += BigStructureHandler;

        // Zones
        uiController.onResidentialPlacement  += ResidentialPlacementHandler;
        uiController.onCommercialPlacement += CommercialPlacementHandler;
        uiController.onIndustrialPlacement += IndustrialPlacementHandler;

        // Services    
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
        sliderController.fillTimerBar(tickTimer);

        if (tickTimer >= tickRateInSeconds)
        {

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

            uiController.updateHourText(hour);
        }
    }

    private void updateDay()
    {
        if (hour == settings.timers.hoursToDay)
        {
            day += 1;
            hour = 0;
            counter = 0;

            uiController.updateHourText(hour);
            uiController.updateDayText(day);
        }
    }

    // General Hanlder Function
    private void PlacmentHandler(Action<Vector3Int> action)
    {
        clearInputActions();
        inputManager.OnMouseClick += action;
    }

    // ===== Handler Functions
    // Zones
    private void ResidentialPlacementHandler()
    {
        PlacmentHandler(structureManager.placeResidential);
    }

    private void CommercialPlacementHandler()
    {
        PlacmentHandler(structureManager.placeCommercial);
    }

    private void IndustrialPlacementHandler()
    {
        PlacmentHandler(structureManager.placeIndustrial);
    }

    // Services
    private void WaterPlantPlacementHandler()
    {
        PlacmentHandler(structureManager.placeWaterPlant);
    }

    private void PowerPlantPlacementHandler()
    {
        PlacmentHandler(structureManager.placePowerPlant);
    }

    private void SewagePlantPlacementHandler()
    {
        PlacmentHandler(structureManager.placeSewagePlant);
    }

    // Others
    private void BigStructureHandler()
    {
        PlacmentHandler(structureManager.placeBigStructure);
    }

    private void RoadPlacementHandler()
    {
        panelController.closeAllPanel();
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
