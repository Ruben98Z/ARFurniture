using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public RectTransform menu;
    float lastPosition;
    bool open = false;
    public float timeMenu = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = Screen.width / 2;
        menu.position = new Vector3(-lastPosition, menu.position.y, 0);
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

    void moveMenu(float time, Vector3 initPos, Vector3 lastPos)
    {
        StartCoroutine(move(time, initPos, lastPos));
    }


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

    // Update is called once per frame
    void Update()
    {
        
    }
}
