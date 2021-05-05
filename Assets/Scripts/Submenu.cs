using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Submenu : MonoBehaviour
{

    #region PUBLIC_MEMBERS

    public Transform viewport;
    public int numViews;

    #endregion // PUBLIC MEMBERS

    List<Transform> views;
    Transform currentView;


    // Start is called before the first frame update
    void Start()
    {
        Transform[] array = viewport.GetComponentsInChildren<Transform>();
        List<Transform> list = new List<Transform>(array);
        views = new List<Transform>();
        for(int i = 0; i < numViews; i++)
        {
            Transform aux = list.Where(obj => obj.name == "Content"+i).SingleOrDefault();
            views.Add(aux);
            if(i == 0)
            {
                currentView = aux;
            }
            else
            {
                aux.gameObject.SetActive(false);
            }
        }
        
    }

    
    public void setViewActive(Transform view)
    {
        currentView.gameObject.SetActive(false);
        view.gameObject.SetActive(true);
        currentView = view;
    }

}
