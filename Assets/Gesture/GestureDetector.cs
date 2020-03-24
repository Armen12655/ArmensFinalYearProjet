using Microsoft.Kinect.VisualGestureBuilder;
using System;
using System.IO;
using UnityEngine;
using Windows.Kinect;

public class GestureEventArgs : EventArgs
{
    public bool IsBodyTrackingIdValid { get; private set; }

    public bool IsGestureDetected { get; private set; }

    public float DetectionConfidence { get; private set; }

    //my modification
    public string GestureID { get; private set; }

    //my mod
    public GestureEventArgs(bool isBodyTrackingIdValid, bool isGestureDetected, float detectionConfidence, string gestureID) //added for more players
    {
        this.IsBodyTrackingIdValid = isBodyTrackingIdValid;
        this.IsGestureDetected = isGestureDetected;
        this.DetectionConfidence = detectionConfidence;
        this.GestureID = gestureID;
    }
}

public class GestureDetector : IDisposable
{
    private readonly string leanDB = "C:\\Users\\armen\\OneDrive\\Project\\ArmensFinalYearProject\\Assets\\StreamingAssest\\GestureDB\\Lean.gbd";
    private readonly string JumpDB = "C:\\Users\\armen\\OneDrive\\Project\\ArmensFinalYearProject\\Assets\\StreamingAssest\\GestureDB\\jumpGesture.gbd";

    /// <summary> Name of the discrete gesture in the database that we want to track </summary>
    private readonly string leanLeftGestureName = "Lean_Left";
    private readonly string leanRightGestureName = "Lean_Right";
    private readonly string jumpGestureName = "Jump";

    private VisualGestureBuilderFrameSource vgbFrameSource = null;

    private VisualGestureBuilderFrameReader vgbFrameReader = null;

    public event EventHandler<GestureEventArgs> OnGestureDetected;

    public GestureDetector(KinectSensor kinectSensor)
    {
        if (kinectSensor == null)
        {
            throw new ArgumentNullException("kinectSensor");
        }

        // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
        this.vgbFrameSource = VisualGestureBuilderFrameSource.Create(kinectSensor, 0);
        this.vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

        // open the reader for the vgb frames
        this.vgbFrameReader = this.vgbFrameSource.OpenReader();
        if (this.vgbFrameReader != null)
        {
            this.vgbFrameReader.IsPaused = true;
            this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
        }

        // load the 'Lean' gesture from the gesture database
        var databasePath = Path.Combine(Application.streamingAssetsPath, this.leanDB);
        using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(databasePath))
        {
            //we only want to track one discrete gesture from the database, so we'll load it by name
            foreach (Gesture gesture in database.AvailableGestures)
            {
                if (gesture.Name.Equals(this.leanLeftGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
                if (gesture.Name.Equals(this.leanRightGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
            }
        }

        // load the 'jump' gesture from the gesture database
        var databasePathForJump = Path.Combine(Application.streamingAssetsPath, this.JumpDB);
        using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(databasePathForJump))
        {

            // we could load all available gestures in the database with a call to vgbFrameSource.AddGestures(database.AvailableGestures), 
            // but for this program, we only want to track one discrete gesture from the database, so we'll load it by name
            foreach (Gesture gesture in database.AvailableGestures)
            {
                if (gesture.Name.Equals(this.jumpGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
            }
        }
    }

    public ulong TrackingId
    {
        get
        {
            return this.vgbFrameSource.TrackingId;
        }

        set
        {
            if (this.vgbFrameSource.TrackingId != value)
            {
                this.vgbFrameSource.TrackingId = value;
            }
        }
    }

    public bool IsPaused
    {
        get
        {
            return this.vgbFrameReader.IsPaused;
        }

        set
        {
            if (this.vgbFrameReader.IsPaused != value)
            {
                this.vgbFrameReader.IsPaused = value;
            }
        }
    }

    // Disposes all unmanaged resources for the class
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                this.vgbFrameReader.Dispose();
                this.vgbFrameReader = null;
            }

            if (this.vgbFrameSource != null)
            {
                this.vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                this.vgbFrameSource.Dispose();
                this.vgbFrameSource = null;
            }
        }
    }

    /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
    /// The Reader_GestureFrameArrived
    private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
    {
        VisualGestureBuilderFrameReference frameReference = e.FrameReference;
        using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
        {
            if (frame != null)
            {
                // get the discrete gesture results which arrived with the latest frame
                var discreteResults = frame.DiscreteGestureResults;

                if (discreteResults != null)
                {
                    // we only have one gesture in this source object, but you can get multiple gestures
                    foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                    {

                        if (gesture.Name.Equals(this.leanLeftGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.leanLeftGestureName));
                                }
                            }
                        }

                        if (gesture.Name.Equals(this.leanRightGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.leanRightGestureName));
                                }
                            }
                        }

                        if (gesture.Name.Equals(this.jumpGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.jumpGestureName));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
    /// The Source_TrackingIdLost
    private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
    {
        if (this.OnGestureDetected != null)
        {
            this.OnGestureDetected(this, new GestureEventArgs(false, false, 0.0f, "none"));
        }
    }
}
