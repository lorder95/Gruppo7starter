using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//[WorkerType(WorkerPlatform.UnityWorker)]
public class RespawnPlayer : MonoBehaviour {
    
    [Require] private PlayerInput.Reader PlayerInputReader;

    /*[Require] private Position.Writer PositionWriter;

    [Require] private Rotation.Writer RotationWriter;

    [Require] private Scale.Writer ScaleWriter;
    */


    private void OnEnable() {
        Debug.LogWarning("Enabled");
        PlayerInputReader.RespawnTriggered.Add(Respawning);
    }

    private void OnDisable() {
        PlayerInputReader.RespawnTriggered.Remove(Respawning);
    }


    void Respawning(Respawn respawn) {

        Debug.LogWarning("Respawning");
        float x = 18.0f;
        float y = 18.0f;
        float xCoord = Random.Range(x, -x);
        float yCoord = Random.Range(y, -y);
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.position = new Vector3(0, SimulationSettings.PlayerSpawnHeight, 0);
        rigidbody.rotation = UnityEngine.Quaternion.identity;
        rigidbody.velocity = new Vector3(0, 0, 0);
        

        var pos = rigidbody.position;

        var positionUpdate = new Position.Update()

            .SetCoords(new Coordinates(pos.x, pos.y, pos.z));


        /*PositionWriter.Send(positionUpdate);

        var rotationUpdate = new Rotation.Update()

            .SetRotation(rigidbody.rotation.ToNativeQuaternion());

        RotationWriter.Send(rotationUpdate);


        var scaleUpdate = new Scale.Update()

            .SetS(1.0f);

        ScaleWriter.Send(scaleUpdate);*/
    }
}
