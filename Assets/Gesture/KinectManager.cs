﻿using UnityEngine;
using System;
using System.Collections.Generic;
using Windows.Kinect;
using UnityEngine.UI;

public class KinectManager : MonoBehaviour
{
    public GameObject Player;
    private CharacterSidewaysMovement turnScript;


    private KinectSensor kinectSensor;

    // color frame and data 
    private ColorFrameReader colorFrameReader;
    private byte[] colorData;
    private Texture2D colorTexture;

    private BodyFrameReader bodyFrameReader;
    private int bodyCount;
    private Body[] bodies;

    private string leanLeftGestureName = "Lean_Left";
    private string leanRightGestureName = "Lean_Right";
    private readonly string jumpGestureName = "Jump";

    // GUI output
    private UnityEngine.Color[] bodyColors;

    /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
    private List<GestureDetector> gestureDetectorList = null;

    // Use this for initialization
    void Start()
    {
        // Debug.Log("kinect didn't connect");
        turnScript = Player.GetComponent<CharacterSidewaysMovement>();
        // get the sensor object

        this.kinectSensor = KinectSensor.GetDefault();
        //Debug.Log("get values");
        if (this.kinectSensor != null)
        {
            this.bodyCount = this.kinectSensor.BodyFrameSource.BodyCount;

            // color reader
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            // create buffer from RGBA frame description
            var desc = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);


            // body data
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // body frame to use
            this.bodies = new Body[this.bodyCount];

            // initialize the gesture detection objects for our gestures
            this.gestureDetectorList = new List<GestureDetector>();
            for (int bodyIndex = 0; bodyIndex < this.bodyCount; bodyIndex++)
            {
                //PUT UPDATED UI STUFF HERE FOR NO GESTURE
                // GestureTextGameObject.text = "none";
                //this.bodyText[bodyIndex] = "none";
                this.gestureDetectorList.Add(new GestureDetector(this.kinectSensor));
            }

            // start getting data from runtime
            this.kinectSensor.Open();
        }
        else
        {
            //kinect sensor not connected
            Debug.Log("kinect didn't connect");
        }
    }

    // Update is called once per frame
    void Update()
    {

        // process bodies
        bool newBodyData = false;
        using (BodyFrame bodyFrame = this.bodyFrameReader.AcquireLatestFrame())
        {
            if (bodyFrame != null)
            {
                bodyFrame.GetAndRefreshBodyData(this.bodies);
                newBodyData = true;
            }
        }

        if (newBodyData)
        {
            // update gesture detectors with the correct tracking id
            for (int bodyIndex = 0; bodyIndex < this.bodyCount; bodyIndex++)
            {
                var body = this.bodies[bodyIndex];
                if (body != null)
                {
                    var trackingId = body.TrackingId;

                    // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                    if (trackingId != this.gestureDetectorList[bodyIndex].TrackingId)
                    {
                        // GestureTextGameObject.text = "none";
                        this.gestureDetectorList[bodyIndex].TrackingId = trackingId;

                        // if the current body is tracked, unPause its detector to get VisualGestureBuilderFrameArrived events
                        // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                        this.gestureDetectorList[bodyIndex].IsPaused = (trackingId == 0);
                        this.gestureDetectorList[bodyIndex].OnGestureDetected += CreateOnGestureHandler(bodyIndex);
                    }
                }
            }
        }

    }

    private EventHandler<GestureEventArgs> CreateOnGestureHandler(int bodyIndex)
    {
        return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e, bodyIndex);
    }

    private void OnGestureDetected(object sender, GestureEventArgs e, int bodyIndex)
    {
        if (e.GestureID == leanLeftGestureName)
        {
            //NEW UI FOR GESTURE DETECTed 
            if (e.DetectionConfidence > 0.65f) //65%
            {
                turnScript.moveLeft = true;
                turnScript.moveRight = false;
            }
        }

        //Debug.Log(e.GestureID);
        if (e.GestureID == leanRightGestureName)
        {
            //NEW UI FOR GESTURE DETECTed
            if (e.DetectionConfidence > 0.65f)
            {
                turnScript.moveRight = true;
                turnScript.moveLeft = false;
            }
        }

        if (e.GestureID == jumpGestureName)
        {
            if (e.DetectionConfidence > 0.20f)
            {
                turnScript.jump = true;
            }
            else
            {
                turnScript.jump = false;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (this.colorFrameReader != null)
        {
            this.colorFrameReader.Dispose();
            this.colorFrameReader = null;
        }

        if (this.bodyFrameReader != null)
        {
            this.bodyFrameReader.Dispose();
            this.bodyFrameReader = null;
        }

        if (this.kinectSensor != null)
        {
            if (this.kinectSensor.IsOpen)
            {
                this.kinectSensor.Close();
            }

            this.kinectSensor = null;
        }
    }

}
