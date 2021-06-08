﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TouchController : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    public Transform augmentationObject;

    public static bool IsSingleFingerStationary => IsSingleFingerDown() && (Input.GetTouch(0).phase == TouchPhase.Stationary);

    public static bool IsSingleFingerDragging => IsSingleFingerDown() && (Input.GetTouch(0).phase == TouchPhase.Moved);

    #endregion // PUBLIC MEMBERS


    #region PRIVATE_MEMBERS
    const float ScaleRangeMin = 0.1f;
    const float ScaleRangeMax = 2.0f;

    Touch[] touches;
    static int lastTouchCount;
    bool isFirstFrameWithTwoTouches;
    bool activeUI;
    bool enablePinchScaling;
    float cachedTouchAngle;
    float cachedTouchDistance;
    float cachedAugmentationScale;
    Vector3 cachedAugmentationRotation;

    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        this.cachedAugmentationScale = this.augmentationObject.localScale.x;
        this.cachedAugmentationRotation = this.augmentationObject.localEulerAngles;
    }

    void Update()
    {
        //Rotate
        if (!activeUI)
        {
            if (Input.touchCount == 2)
            {
                var firstTouch = Input.GetTouch(0);
                var secondTouch = Input.GetTouch(1);

                float currentTouchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
                float diff_y = firstTouch.position.y - secondTouch.position.y;
                float diff_x = firstTouch.position.x - secondTouch.position.x;
                float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;

                if (this.isFirstFrameWithTwoTouches)
                {
                    this.cachedTouchDistance = currentTouchDistance;
                    this.cachedTouchAngle = currentTouchAngle;
                    this.isFirstFrameWithTwoTouches = false;
                }

                float angleDelta = currentTouchAngle - this.cachedTouchAngle;
                float scaleMultiplier = (currentTouchDistance / this.cachedTouchDistance);
                float scaleAmount = this.cachedAugmentationScale * scaleMultiplier;
                float scaleAmountClamped = Mathf.Clamp(scaleAmount, ScaleRangeMin, ScaleRangeMax);

                this.augmentationObject.localEulerAngles = this.cachedAugmentationRotation - new Vector3(0, angleDelta * 3f, 0);

                if (this.enablePinchScaling)
                {
                    this.augmentationObject.localScale = new Vector3(scaleAmountClamped, scaleAmountClamped, scaleAmountClamped);
                }

            }
            else if (Input.touchCount < 2)
            {
                this.cachedAugmentationScale = this.augmentationObject.localScale.x;
                this.cachedAugmentationRotation = this.augmentationObject.localEulerAngles;
                this.isFirstFrameWithTwoTouches = true;
            }
        }
        
        

    }


    #endregion // MONOBEHAVIOUR_METHODS

    #region  PUBLIC_METHODS 
    public void TouchFurniture(Transform furniture)
    {
        this.augmentationObject = furniture;
    }

    public void EnableScale()
    {
        if (!enablePinchScaling)
        {
            this.enablePinchScaling = true;
        }
        else
        {
            this.enablePinchScaling = false;
        }
        
    }

    public void SetActiveUI(bool var)
    {
        this.activeUI = var;
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