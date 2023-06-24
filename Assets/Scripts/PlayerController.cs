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
    private bool InDonorPoint;

    private CarController nearCar;
    private CarController controlledCar;
    
    // Start is called before the first frame update
    void Awake()
    {
        InputActions = new InputActions();
        InputActions.Enable();
        InputActions.Car.Disable();
        InputActions.Menu.Disable();
        InputActions.Character.Sprint.started += OnSprintPressed;
        InputActions.Character.Sprint.canceled += OnSprintCanceled;
        InputActions.Character.EnterCar.performed += OnEnterCar;
        InputActions.Car.ExitCar.performed += OnExitCar;
        InputActions.Character.Interact.performed += OnInteract;
    }

    private void OnExitCar(InputAction.CallbackContext obj)
    {
        if (InCar)
        {
            ExitCar();
        }
    }

    private void OnInteract(InputAction.CallbackContext obj)
    {
        if (InDonorPoint)
        {
            GameManager.Instance.TransferBlood(10);
        }
    }

    private void OnEnterCar(InputAction.CallbackContext obj)
    {
        if (NearEnterableCar)
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
        InputActions.Character.Disable();
        InputActions.Car.Enable();
        controlledCar = nearCar;
        controlledCar.Posses(InputActions);
        ToggleShowPlayer(false);
        GameManager.Instance.SwitchCameraFollow(controlledCar.transform);
    }

    void ExitCar()
    {
        InCar = false;
        InputActions.Car.Disable();
        InputActions.Character.Enable();
        controlledCar.UnPosses();
        TeleportPlayer(controlledCar.ExitTransform);
        ToggleShowPlayer(true);
        controlledCar = null;
        GameManager.Instance.SwitchCameraFollow(transform);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<CarController>())
        {
            NearEnterableCar = true;
            nearCar = other.GetComponentInParent<CarController>();
        }
        else if (!other.GetComponent<BloodDonor>())
        {
            InDonorPoint = true;
        }

        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.GetComponentInParent<CarController>())
        {
            NearEnterableCar = false;
            nearCar = null;
        }
        else if (!other.GetComponent<BloodDonor>())
        {
            InDonorPoint = false;
        }
    }

    void ToggleShowPlayer(bool show)
    {
        GetComponent<Collider2D>().enabled = show;
        GetComponentInChildren<SpriteRenderer>().enabled = show;
    }

    void TeleportPlayer(Transform teleportTransform)
    {
        rigidbody2D.position = teleportTransform.position;
        rigidbody2D.SetRotation(teleportTransform.rotation);
    }
}
