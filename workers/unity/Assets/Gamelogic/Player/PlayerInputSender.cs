
using UnityEngine;

using Improbable.Unity;

using Improbable.Unity.Visualizer;

using Improbable.Player;
using Improbable.Worker;

[WorkerType(WorkerPlatform.UnityClient)]

public class PlayerInputSender : MonoBehaviour

{

	[Require] private PlayerInput.Writer PlayerInputWriter;


    void Update ()

	{
        //Debug.LogWarning("A");
        var speedMultiplier = 3;

		var xAxis = Input.GetAxis("Horizontal")* speedMultiplier;

		var yAxis = Input.GetAxis("Vertical")* speedMultiplier;


        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.LogWarning("A");
            PlayerInputWriter.Send(new PlayerInput.Update().AddRespawn(new Respawn()));
            Debug.LogWarning("B");
        }


        var update = new PlayerInput.Update();

		update.SetJoystick(new Joystick(xAxis, yAxis));
        
		PlayerInputWriter.Send(update);

	}

}