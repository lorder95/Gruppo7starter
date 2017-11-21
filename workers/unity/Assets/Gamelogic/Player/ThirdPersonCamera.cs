using Assets.Gamelogic.Core;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable;
using Improbable.Worker;

namespace Assets.Gamelogic.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Require]
        private ClientAuthorityCheck.Writer ClientAuthorityCheckWriter;
        [Require] private Scale.Reader ScaleReader;
        private Transform camera;
        private UnityEngine.Quaternion cameraRotation;
        private float cameraDistance;
        private Vector3 offset = new Vector3(0, -10, 9);

        private void OnEnable()
        {
            
            // Grab the camera from the Unity scene
            camera = Camera.main.transform;
            // Set the camera as a child of the Player to easily ensure the camera follows the Player
            
            // Set the camera rotation and zoom distance to some initial values
            cameraRotation = SimulationSettings.InitialThirdPersonCameraRotation;
            cameraDistance = SimulationSettings.InitialThirdPersonCameraDistance;
            ScaleReader.ComponentUpdated.Add(OnScaleUpdated);
        }

        void OnDisable()
        {
            
            ScaleReader.ComponentUpdated.Remove(OnScaleUpdated);
        }

        private void LateUpdate()
        {
            SetCameraTransform();
        }

        // Update the position and orientation of the camera to match the cameraRotation and cameraDistance
        private void SetCameraTransform()
        {
            // Set the position of the camera based on the desired rotation towards and distance from the Player model
            camera.localPosition = cameraRotation * Vector3.back * cameraDistance;
            // Set the camera to look towards the Player model
            camera.position=transform.position-offset;
        }

        void OnScaleUpdated(Scale.Update update)
        {
            if (ScaleReader.Authority == Authority.NotAuthoritative)
            {
                if (update.s.HasValue)
                {
                    offset= (new Vector3(offset.x, offset.y - 0.5F, offset.z + 0.5F));
                }
            }
        }
    }
}