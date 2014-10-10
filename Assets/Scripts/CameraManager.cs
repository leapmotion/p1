using UnityEngine;
using System.Collections;
using OVR;

public class CameraManager : MonoBehaviour {
  public GameObject normalCamera;
  public GameObject riftCamera;

	// Use this for initialization
	void Start () {
    Hmd hmd = OVR.Hmd.GetHmd();
    ovrTrackingState ss = hmd.GetTrackingState();
    bool isConnected = (ss.StatusFlags & (uint)ovrStatusBits.ovrStatus_HmdConnected) != 0;
    if (isConnected)
    {
      riftCamera.SetActive(true);
      normalCamera.SetActive(false);
    }
    else
    {
      riftCamera.SetActive(false);
      normalCamera.SetActive(true);
    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
