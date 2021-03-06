﻿using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Unity.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
	public class SplashScreenController : MonoBehaviour
	{
		[SerializeField]
		private GameObject NotReadyWarning;
        [SerializeField]
        private GameObject NameWarning;
        [SerializeField]
		private Button ConnectButton;
		[SerializeField]
		private Dropdown colorDropdown;
		[SerializeField]
		private InputField nameText;
		private Color color;
        private string colore;
		private string name;

        public string GetName() {
            return name;
        }
        public string GetColor() {
            return colore;
        }
		public void AttemptToConnect()
		{
            Debug.LogWarning("Attempt to connect");
			AttemptConnection();
		}

		private void AttemptConnection()
		{


            Debug.LogWarning("Attempt connection");
            // In case the client connection is successful this coroutine is destroyed as part of unloading
            // the splash screen so ConnectionTimeout won't be called
            Text col = colorDropdown.captionText;
            colore = col.text;
			name = nameText.text;
            if(name.Length>=1 && name.Length < 15) {

                // Disable connect button
                ConnectButton.interactable = false;

                // Hide warning if already shown
                NotReadyWarning.SetActive(false);
                NameWarning.SetActive(false);

                NameWarning.SetActive(false);
                Debug.LogWarning("Dati inseriti: " + colore + " - " + name);
                FindObjectOfType<Bootstrap>().ConnectToClient();
			    StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.ClientConnectionTimeoutSecs, ConnectionTimeout));
            } else {
                NameWarning.SetActive(true);


            }
            
			
			
		}

		public static Color getColor(string col){
			Color c;
			switch(col){
			case "Black":
				c = new Color (0, 0, 0, 1);
				break;
			case "Blue":
				c = new Color (0, 0, 1, 1);
				break;
			case "Cyan":
				c = new Color (0, 1, 1, 1);
				break;
			case "Gray":
				c = new Color (0.5F, 0.5F, 0.5F, 1);
				break;
			case "Green":
				c = new Color (0, 1, 0, 1);
				break;
			case "Grey":
				c = new Color (0.5F, 0.5F, 0.5F, 1);
				break;
			case "Magenta":
				c = new Color (255, 0, 255, 1);
				break;
			case "Red":
				c = new Color (1, 0, 0, 1);
				break;
			case "White":
				c = new Color (1, 1, 1, 1);
				break;
			default:
				c = new Color (1, 0.92F, 0.016F, 1);
				break;
			}
			return c;

		}

		private void ConnectionTimeout()
		{
			if (SpatialOS.IsConnected)
			{
				SpatialOS.Disconnect();
			}
				
			NotReadyWarning.SetActive(true);
			ConnectButton.interactable = true;
		}
	}
}

