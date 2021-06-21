using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    #region PRIVATE_MEMBERS

    bool IsPlaced;

    #endregion // PRIVATE MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        this.IsPlaced = false;
    }

    #endregion //MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    public void SetPlaced(bool var)
    {
        this.IsPlaced = var;
    }

    public bool GetPlaced()
    {
        return IsPlaced;
    }

    #endregion //PUBLIC_METHODS
}
