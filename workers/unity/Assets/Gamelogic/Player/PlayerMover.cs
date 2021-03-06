﻿
using Assets.Gamelogic.Core;

using Assets.Gamelogic.Utils;

using Improbable;

using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Player;

using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using System.Collections;
using UnityEngine;



[WorkerType(WorkerPlatform.UnityWorker)]

public class PlayerMover : MonoBehaviour {



    [Require] private Position.Writer PositionWriter;

    [Require] private Rotation.Writer RotationWriter;

    [Require] private PlayerInput.Reader PlayerInputReader;

    [Require] private Scale.Writer ScaleWriter;

    [Require] private Status.Writer StatusWriter;


    private Rigidbody rigidbody;


    void OnEnable() {

        rigidbody = GetComponent<Rigidbody>();
        Debug.LogWarning("Enabled playermover");
        PlayerInputReader.RespawnTriggered.Add(RespawningResponse);
        StatusWriter.CommandReceiver.OnGameWon.RegisterResponse(ResetGame);

    }

    private void OnDisable() {
        PlayerInputReader.RespawnTriggered.Remove(RespawningResponse);
        StatusWriter.CommandReceiver.OnGameWon.DeregisterResponse();
    }

    Win ResetGame(Winner request, ICommandCallerInfo callerInfo) {
        Debug.LogWarning("This entity won: " + request.winnerName);
        Respawning();
        SpatialOS.Commands.SendCommand(StatusWriter, Scoreboard.Commands.ShowReset.Descriptor, new WinnerName(request.winnerName), gameObject.GetSpatialOsEntity().EntityId);
        return new Win();
    }

    void FixedUpdate() {

        var joystick = PlayerInputReader.Data.joystick;

        var direction = new Vector3(joystick.xAxis, 0, joystick.yAxis);



        if (direction.sqrMagnitude > 1) {

            direction.Normalize();

        }

        rigidbody.AddForce(direction * SimulationSettings.PlayerAcceleration);

        //Debug.LogWarning("PM: " + rigidbody.position.y);

        var pos = rigidbody.position;

        var positionUpdate = new Position.Update()

            .SetCoords(new Coordinates(pos.x, pos.y, pos.z));


        PositionWriter.Send(positionUpdate);

        var rotationUpdate = new Rotation.Update()

            .SetRotation(rigidbody.rotation.ToNativeQuaternion());

        RotationWriter.Send(rotationUpdate);

        if(pos.y < -60) {
            Respawning();
        }


    }

    void Respawning() {
        float x = 18.0f;
        float y = 18.0f;
        float xCoord = Random.Range(-x, x);
        float yCoord = Random.Range(-y, y);
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        transform.position = new Vector3(xCoord, SimulationSettings.PlayerSpawnHeight, yCoord);
        transform.rotation = UnityEngine.Quaternion.identity;
        rigidbody.velocity = new Vector3(0, 0, 0);


        var pos = rigidbody.position;
        Debug.LogWarning("Respawning: " + pos.y);
        var positionUpdate = new Position.Update()

            .SetCoords(new Coordinates(pos.x, pos.y, pos.z));

        PositionWriter.Send(positionUpdate);

        var rotationUpdate = new Rotation.Update()

            .SetRotation(rigidbody.rotation.ToNativeQuaternion());

        RotationWriter.Send(rotationUpdate);


        var scaleUpdate = new Scale.Update()

            .SetS(1.0f);

        ScaleWriter.Send(scaleUpdate);
    }

    void RespawningResponse(Respawn respawn) {
        Respawning();
    }


}
