using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidbody2D;
    
    [SerializeField]
    private float SpeedModifer;
    
    [SerializeField]
    private float RunModifer;
    
    private InputActions InputActions;

    private bool run;
    private bool NearEnterableCar;
    private bool InCar;
    
    // Start is called before the first frame update
    void Awake()
    {
        InputActions = new InputActions();
        InputActions.Enable();
        InputActions.Character.Sprint.started += OnSprintPressed;
        InputActions.Character.Sprint.canceled += OnSprintCanceled;
        InputActions.Character.EnterCar.performed += OnEnterCar;
    }

    private void OnEnterCar(InputAction.CallbackContext obj)
    {
        if (InCar)
        {
            ExitCar();
        }
        else if (NearEnterableCar)
        {
            EnterCar();
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Sprint stop!");
        run = false;
    }

    private void OnSprintPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Sprint start!");
        run = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move(InputActions.Character.Walk.ReadValue<Vector2>());

        Vector2 mousePos = InputActions.Character.LookMousePos.ReadValue<Vector2>();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(
            mousePos.x, 
            mousePos.y,
            Camera.main.nearClipPlane + 1));
        

        RotateCharacter(mouseWorld);

    }

    private void Move(Vector2 input)
    {
        Vector2 translation = input * SpeedModifer * Time.fixedDeltaTime;
        if (run)
        {
            translation *= RunModifer;
        }
        rigidbody2D.MovePosition(rigidbody2D.position += translation);
        
    }

    private void RotateCharacter(Vector2 lookat)
    {
        // Debug.LogFormat("Look at {0}", lookat.ToString());

        Vector2 relativeVector = rigidbody2D.position - lookat;
        
        relativeVector.Normalize();

        float desiredAngle = Mathf.Atan2(relativeVector.y, relativeVector.x);
        desiredAngle *= Mathf.Rad2Deg;
        desiredAngle -= 90;
        
        rigidbody2D.SetRotation(desiredAngle);
    }

    void EnterCar()
    {
        InCar = true;
    }

    void ExitCar()
    {
        InCar = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponentInParent<CarController>())
        {
            return;
        }

        NearEnterableCar = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.GetComponentInParent<CarController>())
        {
            return;
        }

        NearEnterableCar = false;
    }
}
