
using Improbable.Player;

using Improbable.Unity.Visualizer;

using UnityEngine;
using Assets.Gamelogic.UI;
using UnityEngine.UI;



public class MyPlayerHighlighter : MonoBehaviour

{



	[Require] private PlayerInput.Writer PlayerInputWriter;



	[SerializeField] private GameObject playerBody;
	[SerializeField] private Text name;



	private void OnEnable()

	{


}

}