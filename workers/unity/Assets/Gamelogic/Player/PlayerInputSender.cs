
using UnityEngine;

using Improbable.Unity;

using Improbable.Unity.Visualizer;

using Improbable.Player;
using Improbable.Worker;
using UnityEngine.SceneManagement;
using Assets.Gamelogic.Core;
using Improbable.Unity.Core;
using Improbable.Core;
using UnityEngine.UI;
using Improbable.Entity.Component;
using Improbable.Collections;
using System;

[WorkerType(WorkerPlatform.UnityClient)]

public class PlayerInputSender : MonoBehaviour

{

	[Require] private PlayerInput.Writer PlayerInputWriter;
    [Require] private Status.Reader StatusReader;
    [Require] private Scale.Reader ScaleReader;
    [Require] private Scoreboard.Writer ScoreboardWriter;
    [SerializeField] private GameObject Model;

    private GameObject scoreCanvasUI;
    private Text totalPointsGUI;

    private void Awake() {
        scoreCanvasUI = GameObject.Find("ScoreCanvas");
        if (scoreCanvasUI) {
            totalPointsGUI = scoreCanvasUI.GetComponentInChildren<Text>();
            scoreCanvasUI.SetActive(true);
            updateGUI(1);
        }
    }

    void OnEnable() {


        StatusReader.PlayerDeadTriggered.Add(BackToSplash);
        ScoreboardWriter.CommandReceiver.OnSendScoreboard.RegisterResponse(OnUpdateScoreboard);
        ScaleReader.ComponentUpdated.Add(OnNumberOfPointsUpdated);

    }

    private void OnDisable() {
        StatusReader.PlayerDeadTriggered.Remove(BackToSplash);
        ScoreboardWriter.CommandReceiver.OnSendScoreboard.DeregisterResponse();
        ScaleReader.ComponentUpdated.Remove(OnNumberOfPointsUpdated);
    }
    private ScoreResponse OnUpdateScoreboard(ScoreRequest request, ICommandCallerInfo callerInfo) {
        List<ScoreEntry> points = request.points;

        for (int i = 0; i < 5; i++) {
            if(i < points.Count) {
                scoreCanvasUI.transform.Find("Position" + i).GetComponentInChildren<Text>().text = points[i].name + ": " + points[i].value;
            } else {
                scoreCanvasUI.transform.Find("Position" + i).GetComponentInChildren<Text>().text = "";
            }
            
        }
        return new ScoreResponse();

    }
    void BackToSplash(Dead dead) {
        Debug.LogWarning("Called backtosplash");
        PlayerInputWriter.Send(new PlayerInput.Update().AddRespawn(new Respawn()));     
    }


    private void OnNumberOfPointsUpdated(Scale.Update update) {
        float v = update.s.Value * SimulationSettings.ScoreIncrement-(SimulationSettings.ScoreIncrement-1);
        int numberOfPoints = (int)v;
        updateGUI(numberOfPoints);
    }
    void updateGUI(int score) {
        if (scoreCanvasUI) {
            if (score > 0) {
                scoreCanvasUI.SetActive(true);
                totalPointsGUI.text = score.ToString() + "/" + (SimulationSettings.MaxScore*SimulationSettings.ScoreIncrement-(SimulationSettings.ScoreIncrement-1)).ToString();
            } else {
                scoreCanvasUI.SetActive(false);
            }
        }
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