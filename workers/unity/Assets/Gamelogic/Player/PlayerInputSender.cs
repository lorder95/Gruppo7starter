
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
using System.Xml.Serialization;
using System.Timers;
using System.Collections;
using Assets.Gamelogic;

[WorkerType(WorkerPlatform.UnityClient)]

public class PlayerInputSender : MonoBehaviour

{

	[Require] private PlayerInput.Writer PlayerInputWriter;
    [Require] private Status.Reader StatusReader;
    [Require] private Scale.Reader ScaleReader;
    [Require] private Scoreboard.Writer ScoreboardWriter;
    [SerializeField] private GameObject Model;
    [SerializeField] private ParticleSystem expl;

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
        GetComponent<AudioSource>().Play();

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
        Explode();
             
    }

    public void Explode(){
        expl.Play();
        if (expl.isPlaying)
        {
            FindObjectOfType<PlayerExplosion>().explosion=true;
            Debug.LogWarning("Explosion");
        }
        Invoke("Chiusura", expl.time + 1F);
    }

    void Chiusura(){
           
        expl.Stop();
        Debug.LogWarning("Stop  Explosion: Respawn");
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
   

		var xAxis = Input.GetAxis("Horizontal");

		var yAxis = Input.GetAxis("Vertical");

        Vector3 targetDirection = new Vector3(xAxis, 0f, yAxis);
        targetDirection = Camera.main.transform.TransformDirection(targetDirection);

        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.LogWarning("A");
            //SceneManager.LoadSceneAsync(BuildSettings.SplashScreenScene, LoadSceneMode.Additive);
            //Model.GetComponent<Renderer>().enabled = (true);
            //GetComponent<Rigidbody>().isKinematic = false;
            PlayerInputWriter.Send(new PlayerInput.Update().AddRespawn(new Respawn()));
            Debug.LogWarning("B");
        }


        var update = new PlayerInput.Update();

		update.SetJoystick(new Joystick(targetDirection.x, targetDirection.z));
        
		PlayerInputWriter.Send(update);

	}

}