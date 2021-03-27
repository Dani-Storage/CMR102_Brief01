
using System.Collections;
using System.Collections.Generic;
using Vuforia;
using UnityEngine;
using UnityEngine.UI;

public class DualTracker : MonoBehaviour, ITrackableEventHandler
{
    public enum Orientation
    {
        horizontal,
        vertical
    }

    public Orientation currentRotation; // value of horizontal or vertical results

    public Transform arCamera; // ref to AR Camera
    public TrackableBehaviour trackableBehaviour; // reference to AR Marker script

    private Transform rotationalHelper;  // runtime transform create assists determining orientation of marker

    public float trackerThreshold = 0.6f; // determines when marker is horizontal or vertical

    public Text debugDeviceAngle; // piece text used to show what angle device is
    public Text debugMarkerOrientation; // text used to show orientation marker currently in

    public bool trackerFound = false; // detected tracker?

    public float currentAngleCompared; // current value angle comparisons

    public GameObject gameWorld; // a ref to our gameworld

    public Transform verticalTracker; // vertical tracker ref
    public Transform horizontalTracker; // horizontal track ref 


    // Start is called before the first frame update
    void Start()
    {
        if (trackableBehaviour != null)
        {
            trackableBehaviour.RegisterTrackableEventHandler(this);
        }

        rotationalHelper = new GameObject($"{trackableBehaviour.TrackableName.ToString()} Rotation Helper ").transform;
        rotationalHelper.position = Vector3.zero;
        rotationalHelper.rotation = Quaternion.identity;
        OnTrackingLost(); // just hide everything
    }

    // Update is called once per frame
    void Update()
    {
        if (debugDeviceAngle !=null)
        {
            debugDeviceAngle.text = $"Current Angle: {currentAngleCompared}"; //set the text to the angle
        }
        if (debugMarkerOrientation != null)
        {
            debugMarkerOrientation.text = currentRotation.ToString(); // set the text to the rotation
        }

    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            if (trackerFound == false)
            {
                trackerFound = true;
                OnTrackingFound();
            }
            else
            {
                if (trackerFound == true)
                {
                    trackerFound = false;
                    OnTrackingLost();
                }
            }
        }
    }

    private void OnTrackingFound()
    {
        UpdateTrackerRotation(); // update the tracjer rotation of the tracker. 
        gameWorld.SetActive(true); 
    }

    private void OnTrackingLost()
    {
        gameWorld.SetActive(false); // turn our gameworld object off
    }


    /// <summary>
    /// Gets and updates the rotation of our devices camera.
    /// </summary>

    void UpdateTrackerRotation()
    {
        ConvertDeviceRotation(); // convert our current device input.

        rotationalHelper.eulerAngles = new Vector3(rotationalHelper.eulerAngles.x, arCamera.eulerAngles.y, arCamera.eulerAngles.z); // grab the cameras local y and z orientation and match it to the rotation.
        currentAngleCompared = Vector3.Dot(rotationalHelper.rotation * Vector3.forward, arCamera.forward);// compare the difference in the world forward , and the ar camera forward direction.

        if (currentAngleCompared < trackerThreshold)
        
        {
            //we are more than threshold we must be vertical
            currentRotation = Orientation.horizontal;
            transform.position = horizontalTracker.position; // set our position to our horizontal tracker
            transform.rotation = horizontalTracker.rotation; // set our rotation to our horizontal tracker
        }
        else if (currentAngleCompared > trackerThreshold)
        {
            // we are more than threshold we must be vertical
            currentRotation = Orientation.vertical;
            transform.position = verticalTracker.position; // set our position to our vertical tracker
            transform.rotation = verticalTracker.rotation; // set our rotation to our vertical tracker
        }
    }

    /// <summary>
    /// essentially the gyro information and convert it so unity can handle the input
    /// </summary>
    private void ConvertDeviceRotation()
    {
        rotationalHelper.rotation = Input.gyro.attitude; // match the rotation of the gyro;
        rotationalHelper.Rotate(0, 0, 180f, Space.Self); // rotate around its local axis and swap it from the gyro's input
        rotationalHelper.Rotate(90, 180, 0, Space.World); // rotate it make sense of the camera facing from the back of your phone camera // save the rotation
    }

}