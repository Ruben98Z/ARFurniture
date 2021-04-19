using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class FurnitureManager : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    public TouchHandler touchHandler;
    public ProductPlacement productPlacement;

    #endregion // PUBLIC MEMBERS

    StateManager stateManager;
    SmartTerrain smartTerrain;
    [SerializeField] PlaneFinderBehaviour planeFinder = null;
    ContentPositioningBehaviour contentPositioningBehaviour;
    AnchorBehaviour planeAnchor;
    SmartTerrain terrain;
    PositionalDeviceTracker positionalDeviceTracker;
    GameObject currentFurniture;

    static TrackableBehaviour.Status StatusCached = TrackableBehaviour.Status.NO_POSE;
    static TrackableBehaviour.StatusInfo StatusInfoCached = TrackableBehaviour.StatusInfo.UNKNOWN;

    public static bool TrackingStatusIsTrackedAndNormal
    {
        get
        {
            return
                (StatusCached == TrackableBehaviour.Status.TRACKED ||
                 StatusCached == TrackableBehaviour.Status.EXTENDED_TRACKED) &&
                StatusInfoCached == TrackableBehaviour.StatusInfo.NORMAL;
        }
    }



    void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

        this.contentPositioningBehaviour = this.planeFinder.GetComponent<ContentPositioningBehaviour>();
        this.currentFurniture = productPlacement.GetFurniture();
        this.planeAnchor = this.currentFurniture.GetComponentInParent<AnchorBehaviour>();
        this.contentPositioningBehaviour.AnchorStage = this.planeAnchor;
        
    }

    public void ChangeAnchor(GameObject anchor)
    {
        this.productPlacement.SetIsPlaced(false);
        //this.planeAnchor = this.furniture.transform.Find(nameAnchor).GetComponent<AnchorBehaviour>();
        this.planeAnchor = anchor.GetComponent<AnchorBehaviour>();
        this.contentPositioningBehaviour.AnchorStage = this.planeAnchor;
    }

    public void ChangeTouchFurniture(GameObject newFurniture)
    {
        this.currentFurniture = newFurniture;
        this.touchHandler.TouchFurniture(newFurniture.transform);
        this.productPlacement.changeFurniture(newFurniture);
    }


    public void RemoveFurniture()
    {
        this.terrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        // Stop and restart trackers
        this.terrain.Stop(); 
        this.positionalDeviceTracker.Reset();
        this.terrain.Start(); 
    }


    public void OnAutomaticHitTest(HitTestResult result)
    {

        if (this.productPlacement.IsPlaced)
        {
            this.productPlacement.RemoveAnchor();
            ////this.currentFurniture.PositionAt(result.Position);
        }
    }


    public void OnInteractiveHitTest(HitTestResult result)
    {
        if (result == null)
        {
            Debug.LogError("Invalid hit test result!");
            return;
        }

        this.contentPositioningBehaviour.DuplicateStage = false;
        if (TrackingStatusIsTrackedAndNormal)
        {
            this.contentPositioningBehaviour.AnchorStage = this.planeAnchor;
            this.contentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
            this.productPlacement.PlaceProduct(this.planeAnchor.transform);
            UtilityHelper.EnableRendererColliderCanvas(this.currentFurniture, true);
            this.productPlacement.SetIsPlaced(true);

        }
    }


    #region VUFORIA_CALLBACKS

    void OnVuforiaStarted()
    {

        stateManager = TrackerManager.Instance.GetStateManager();

        // Check trackers to see if started and start if necessary
        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (this.positionalDeviceTracker != null && this.smartTerrain != null)
        {
            if (!this.positionalDeviceTracker.IsActive)
            {
                Debug.LogError("The Ground Plane feature requires the Device Tracker to be started. " +
                               "Please enable it in the Vuforia Configuration or start it at runtime through the scripting API.");
                return;
            }

            if (this.positionalDeviceTracker.IsActive && !this.smartTerrain.IsActive)
                this.smartTerrain.Start();
        }
        else
        {
            if (this.positionalDeviceTracker == null)
                Debug.Log("PositionalDeviceTracker returned null. GroundPlane not supported on this device.");
            if (this.smartTerrain == null)
                Debug.Log("SmartTerrain returned null. GroundPlane not supported on this device.");
        }
    }

    void OnVuforiaPaused(bool paused)
    {
        Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");
    }

    #endregion // VUFORIA_CALLBACKS


    #region DEVICE_TRACKER_CALLBACKS

    void OnTrackerStarted()
    {

        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (this.positionalDeviceTracker != null && this.smartTerrain != null)
        {
            if (!this.positionalDeviceTracker.IsActive)
            {
                Debug.LogError("The Ground Plane feature requires the Device Tracker to be started. " +
                               "Please enable it in the Vuforia Configuration or start it at runtime through the scripting API.");
                return;
            }

            if (!this.smartTerrain.IsActive)
                this.smartTerrain.Start();

            Debug.Log("PositionalDeviceTracker is Active?: " + this.positionalDeviceTracker.IsActive +
                      "\nSmartTerrain Tracker is Active?: " + this.smartTerrain.IsActive);
        }
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
    {
        Debug.Log("PlaneManager.OnDevicePoseStatusChanged(" + status + ", " + statusInfo + ")");

        StatusCached = status;
        StatusInfoCached = statusInfo;

        switch (statusInfo)
        {
            case TrackableBehaviour.StatusInfo.NORMAL:
                break;
            case TrackableBehaviour.StatusInfo.UNKNOWN:
                break;
            case TrackableBehaviour.StatusInfo.INITIALIZING:
                break;
            case TrackableBehaviour.StatusInfo.EXCESSIVE_MOTION:
                break;
            case TrackableBehaviour.StatusInfo.INSUFFICIENT_FEATURES:
                break;
            case TrackableBehaviour.StatusInfo.INSUFFICIENT_LIGHT:
                break;
            default:
                break;
        }
    }

    #endregion // DEVICE_TRACKER_CALLBACK_METHODS
}

