//	Created by: Sunny Valley Studio 
//	https://svstudio.itch.io

using System.Collections;
using System.Collections.Generic;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace SVS
{

    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private GameSettings settings;
        public Camera gameCamera;
        private float cameraMovementSpeed;

        private void Start()
        {
            cameraMovementSpeed = settings.camera.speed;
            gameCamera = GetComponent<Camera>();
        }
        public void MoveCamera(Vector3 inputVector)
        {
            var movementVector = Quaternion.Euler(0,30,0) * inputVector;
            gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
        }
    }
}