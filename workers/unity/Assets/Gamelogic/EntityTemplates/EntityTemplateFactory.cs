using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine;
using Improbable.Unity.Entity;

namespace Assets.Gamelogic.EntityTemplates
{
    public class EntityTemplateFactory : MonoBehaviour
    {
        public static Entity CreatePlayerCreatorTemplate()
        {
            Debug.LogWarning("Creating playercreator");
            var playerCreatorEntityTemplate = EntityBuilder.Begin()
                .AddPositionComponent(new Improbable.Coordinates(0, SimulationSettings.PlayerSpawnHeight, 0).ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.PlayerCreatorPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new PlayerCreation.Data(), CommonRequirementSets.PhysicsOnly)
            //  .AddComponent(new Status.Data(), CommonRequirementSets.PhysicsOnly)
			//	.AddComponent(new PlayerInput.Data(new Joystick(0, 0)), CommonRequirementSets.SpecificClientOnly(clientId))
                .Build();

            return playerCreatorEntityTemplate;
        }

        public static Entity CreatePlayerTemplate(string clientId, string colore)
        {
            float x = 18.0f;
            float y = 18.0f;
            float xCoord = Random.Range(x, -x);
            float yCoord = Random.Range(y, -y);
            var playerTemplate = EntityBuilder.Begin()
                .AddPositionComponent(new Improbable.Coordinates(xCoord, SimulationSettings.PlayerSpawnHeight, yCoord).ToUnityVector(), CommonRequirementSets.PhysicsOnly)

                .AddMetadataComponent(entityType: SimulationSettings.PlayerPrefabName)
                .SetPersistence(false)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Scale.Data(1), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Status.Data(), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new ClientAuthorityCheck.Data(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout), CommonRequirementSets.PhysicsOnly)
				.AddComponent(new PlayerInput.Data(new Joystick(xAxis: 0, yAxis: 0)), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new PlayerColor.Data(colore),CommonRequirementSets.PhysicsOnly)                                             
                .Build();
            return playerTemplate;
        }
        

        public static Entity CreateRampa1Template() {
            return EntityBuilder.Begin()
                .AddPositionComponent(Vector3.zero, CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent("rampa1")
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                
                .Build();
        }

        public static Entity CreateCubeTemplate()
        {
            float x = 18.0f;
            float y = 18.0f;
            float xCoord = Random.Range(x, -x);
            float yCoord = Random.Range(y, -y);
            var cubeTemplate = EntityBuilder.Begin()
                .AddPositionComponent(new Vector3 (xCoord,0.45f,yCoord), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.CubePrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .Build();

            return cubeTemplate;
        }
    }
}
