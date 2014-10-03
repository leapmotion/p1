using UnityEngine;
using System.Collections;
using Leap;

namespace P1
{
		public class LeapGUI : MonoBehaviour
		{
		public float scaleLeap;

		public GameObject ulSPhere;
		public GameObject lrSPhere;

				public GameObject interactionBoxCenter;
				Controller controller = new Controller ();
				int lastFrameID = -1; //DEFAULT: No frames seen

				// Use this for initialization
				void Start ()
				{
					scaleLeap = transform.localScale.x;
				}
	
				// Update is called once per frame
				void Update ()
				{
						Frame frame = controller.Frame ();
						if (frame.IsValid == false ||
								frame.Id == lastFrameID) {
								return;
						}

						UpdateFrame (frame);
						DrawInteractionBox (frame);
				}

				void UpdateFrame (Frame frame)
				{
						foreach (Finger finger in frame.Fingers) {
								if (finger.Type () == Finger.FingerType.TYPE_INDEX) {
										GameObject indexSprite = (GameObject)Instantiate (Resources.Load ("IndexFinger"));
										indexSprite.transform.parent = transform;
										indexSprite.transform.localPosition = new Vector3 (finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);
								}
						}
				}

				void DrawInteractionBox (Frame frame)
				{
						if (GlDrawer.instance == null) {
								return;
						}

						InteractionBox box = frame.InteractionBox;
						Vector3 center = new Vector3 (box.Center.x, 0, box.Center.z);
						Vector3 dimensions = new Vector3 (box.Width, box.Height, box.Depth) * scaleLeap;

			ulSPhere.transform.localPosition = dimensions;
			lrSPhere.transform.localPosition = dimensions * (-1);

						interactionBoxCenter.transform.localPosition = center;
						GlDrawer.instance.DrawWireCube (interactionBoxCenter.transform.position, interactionBoxCenter.transform.rotation, dimensions, Color.red);
				}
		}
}