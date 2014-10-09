using UnityEngine;
using System.Collections;

namespace P1
{
		public class TwitterListStatus : MonoBehaviour
		{
		
				TwitterList list_;
				TwitterStatus status_;
				int index_;
				float HEIGHT = 2;
			public	TextMesh text;

				public TwitterStatus status {
						get { return status_; }
						set {
								status_ = value;
								DisplayStatus ();
						}
				}

				public TwitterList list {
						get { return list_; }
						set {
								list_ = value;
						}
				}

				public int index {
						get { return index_; }
						set {
								index_ = value;
								MoveMe ();
						}
				}

				// Use this for initialization
				void Start ()
				{
						if (text == null)
								text = GetComponentInChildren<TextMesh> ();
				}
	
				// Update is called once per frame
				void Update ()
				{

				}

				void DisplayStatus ()
				{
						text.text = status.text;
				}

				void MoveMe ()
				{
						transform.localPosition = new Vector3 (0, -index * HEIGHT, 0);
				}
		}
}