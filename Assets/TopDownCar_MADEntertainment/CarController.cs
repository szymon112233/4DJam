﻿using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public enum controlMode
{
	KeyBoard,Touch
}

public class CarController : MonoBehaviour
{

	public Transform ExitTransform;
	public Animation animation;
	public Light2D FrontLightL;
	public Light2D FrontLightR;
	public Light2D BackLightL;
	public Light2D BackLightR;

	public controlMode CarControlMode;

	public float MaxSpeed = 7.0f;
	public float MaxSteer = 2.0f;
	public float Breaks = 0.2f;

	[SerializeField]
	float Acceleration = 0.0f;
	float Steer = 0.0f;

	bool AccelFwd, AccelBwd;
	bool TouchAccel,TouchBack,TouchBreaks;
	bool SteerLeft, SteerRight;

	private InputActions InputController;
	private bool possesed;
	

	void Start ()
	{
		FrontLightL.intensity = 0;
		FrontLightR.intensity = 0;
		BackLightL.intensity = 0;
		BackLightR.intensity = 0;
		
	}

	public void Posses(InputActions controller)
	{
		InputController = controller;
		possesed = true;
		InputController.Car.Break.performed += BreakOnperformed;
		InputController.Car.Break.canceled += BreakOncanceled;
		animation.Play("StartEngine");
	}

	private void BreakOncanceled(InputAction.CallbackContext obj)
	{
		animation.Stop();
		BackLightL.intensity = 0;
		BackLightR.intensity = 0;
	}

	private void BreakOnperformed(InputAction.CallbackContext obj)
	{
		animation.Play("Break");
	}

	public void UnPosses()
	{
		InputController.Car.Break.performed -= BreakOnperformed;
		InputController.Car.Break.canceled -= BreakOncanceled;
		InputController = null;
		possesed = false;
		animation.Play("StopEngine");
	}

	void FixedUpdate () 
	{
		if (possesed)
		{
			// if (CarControlMode == controlMode.KeyBoard) 
			// {
			// 	if (InputController.Car.Accelrate.ReadValue<float>() > 0)
			// 		Accel (1);													//Accelerate in forward direction
			// 	else if (InputController.Car.Accelrate.ReadValue<float>() < 0)
			// 		Accel (-1);													//Accelerate in backward direction
			// 	else if (InputController.Car.Break.IsInProgress()) 
			// 	{
			// 		if (AccelFwd)
			// 			StopAccel (1, Breaks);									//Breaks while in forward direction
			// 		else if (AccelBwd)
			// 			StopAccel (-1, Breaks);									//Breaks while in backward direction
			// 	} 
			// 	else 
			// 	{
			// 		if (AccelFwd)
			// 			StopAccel (1, 0.1f);									//Applies breaks slowly if no key is pressed while in forward direction
			// 		else if (AccelBwd)
			// 			StopAccel (-1, 0.1f);									//Applies breaks slowly if no key is pressed while in backward direction
			// 	}
			// }

			if (CarControlMode == controlMode.Touch) 
			{
				if (TouchAccel)
					Accel (1);
				else if(TouchBack)
					Accel (-1);
				else if(TouchBreaks)
				{
					if (AccelFwd)
						StopAccel (1, Breaks);
					else if (AccelBwd)
						StopAccel (-1, Breaks);	
				}
				else
				{
					if (AccelFwd)
						StopAccel (1, 0.1f);
					else if (AccelBwd)
						StopAccel (-1, 0.1f);
				}
			}
		}
	}

	// Functions to be called from Onscreen buttons for touch input.
	public void SetTouchAccel(bool TouchState)
	{
		TouchAccel = TouchState;
	}

	public void SetTouchBack(bool TouchState)
	{
		TouchBack = TouchState;
	}

	public void SetTouchBreaks(bool TouchState)
	{
		TouchBreaks = TouchState;
	}

	public void SetSteerLeft(bool TouchState)
	{
		SteerLeft = TouchState;
	}

	public void SetSteerRight(bool TouchState)
	{
		SteerRight = TouchState;
	}

	public void Accel(int Direction)
	{
		if (Direction == 1) 
		{
			AccelFwd = true;
			if (Acceleration <= MaxSpeed) 
			{
				Acceleration += 0.05f;
			}

			if(CarControlMode == controlMode.KeyBoard)
			{
				if (IsSteeringLeft())
					transform.Rotate (Vector3.forward * Steer);				//Steer left
				if (IsSteeringRight())
					transform.Rotate (Vector3.back * Steer);				//steer right
			}
			else if(CarControlMode == controlMode.Touch)
			{
				if(SteerLeft)
					transform.Rotate (Vector3.forward * Steer);
				else if(SteerRight)
					transform.Rotate (Vector3.back * Steer);
			}
		} 
		else if (Direction == -1) 
		{
			AccelBwd = true;
			if ((-1 * MaxSpeed) <= Acceleration) 
			{
				Acceleration -= 0.05f;
			}

			if(CarControlMode == controlMode.KeyBoard)
			{
				if (IsSteeringLeft())
					transform.Rotate (Vector3.back * Steer);				//Steer left (while in reverse direction)
				if (IsSteeringRight())
					transform.Rotate (Vector3.forward * Steer);				//Steer left (while in reverse direction)
			}
			else if(CarControlMode == controlMode.Touch)
			{
				if(SteerLeft)
					transform.Rotate (Vector3.forward * Steer);
				else if(SteerRight)
					transform.Rotate (Vector3.back * Steer);
			}
		}
			
		if (Steer <= MaxSteer)
			Steer += 0.01f;

		if(CarControlMode == controlMode.Touch)
			transform.Translate (Vector2.up * Acceleration * Time.deltaTime);
		else if(CarControlMode == controlMode.KeyBoard)
			transform.Translate (Vector2.up * Acceleration * Time.deltaTime);
	}

	public void StopAccel(int Direction, float BreakingFactor)
	{
		if (Direction == 1) 
		{
			if (Acceleration >= 0.0f) 
			{
				Acceleration -= BreakingFactor;

				if (CarControlMode == controlMode.KeyBoard) 
				{
					if (IsSteeringLeft())
						transform.Rotate (Vector3.forward * Steer);
					if (IsSteeringRight())
						transform.Rotate (Vector3.back * Steer);
				}
				else if(CarControlMode == controlMode.Touch)
				{
					if(SteerLeft)
						transform.Rotate (Vector3.forward * Steer);
					else if(SteerRight)
						transform.Rotate (Vector3.back * Steer);
				}
			}
			else
				AccelFwd = false;
		} 
		else if (Direction == -1) 
		{
			if(Acceleration <= 0.0f)
			{
				Acceleration += BreakingFactor;

				if (CarControlMode == controlMode.KeyBoard) 
				{
					if (IsSteeringLeft())
						transform.Rotate (Vector3.back * Steer);
					if (IsSteeringRight())
						transform.Rotate (Vector3.forward * Steer);
				}
				else if(CarControlMode == controlMode.Touch)
				{
					if(SteerLeft)
						transform.Rotate (Vector3.forward * Steer);
					else if(SteerRight)
						transform.Rotate (Vector3.back * Steer);
				}
			}
			else
				AccelBwd = false;
		}

		if (Steer >= 0.0f)
			Steer -= 0.01f;

		transform.Translate (Vector2.up * Acceleration * Time.deltaTime);
	}

	private bool IsSteeringLeft()
	{
		// return InputController.Car.Turn.ReadValue<float>() < 0;
		return false;
	}
	
	private bool IsSteeringRight()
	{
		// return InputController.Car.Turn.ReadValue<float>() > 0;
		return false;
	}
}
