using UnityEngine;
using System.Collections;
using Leap;

namespace P1
{
		/// <summary>
		/// Embeds Leap objects in Unity scene with Interaction Box = Field of View
		/// </summary>
		/// <remarks>
		/// Default orientation has controler facing upwards.
		/// LeapGUI should be a a child object of Main Camera.
		/// Objects using Leap coordinates should be child objects
		/// of LeapGUI.
		/// </remarks>
		public class LeapGUI : MonoBehaviour
		{
				Controller controller = new Controller ();
				long lastFrameID = -1; //NOTE: -1 < frame.Id for all IDs

				float viewAngle = 60f * (2 * Mathf.PI / 360); // Field of View of parent camera
				public float leapScale = 5.0f; // Rescaling of Leap coordinates to Unity scene

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
								AlignInteractionBox (frame);
						}
						lastFrameID = frame.Id;

						// Draw Finger Trails
						foreach (Finger finger in frame.Fingers) {
								if (finger.Type () == Finger.FingerType.TYPE_INDEX) {
										GameObject indexSprite = (GameObject)Instantiate (Resources.Load ("IndexFinger"));

										// Position is in scaled and displaced Leap coordinates
										indexSprite.transform.parent = transform;
										indexSprite.transform.localPosition = new Vector3 (finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);
								}
						}
				}

				// Align the Interaction Box with the Main Camera field of view
				void AlignInteractionBox (Frame frame)
				{
						InteractionBox box = frame.InteractionBox;

						// Position of Leap to align scaled InteractionBox in Field of View
						float front = (box.Width / 2) / Mathf.Tan (viewAngle / 2);
						transform.localPosition = new Vector3 (0, - box.Center.y, front + box.Depth / 2) * leapScale;

						// Local Scale includes transformation from Leap Right-Chiral to Unity Left-Chiral coordinates
						transform.localScale = new Vector3 (leapScale, leapScale, -leapScale);
				}

				// Draw a wire-frame Interaction Box
				void DrawInteractionBox (Frame frame)
				{
						if (GlDrawer.instance == null) {
								return;
						}

						InteractionBox box = frame.InteractionBox;
						Vector3 center = new Vector3 (box.Center.x, box.Center.y, box.Center.z);
						Vector3 dimensions = new Vector3 (box.Width, box.Height, box.Depth);

						GlDrawer.instance.DrawWireCube (center, transform.rotation, dimensions, Color.red);
				}
		}
}