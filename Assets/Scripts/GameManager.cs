using SVS;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;
    public UIController uiController;
    public StructureManager structureManager;

    private void Start()
    {
        uiController.onRoadPlacement += RoadPlacementHandler;
        uiController.onHousePlacement  += HousePlacementHandler;
        uiController.onSpecialPlacement += SpecialPlacementHandler;

    }

    private void SpecialPlacementHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placeSpecialStructure;
    }

    private void HousePlacementHandler()
    {
        clearInputActions();
        inputManager.OnMouseClick += structureManager.placeHouse;
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

    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));
    }
}
