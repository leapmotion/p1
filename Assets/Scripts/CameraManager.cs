using UnityEngine;
using System.Collections;
using OVR;

public class CameraManager : MonoBehaviour {
  public GameObject normalCamera;
  public GameObject riftCamera;
  public GameObject focusPoint;

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
    if (focusPoint)
    {
      if (Input.GetKeyDown(KeyCode.Space))
      {
        riftCamera.transform.rotation = Quaternion.LookRotation(focusPoint.transform.position - riftCamera.transform.position);
        normalCamera.transform.rotation = Quaternion.LookRotation(focusPoint.transform.position - normalCamera.transform.position);
      }
    }
	}
}
