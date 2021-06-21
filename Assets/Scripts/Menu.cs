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

    float uiPosition;
    bool open = false;

    List<Transform> views;
    RectTransform currentView;
    ScrollRect scrollView;

    Text messageText;
    GameObject message;
    bool scaleActive = false;
    float timeMessage = 1.0f;

    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        initMenu();
        initSubmenus();
        initMessage();
        
    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS


    void initMenu()
    {
        //Menu
        uiPosition = Screen.width / 2;
        menu.position = new Vector3(-uiPosition, menu.position.y, 0);
        Debug.Log(uiPosition);
    }

    void initMessage()
    {
        this.messageText = GameObject.Find("MessageText").GetComponent<Text>();
        this.message = GameObject.Find("Message");
        this.message.gameObject.SetActive(false);
    }


    void initSubmenus()
    {
        //Submenus
        Transform[] array = menu.GetComponentsInChildren<Transform>();
        List<Transform> list = new List<Transform>(array);
        views = new List<Transform>();
        int i = 0;
        bool lastView = false;
        while (!lastView)
        {
            Transform content = list.Where(obj => obj.name == "Content" + i).SingleOrDefault();
            if (content != null)
            {
                views.Add(content);
                if (i == 0)
                {
                    currentView = content.GetComponent<RectTransform>();
                }
                else
                {
                    RectTransform contentRectTransform = content.GetComponent<RectTransform>();
                    contentRectTransform.position = new Vector3(-uiPosition*4, contentRectTransform.position.y, 0);


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

    IEnumerator display(float time)
    {
        this.message.SetActive(true);
        yield return new WaitForSeconds(time);
        this.message.SetActive(false);
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

        moveMenu(timeMenu, menu.position, new Vector3(num * uiPosition, menu.position.y, 0));
        open = !open;
    }

    public void SetViewActive(RectTransform view)
    {
        currentView.position = new Vector3(-uiPosition, currentView.position.y, currentView.position.z);
        view.position = new Vector3(uiPosition, view.position.y, view.position.z);

        currentView = view;
        scrollView.content = currentView.GetComponent<RectTransform>();
    }

    public void ActiveScale()
    {
        if (scaleActive)
        {
            this.messageText.text = "Escalado desactivado";
            scaleActive = false;
        }
        else
        {
            this.messageText.text = "Escalado activado";
            scaleActive = true;
        }

        StartCoroutine(display(timeMessage));
    }

    #endregion //PUBLIC_METHODS
}
