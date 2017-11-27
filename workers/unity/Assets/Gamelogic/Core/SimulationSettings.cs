using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public static class SimulationSettings
    {   
		public static readonly float PlayerSpawnHeight = 10;
		public static readonly float PlayerAcceleration = 15;
		public static readonly float ClientConnectionTimeoutSecs = 7;

        public static readonly Quaternion InitialThirdPersonCameraRotation = Quaternion.Euler(40, 0, 0);
        public static readonly float InitialThirdPersonCameraDistance = 15;

        public static readonly string PlayerPrefabName = "Player";
        public static readonly string PlayerCreatorPrefabName = "PlayerCreator";
        public static readonly string CubePrefabName = "Cube";

        public static readonly float HeartbeatCheckIntervalSecs = 3;
        public static readonly uint TotalHeartbeatsBeforeTimeout = 3;
        public static readonly float HeartbeatSendingIntervalSecs = 3;

        public static readonly int TargetClientFramerate = 60;
        public static readonly int TargetServerFramerate = 60;
        public static readonly int FixedFramerate = 20;

        public static readonly float PlayerCreatorQueryRetrySecs = 4;
        public static readonly float PlayerEntityCreationRetrySecs = 4;

        public static readonly float PlayerIncrement = 0.2f;
        public static readonly int MaxScore = 10;
        public static readonly int ScoreIncrement =5;
        public static readonly string DefaultSnapshotPath = Application.dataPath + "/../../../snapshots/default.snapshot";

        public static readonly string MouseScrollWheel = "Mouse ScrollWheel";
        public static readonly float ThirdPersonZoomSensitivity = 6f;
        public static readonly float ThirdPersonCameraMinDistance = 4f;
        public static readonly float ThirdPersonCameraMaxDistance = 60f;
        public static readonly int RotateCameraMouseButton = 1;
        public static readonly float ThirdPersonCameraSensitivity = 2f;
        public static readonly float ThirdPersonCameraMinPitch = 5f;
        public static readonly float ThirdPersonCameraMaxPitch = 70f;
    }
}
