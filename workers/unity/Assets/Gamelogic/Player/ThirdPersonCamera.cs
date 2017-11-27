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

        private void Update()
        {
            SelectNextCameraDistance();
            SelectNextCameraRotation();
        }

        // If the user scrolls up on their mousewheel then zoom in, if they scroll down then zoom out
        private void SelectNextCameraDistance()
        {
            var mouseScroll = Input.GetAxis(SimulationSettings.MouseScrollWheel);
            if (!mouseScroll.Equals(0f))
            {
                var distanceChange = cameraDistance - mouseScroll * SimulationSettings.ThirdPersonZoomSensitivity;
                cameraDistance = Mathf.Clamp(distanceChange, SimulationSettings.ThirdPersonCameraMinDistance,
                    SimulationSettings.ThirdPersonCameraMaxDistance);
            }
        }

        // If the user holds right mouse button and moves their mouse about, the camera rotates around the player
        private void SelectNextCameraRotation()
        {
            if (Input.GetMouseButton(SimulationSettings.RotateCameraMouseButton))
            {
                var yaw = (cameraRotation.eulerAngles.y + Input.GetAxis("Mouse X") * SimulationSettings.ThirdPersonCameraSensitivity) % 360f;
                var pitch = Mathf.Clamp(cameraRotation.eulerAngles.x - Input.GetAxis("Mouse Y") * SimulationSettings.ThirdPersonCameraSensitivity,
                        SimulationSettings.ThirdPersonCameraMinPitch,
                        SimulationSettings.ThirdPersonCameraMaxPitch);
                cameraRotation = UnityEngine.Quaternion.Euler(new Vector3(pitch, yaw, 0));
            }
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
                    if(update.s.Value == 1.0f) {
                        offset = new Vector3(0, -10, 9);
                    } else {
                        offset= (new Vector3(offset.x, offset.y - 0.3F, offset.z + 0.3F));
                    }
                    
                }
            }
        }
    }
}