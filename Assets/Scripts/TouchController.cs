using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TouchController : MonoBehaviour
{
    #region PUBLIC_MEMBERS

   

    public static bool IsSingleFingerStationary => IsSingleFingerDown() && (Input.GetTouch(0).phase == TouchPhase.Stationary);

    public static bool IsSingleFingerDragging => IsSingleFingerDown() && (Input.GetTouch(0).phase == TouchPhase.Moved);

    #endregion // PUBLIC MEMBERS


    #region PRIVATE_MEMBERS
    Transform furnitureTransform;

    const float ScaleRangeMin = 0.1f;
    const float ScaleRangeMax = 2.0f;

    Touch[] touches;
    static int lastTouchCount;
    bool firstTimeFingersTouchScreen;
    bool enableScale;
    float initialAngle;
    float initialDistance;
    float furnitureScale;
    Vector3 furnitureEulerAngles;

    #endregion // PRIVATE_MEMBERS


    #region  PUBLIC_METHODS 

    public void Rotate()
    {
        if (Input.touchCount == 2)
        {
            //Se obtienen los dos toques
            var firstTouch = Input.GetTouch(0);
            var secondTouch = Input.GetTouch(1);

            //Se calcula la distancia que hay entre estos dos 
            float currentTouchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);

            //Con la diferencia entre la posición de las coordenadas X e Y de los toques se calcula la tangente
            float diff_y = firstTouch.position.y - secondTouch.position.y;
            float diff_x = firstTouch.position.x - secondTouch.position.x;
            float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;


            //Al tocar por primera vez con los dos dedos se guarda la distancia inicial que hay entre ellos y su ángulo
            if (this.firstTimeFingersTouchScreen)
            {
                this.initialDistance = currentTouchDistance;
                this.initialAngle = currentTouchAngle;
                this.firstTimeFingersTouchScreen = false;
            }


            //Se calcula el ángulo delta
            float angleDelta = currentTouchAngle - this.initialAngle;

            //Se calcula el multiplicador para escalar el objeto con la distancia inicial y actual de los dedos
            float scaleMultiplier = (currentTouchDistance / this.initialDistance);
            float scaleAmount = this.furnitureScale * scaleMultiplier;
            float scaleAmountClamped = Mathf.Clamp(scaleAmount, ScaleRangeMin, ScaleRangeMax);

            //Se realiza la rotación con el ángulo delta
            this.furnitureTransform.localEulerAngles = this.furnitureEulerAngles - new Vector3(0, angleDelta * 3f, 0);

            //Se escala el modelo si está activa la opción
            if (this.enableScale)
            {
                this.furnitureTransform.localScale = new Vector3(scaleAmountClamped, scaleAmountClamped, scaleAmountClamped);
            }

        }
        else if (Input.touchCount < 2)
        {
            SetScaleRotation();
            this.firstTimeFingersTouchScreen = true;
        }
    }

    public void SetScaleRotation()
    {
        this.furnitureScale = this.furnitureTransform.localScale.x;
        this.furnitureEulerAngles = this.furnitureTransform.localEulerAngles;
    }
    public void TouchFurniture(Transform furniture)
    {
        this.furnitureTransform = furniture;
    }

    public void EnableScale()
    {
        if (!enableScale)
        {
            this.enableScale = true;
        }
        else
        {
            this.enableScale = false;
        }
        
    }



    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    static bool IsSingleFingerDown()
    {
        if (Input.touchCount == 0 || Input.touchCount >= 2)
            lastTouchCount = Input.touchCount;

        return (
            Input.touchCount == 1 &&
            Input.GetTouch(0).fingerId == 0 &&
            lastTouchCount == 0);
    }

    #endregion // PRIVATE_METHODS

}
