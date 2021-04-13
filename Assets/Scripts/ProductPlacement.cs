using UnityEngine;
using Vuforia;

public class ProductPlacement : MonoBehaviour
{


    #region PRIVATE_MEMBERS
    [Header("Augmentation Objects")]
    [SerializeField] GameObject furniture = null;


    [Header("Augmentation Size")]
    [Range(0.1f, 2.0f)]
    [SerializeField] float productSize = 0.65f;


    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit cameraToPlaneHit;

    float augmentationScale;
    Vector3 productScale;
    string floorName;

    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        this.mainCamera = Camera.main;

        SetupFloor();


        this.augmentationScale = VuforiaRuntimeUtilities.IsPlayMode() ? 0.1f : this.productSize;

        this.productScale =
            new Vector3(this.augmentationScale,
                        this.augmentationScale,
                        this.augmentationScale);

        this.furniture.transform.localScale = this.productScale;
    }


    void Update()
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
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS

    public void changeFurniture(GameObject newFurniture)
    {
        this.furniture = newFurniture;
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS

    void SetupFloor()
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
