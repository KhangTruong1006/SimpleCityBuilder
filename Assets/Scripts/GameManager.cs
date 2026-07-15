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

    [Header("Simulation Settings")]
    public float tickRateInSeconds;
    [ReadOnly]
    public float tickTimer = 0.0f;

    private void Awake()
    {
        tickRateInSeconds = settings.masterSettings.tickRateInSeconds;
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
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickRateInSeconds)
        {
            runSimulationTick();
            tickTimer = 0.0f;
        }
    }

    private void runSimulationTick()
    {
        populationManager.runSimulationTick();
        economyManager.runSimulationTick();
    }

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

    private void clearInputActions()
    {
        inputManager.OnMouseClick = null ;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }
}
