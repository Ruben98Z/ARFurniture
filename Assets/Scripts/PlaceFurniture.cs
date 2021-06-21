using UnityEngine;
using Vuforia;

public class PlaceFurniture : MonoBehaviour
{



    #region PRIVATE_MEMBERS
    GameObject furniture;


    Camera mainCamera;
    Ray cameraToPlane;
    RaycastHit cameraHit;

    string floorName;
    GameObject floor;

    Model furniturePlaced;

    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        this.mainCamera = Camera.main;

    }

    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS

    public void UpdatePosition()
    {
        //Se comprueba que solo un dedo esta tocando la pantalla
        if (TouchController.IsSingleFingerDragging)
        {
            //Se obtiene el rayo que va desde la camara hasta el plano
            this.cameraToPlane = this.mainCamera.ScreenPointToRay(Input.mousePosition);

            //Si el rayo interacciona con el objeto cuyo nombre sea el del suelo entonces se posiciona el mueble donde este ha interseccionado con el plano
            if (Physics.Raycast(this.cameraToPlane, out this.cameraHit))
            {
                if (this.cameraHit.collider.gameObject.name == floorName)
                {
                    this.furniture.PositionAt(this.cameraHit.point);
                }

            }

        }
    }

    public void ChangeFurniture(GameObject newFurniture)
    {
        this.furniture = newFurniture;
        this.furniturePlaced = this.furniture.GetComponent<Model>();
    }

    public void PlaceProduct(Transform anchor)
    {
        this.furniture.transform.SetParent(anchor, true);
        SetFloor();
    }


    public void RemoveAnchor()
    {
        this.furniture.transform.SetParent(null);
    }


    public void SetIsPlaced(bool var)
    {
        this.furniturePlaced.SetPlaced(var);
    }

    public bool GetIsPlaced()
    {
        return this.furniturePlaced.GetPlaced();
    }

    public GameObject GetFurniture()
    {
        return this.furniture;
    }

    

    public void SetupFloor()
    {
        //Si la aplicación esta en PlayMode entonces se genera un suelo llamado "Emulator Ground Plane"
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

        Debug.Log(this.floorName);
    }

    public void SetFloor()
    {

        this.floor.transform.SetParent(this.furniture.transform.parent);
        Debug.Log(this.floorName);
    }


    #endregion //PUBLIC_METHODS

}
