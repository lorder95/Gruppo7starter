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

