using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{

    bool IsPlaced;

    // Start is called before the first frame update
    void Start()
    {
        this.IsPlaced = false;
    }

    public void SetPlaced(bool var)
    {
        this.IsPlaced = var;
    }

    public bool GetPlaced()
    {
        return IsPlaced;
    }
}
