using Assets.Gamelogic.Core;
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
		private Button ConnectButton;
		[SerializeField]
		private Dropdown colorDropdown;
		[SerializeField]
		private InputField nameText;
		public static Color color;
		public static string name;

		public void AttemptToConnect()
		{
			// Disable connect button
			ConnectButton.interactable = false;

			// Hide warning if already shown
			NotReadyWarning.SetActive(false);

			AttemptConnection();
		}

		private void AttemptConnection()
		{

			// In case the client connection is successful this coroutine is destroyed as part of unloading
			// the splash screen so ConnectionTimeout won't be called
			FindObjectOfType<Bootstrap>().ConnectToClient();
			StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.ClientConnectionTimeoutSecs, ConnectionTimeout));
			Text colore = colorDropdown.captionText;
			string col = colore.text;
			color = getColor (col);
			name = nameText.text;
		}

		private Color getColor(string col){
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

