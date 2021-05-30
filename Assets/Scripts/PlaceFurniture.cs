using UnityEngine;
using Vuforia;

public class PlaceFurniture : MonoBehaviour
{


    public bool IsPlaced { get; private set; }

    #region PRIVATE_MEMBERS
    [SerializeField] GameObject furniture = null;


    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit cameraHit;

    string floorName;
    GameObject floor;

    bool activeUI;

    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        this.mainCamera = Camera.main;

        SetupFloor();

    }


    void Update()
    {

        if (!activeUI)
        {
            if (TouchController.IsSingleFingerDragging || (VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0)))
            {
                this.cameraToPlaneRay = this.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(this.cameraToPlaneRay, out this.cameraHit))
                {
                    if (this.cameraHit.collider.gameObject.name == floorName)
                    {
                        this.furniture.PositionAt(this.cameraHit.point);
                    }

                    //if (this.cameraHit.collider.gameObject.name != floorName && this.cameraHit.collider.gameObject.name != "Emulator Ground Plane")
                    //{
                    //    GameObject product = GameObject.Find(this.cameraHit.collider.gameObject.name);
                    //    ChangeFurniture(product);
                        
                    //}

                }

            }
        }            
            
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS

    public void ChangeFurniture(GameObject newFurniture)
    {
        this.furniture = newFurniture;
    }

    public void PlaceProduct(Transform anchor)
    {
        this.furniture.transform.SetParent(anchor, true);
        SetFloor();
    }


    //Revisar
    public void RemoveAnchor()
    {
        this.furniture.transform.SetParent(null);
    }

    //Revisar
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
            this.floor = new GameObject(this.floorName, typeof(BoxCollider));
            this.floor.transform.SetParent(this.furniture.transform.parent);
            this.floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            this.floor.transform.localScale = Vector3.one;
            this.floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
        }
    }

    public void SetFloor()
    {
           
            this.floor.transform.SetParent(this.furniture.transform.parent);
        
    }


    #endregion // PRIVATE_METHODS

}
