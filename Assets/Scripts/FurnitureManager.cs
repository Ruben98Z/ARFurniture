using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.SceneManagement;

public class FurnitureManager : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    public TouchController touchController;
    public PlaceFurniture placeFurniture;
    public GameObject currentFurniture;

    #endregion // PUBLIC MEMBERS


    #region PRIVATE_MEMBERS

    StateManager stateManager;
    SmartTerrain smartTerrain;
    [SerializeField] PlaneFinderBehaviour planeFinder = null;
    ContentPositioningBehaviour contentPositioningBehaviour;
    AnchorBehaviour planeAnchor;
    PositionalDeviceTracker positionalDeviceTracker;
    bool activeUI = true;


    static TrackableBehaviour.Status StatusCached = TrackableBehaviour.Status.NO_POSE;
    static TrackableBehaviour.StatusInfo StatusInfoCached = TrackableBehaviour.StatusInfo.UNKNOWN;


    #endregion // PRIVATE MEMBERS


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


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        //Llamadas de Vuforia
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);


        SetActive();

        //Se inicializa el modelo
        this.placeFurniture.ChangeFurniture(this.currentFurniture);
        this.touchController.TouchFurniture(this.currentFurniture.transform);
        this.placeFurniture.SetupFloor();       
        this.touchController.SetScaleRotation();

        this.contentPositioningBehaviour = this.planeFinder.GetComponent<ContentPositioningBehaviour>();
        this.planeAnchor = this.currentFurniture.GetComponentInParent<AnchorBehaviour>();
        this.contentPositioningBehaviour.AnchorStage = this.planeAnchor;
        
    }

    void Update()
    {
        if (!this.activeUI)
        {
            this.placeFurniture.UpdatePosition();

            this.touchController.Rotate();
        }
    }



    #endregion //MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    public void ChangeAnchorFurniture(GameObject anchor)
    {
        //Cambia el Anchor
        this.planeAnchor = anchor.GetComponent<AnchorBehaviour>();
        this.contentPositioningBehaviour.AnchorStage = this.planeAnchor;
        
    }

    public void HandleFurniture(GameObject newFurniture)
    {
        //Cambia el mueble que se vaya a manejar
        this.currentFurniture = newFurniture;
        this.touchController.TouchFurniture(newFurniture.transform);
        this.placeFurniture.ChangeFurniture(newFurniture);
    }



    public void RemoveFurniture()
    {
        //Se actualiza el estado del mueble y se le vuelve a asignar su Anchor
        this.placeFurniture.SetIsPlaced(false);
        this.placeFurniture.PlaceProduct(this.planeAnchor.transform);
        

        this.smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
        this.positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        // Stop and restart trackers
        this.smartTerrain.Stop();
        this.positionalDeviceTracker.Reset();
        this.smartTerrain.Start();

        // Set Floor
        this.placeFurniture.SetFloor();

    }

    public void SetActive()
    {
        this.activeUI = !this.activeUI;
    }


    public void OnAutomaticHitTest(HitTestResult result)
    {
        //Cuando el mueble este en el entorno se quita su Anchor
        if (this.placeFurniture.GetIsPlaced())
        {
            this.placeFurniture.RemoveAnchor();
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
        //Si se ha detectado el plano y la interfaz esta incativa se coloca el mueble
        if (TrackingStatusIsTrackedAndNormal && !this.activeUI)
        {
            this.contentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
            this.placeFurniture.PlaceProduct(this.planeAnchor.transform);
            UtilityHelper.EnableRendererColliderCanvas(this.currentFurniture, true);
            this.placeFurniture.SetIsPlaced(true);

        }


    }

    #endregion //PUBLIC_METHODS


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

