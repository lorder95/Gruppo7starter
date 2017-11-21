
using UnityEngine;

using Improbable.Unity;

using Improbable.Unity.Visualizer;

using Improbable.Player;
using Improbable.Worker;
using UnityEngine.SceneManagement;
using Assets.Gamelogic.Core;
using Improbable.Unity.Core;

[WorkerType(WorkerPlatform.UnityClient)]

public class PlayerInputSender : MonoBehaviour

{

	[Require] private PlayerInput.Writer PlayerInputWriter;
    [Require] private Status.Reader StatusReader;
    [SerializeField] private GameObject Model;

    void OnEnable() {


        StatusReader.PlayerDeadTriggered.Add(BackToSplash);

    }

    private void OnDisable() {
        StatusReader.PlayerDeadTriggered.Remove(BackToSplash);
    }

    void BackToSplash(Dead dead) {
        Debug.LogWarning("Called");
        SpatialOS.Disconnect();

        SceneManager.LoadScene(BuildSettings.SplashScreenScene, LoadSceneMode.Additive);
    }
    void Update ()

	{
        //Debug.LogWarning("A");
        var speedMultiplier = 3;

		var xAxis = Input.GetAxis("Horizontal")* speedMultiplier;

		var yAxis = Input.GetAxis("Vertical")* speedMultiplier;


        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.LogWarning("A");
            //SceneManager.LoadSceneAsync(BuildSettings.SplashScreenScene, LoadSceneMode.Additive);
            //Model.GetComponent<Renderer>().enabled = (true);
            //GetComponent<Rigidbody>().isKinematic = false;
            PlayerInputWriter.Send(new PlayerInput.Update().AddRespawn(new Respawn()));
            Debug.LogWarning("B");
        }


        var update = new PlayerInput.Update();

		update.SetJoystick(new Joystick(xAxis, yAxis));
        
		PlayerInputWriter.Send(update);

	}

}