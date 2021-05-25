using UnityEngine;
using Vuforia;

public class ProductPlacement : MonoBehaviour
{


    public bool IsPlaced { get; private set; }

    #region PRIVATE_MEMBERS
    [SerializeField] GameObject furniture = null;
    [SerializeField] float productSize = 1f;


    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit cameraToPlaneHit;

    float scale;
    Vector3 productScale;
    string floorName;

    bool activeUI;

    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        this.mainCamera = Camera.main;

        SetupFloor();


        this.scale = VuforiaRuntimeUtilities.IsPlayMode() ? 0.1f : this.productSize;

        this.productScale = new Vector3(this.scale, this.scale, this.scale);

        this.furniture.transform.localScale = this.productScale;
    }


    void Update()
    {
        if (!activeUI)
        {
            if (TouchHandler.IsSingleFingerDragging || (VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0)))
            {
                this.cameraToPlaneRay = this.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(this.cameraToPlaneRay, out this.cameraToPlaneHit))
                {
                    if (this.cameraToPlaneHit.collider.gameObject.name == floorName)
                    {
                        this.furniture.PositionAt(this.cameraToPlaneHit.point);
                    }
                }

            }
        }            
            
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS

    public void changeFurniture(GameObject newFurniture)
    {
        this.furniture = newFurniture;
    }

    public void PlaceProduct(Transform anchor)
    {
        this.furniture.transform.SetParent(anchor, true);
    }

    public void RemoveAnchor()
    {
        this.furniture.transform.SetParent(null);
    }

    public void ResetPosition()
    {
        this.furniture.transform.position = Vector3.zero;
    }

    public void SetIsPlaced(bool var)
    {
        this.IsPlaced = var;
    }

    public GameObject GetFurniture()
    {
        return this.furniture;
    }

    public void SetActiveUI(bool var)
    {
        this.activeUI = var;
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS

    public void SetupFloor()
    {
        if (VuforiaRuntimeUtilities.IsPlayMode())
        {
            this.floorName = "Emulator Ground Plane";
        }
        else
        {
            this.floorName = "Floor";
            GameObject floor = new GameObject(this.floorName, typeof(BoxCollider));
            floor.transform.SetParent(this.furniture.transform.parent);
            floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            floor.transform.localScale = Vector3.one;
            floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
        }
    }
    #endregion // PRIVATE_METHODS

}
