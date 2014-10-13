using UnityEngine;
using System.Collections;
using OVR;
using SimpleJSON;

namespace P1
{
  public class CameraManager : MonoBehaviour {
    public GameObject normalCamera;
    public GameObject riftCamera;
    public GameObject focusPoint;
    public GameObject handController;

	  // Use this for initialization
	  void Start () {
      Hmd hmd = OVR.Hmd.GetHmd();
      ovrTrackingState ss = hmd.GetTrackingState();
      bool isConnected = (ss.StatusFlags & (uint)ovrStatusBits.ovrStatus_HmdConnected) != 0;
      if (isConnected)
      {
        riftCamera.SetActive(true);
        normalCamera.SetActive(false);
        handController.transform.parent = riftCamera.GetComponent<OVRCameraController>().CameraMain.transform;
        handController.transform.rotation = riftCamera.GetComponent<OVRCameraController>().CameraMain.transform.rotation * handController.transform.rotation;
      }
      else
      {
        riftCamera.SetActive(false);
        normalCamera.SetActive(true);
        handController.transform.parent = normalCamera.transform;
        handController.transform.rotation = normalCamera.transform.rotation * handController.transform.rotation;
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
}