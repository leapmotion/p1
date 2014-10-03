using UnityEngine;
using System.Collections;
using Leap;

namespace P1
{
		public class LeapGUI : MonoBehaviour
		{
				Controller controller = new Controller ();
				long lastFrameID = -1; //DEFAULT: -1 < frame.Id for all IDs

				float viewAngle = 60f; // Field of View of parent camera
				public float leapScale = 2.0f; // Rescaling of Leap coordinates to Unity scene

				public GameObject ulSPhere;
				public GameObject lrSPhere;
				public GameObject interactionBoxCenter;

				// Use this for initialization
				void Start ()
				{
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
			if (lastFrameID < 0) {
				// First-frame initialization
				AlignInteractionBox(frame);
						}
			lastFrameID = frame.Id;

						foreach (Finger finger in frame.Fingers) {
								if (finger.Type () == Finger.FingerType.TYPE_INDEX) {
										GameObject indexSprite = (GameObject)Instantiate (Resources.Load ("IndexFinger"));

										// Position is in scaled and displaced Leap coordinates
										indexSprite.transform.parent = transform;
										indexSprite.transform.localPosition = new Vector3 (finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);

								}
						}
				}

				void AlignInteractionBox (Frame frame)
				{
						InteractionBox box = frame.InteractionBox;

						// Position of Leap to align scaled InteractionBox in Field of View
						float front = (box.Width / 2) / Mathf.Tan (viewAngle / 2);
						transform.localPosition = new Vector3 (0, - box.Center.y, front + box.Depth / 2) * leapScale;

						// Local Scale includes transformation from Leap Right-Chiral to Unity Left-Chiral coordinates
			transform.localScale = new Vector3 (leapScale, leapScale, -leapScale);

			Debug.Log ("AlignInteractionBox");
				}

				void DrawInteractionBox (Frame frame)
				{
						if (GlDrawer.instance == null) {
								return;
						}

			InteractionBox box = frame.InteractionBox;
			Vector3 center = new Vector3 (box.Center.x, box.Center.y, box.Center.z);
			Vector3 dimensions = new Vector3 (box.Width, box.Height, box.Depth);

			interactionBoxCenter.transform.localPosition = center;
			ulSPhere.transform.localPosition = center + dimensions/2;
			lrSPhere.transform.localPosition = center - dimensions/2;
/*
						interactionBoxCenter.transform.localPosition = center;
						GlDrawer.instance.DrawWireCube (interactionBoxCenter.transform.position, interactionBoxCenter.transform.rotation, dimensions, Color.red);
						*/
				}
		}
}