using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Menu : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    public RectTransform menu;
    public float timeMenu = 0.5f;

    #endregion // PUBLIC MEMBERS

    #region PRIVATE_MEMBERS

    float lastPosition;
    bool open = false;

    List<Transform> views;
    Transform currentView;
    ScrollRect scrollView;

    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        //Menu
        lastPosition = Screen.width / 2;
        menu.position = new Vector3(-lastPosition, menu.position.y, 0);

        //Submenus
        Transform[] array = menu.GetComponentsInChildren<Transform>();
        List<Transform> list = new List<Transform>(array);
        views = new List<Transform>();
        int i = 0;
        bool lastView = false;
        while(!lastView)
        {
            Transform aux = list.Where(obj => obj.name == "Content" + i).SingleOrDefault();
            if(aux != null)
            {
                views.Add(aux);
                if (i == 0)
                {
                    currentView = aux;
                }
                else
                {
                    aux.gameObject.SetActive(false);
                }
                i++;
            }
            else
            {
                lastView = true;
            }
            
        }

        Transform scrollObject = list.Where(obj => obj.name == "ScrollView").SingleOrDefault();
        scrollView = scrollObject.GetComponent<ScrollRect>();
    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    IEnumerator move(float time, Vector3 initPos, Vector3 lastPos)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            menu.position = Vector3.Lerp(initPos, lastPos, (elapsedTime/time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        menu.position = lastPos;
    }



    void moveMenu(float time, Vector3 initPos, Vector3 lastPos)
    {
        StartCoroutine(move(time, initPos, lastPos));
    }


    #endregion // PRIVATE_METHODS

    #region PUBLIC_METHODS

    public void OnClickDropMenu()
    {
        int num = -1;
        if (!open)
        {
            num = 1;
        }

        moveMenu(timeMenu, menu.position, new Vector3(num * lastPosition, menu.position.y, 0));
        open = !open;
    }

    public void setViewActive(Transform view)
    {
        currentView.gameObject.SetActive(false);
        view.gameObject.SetActive(true);
        currentView = view;
        scrollView.content = currentView.GetComponent<RectTransform>();
    }

    #endregion //PUBLIC_METHODS
}
