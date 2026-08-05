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
    public DemandController demandController;

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

    private float gameSpeed_1;
    private float gameSpeed_2;
    private float gameSpeed_3;
    private int currentSpeedMode = 0;

    private bool isPaused = false;

    private void Awake()
    {
        gameSpeed_1 = settings.masterSettings.speed_1;
        gameSpeed_2 = settings.masterSettings.speed_2;
        gameSpeed_3 = settings.masterSettings.speed_3;

        tickRateInSeconds = gameSpeed_1;
        sliderController.updateTimerBarMaxValue(tickRateInSeconds);
    }

    private void Start()
    {
        uiController.onRoadPlacement += RoadPlacementHandler;
        uiController.changeSpeed += changeSpeedModeOnButtonClicked;
        uiController.pauseGame += togglePause;

        // Zones
        uiController.onResidentialPlacement  += () => PlacmentHandler(structureManager.placeResidential);
        uiController.onCommercialPlacement += () => PlacmentHandler(structureManager.placeCommercial);
        uiController.onIndustrialPlacement += () => PlacmentHandler(structureManager.placeIndustrial);

        // Services    
        uiController.onWaterPlantPlacement += () => PlacmentHandler(structureManager.placeWaterPlant);
        uiController.onSewagePlacement += () => PlacmentHandler(structureManager.placeSewagePlant);
        uiController.onPowerPlacement += () => PlacmentHandler(structureManager.placePowerPlant);
    }

    
    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));

        inputManager.checkKeyInput(KeyCode.Space, togglePause);
        inputManager.checkKeyInput(KeyCode.Alpha1, () => setCurrentSpeedMode(0));
        inputManager.checkKeyInput(KeyCode.Alpha2, () => setCurrentSpeedMode(1));
        inputManager.checkKeyInput(KeyCode.Alpha3, () => setCurrentSpeedMode(2));

        if (isPaused)
        {
            return;
        }

        // Central clock
        // 1 day in game = 24 mins (24 in-games hours)
        // 1 mins (1 in-game hour) = 20 counters ( 1 per 3 seconds (speed 1))

        tickTimer += Time.deltaTime;
        sliderController.fillTimerBar(tickTimer);

        if (tickTimer >= tickRateInSeconds)
        {

            runSimulationTick();

            updateCounter();
            updateHour();
            updateDay();

            
            
            tickTimer = 0.0f;
        }
    }

    private void runSimulationTick()
    {
        //waterAndPowerService.runSimulationTick();
        populationManager.runSimulationTick();
        economyManager.runSimulationTick();
        demandController.runSimulationTick();
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

    private void RoadPlacementHandler()
    {
        panelController.closeAllPanel();
        clearInputActions();

        inputManager.OnMouseClick += roadManager.PlaceRoad;
        inputManager.OnMouseHold += roadManager.PlaceRoad;
        inputManager.OnMouseUp += roadManager.finishPlacingRoad;
    }

    private void changeSpeedModeOnButtonClicked() // For panel button input
    {
        currentSpeedMode += 1;

        if (currentSpeedMode > 2) { 
            currentSpeedMode = 0;
        }

        switchGameSpeed();
    }

    private void setCurrentSpeedMode(int speedMode) // For num key input
    {
        currentSpeedMode = speedMode;
        switchGameSpeed();
    }

    private void switchGameSpeed()
    {
        switch (currentSpeedMode)
        {
            case 0:
                changeTickRateInSecond(gameSpeed_1);
                break;
            case 1:
                changeTickRateInSecond(gameSpeed_2);
                break;
            case 2:
                changeTickRateInSecond(gameSpeed_3);
                break;
        }
    }

    private void togglePause()
    {
        isPaused = !isPaused;
        uiController.updatePauseBtnText(isPaused);
    }

    private void changeTickRateInSecond(float gameSpeed)
    {
        tickRateInSeconds = gameSpeed;
        uiController.updateSpeedBtnText(currentSpeedMode + 1);
        sliderController.updateTimerBarMaxValue(tickRateInSeconds);
    }

    // ===== Other
    private void clearInputActions()
    {
        inputManager.OnMouseClick = null ;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }  
}
