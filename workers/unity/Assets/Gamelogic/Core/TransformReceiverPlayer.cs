using Assets.Gamelogic.UI;
using Improbable;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.Core {
    public class TransformReceiverPlayer : MonoBehaviour {
        [Require] private Position.Reader PositionReader;
        [Require] private Rotation.Reader RotationReader;
        [Require] private Scale.Reader ScaleReader;
        [Require] private PlayerData.Reader PlayerDataReader;

        void OnEnable() {

            transform.position = PositionReader.Data.coords.ToUnityVector();
            transform.rotation = RotationReader.Data.rotation.ToUnityQuaternion();
            transform.localScale = new Vector3(ScaleReader.Data.s, ScaleReader.Data.s, ScaleReader.Data.s);
            PositionReader.ComponentUpdated.Add(OnPositionUpdated);
            RotationReader.ComponentUpdated.Add(OnRotationUpdated);
            ScaleReader.ComponentUpdated.Add(OnScaleUpdated);
            transform.Find("Sphere").GetComponent<Renderer>().material.color = SplashScreenController.getColor(PlayerDataReader.Data.color);
            GetComponentInChildren<Text>().text = PlayerDataReader.Data.name;
        }

        void OnDisable() {
            PositionReader.ComponentUpdated.Remove(OnPositionUpdated);
            RotationReader.ComponentUpdated.Remove(OnRotationUpdated);
            ScaleReader.ComponentUpdated.Remove(OnScaleUpdated);
        }

        void OnPositionUpdated(Position.Update update) {
            if (PositionReader.Authority == Authority.NotAuthoritative) {
                if (update.coords.HasValue) {
                    transform.position = update.coords.Value.ToUnityVector();
                    //Debug.LogWarning("TRP: " + transform.position.y);
                }
            }
        }

        void OnRotationUpdated(Rotation.Update update) {
            if (RotationReader.Authority == Authority.NotAuthoritative) {
                if (update.rotation.HasValue) {
                    transform.rotation = update.rotation.Value.ToUnityQuaternion();
                }
            }
        }

        void OnScaleUpdated(Scale.Update update) {
            if (ScaleReader.Authority == Authority.NotAuthoritative) {
                if (update.s.HasValue) {
                    var v = update.s.Value;
                    var old = transform.localScale.x;
                    transform.localScale = new Vector3(v,v,v);
                }
            }
        }
    }
}