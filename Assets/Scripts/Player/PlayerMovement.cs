﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class PlayerMovement : MovementComponent
{
    #region Fields

    private InputHandler inputHandler;
    private int activeInputsCount = 0;

    #endregion

    #region Properties
    
    #endregion

    override protected void Start()
    {
        base.Start();

        inputHandler = GetComponent<InputHandler>();
        inputHandler.OnInputChanged += InputChanged;
    }

    private void OnDestroy()
    {
        inputHandler.OnInputChanged -= InputChanged;
    }

    private void InputChanged(Vector2 direction, bool isActive)
    {
        if (isActive)
        {
            activeInputsCount++;
            MoveOnDirection(CurrentDirection + direction);
        }
        else
        {
            activeInputsCount--;

            if (activeInputsCount <= 0)
            {
                activeInputsCount = 0;
                StopMoving();
            }
            else
            {
                MoveOnDirection(CurrentDirection - direction);
            }
        }
    }

}
