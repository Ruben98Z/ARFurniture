using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class FurnitureManager : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    public GameObject furniture;
    public TouchHandler touchHandler;
    public ProductPlacement productPlacement;

    #endregion // PUBLIC MEMBERS

    [SerializeField] PlaneFinderBehaviour planeFinder = null;
    ContentPositioningBehaviour contentPositioningBehaviour;
    AnchorBehaviour planeAnchor;



    public void ChangeAnchor(GameObject anchor)
    {
        this.contentPositioningBehaviour = this.planeFinder.GetComponent<ContentPositioningBehaviour>();
        //this.planeAnchor = this.furniture.transform.Find(nameAnchor).GetComponent<AnchorBehaviour>();
        this.planeAnchor = anchor.GetComponent<AnchorBehaviour>();
        this.contentPositioningBehaviour.AnchorStage = this.planeAnchor;
    }

    public void ChangeTouchFurniture(GameObject newFurniture)
    {
        this.touchHandler.TouchFurniture(newFurniture.transform);
        this.productPlacement.changeFurniture(newFurniture);
    }


}
