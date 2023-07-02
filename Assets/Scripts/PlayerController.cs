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

    public float BloodTransfusionTotalTime;
    private float BloodTransfusionTimer;
    
    private InputActions InputActions;

    private bool run;
    private bool NearEnterableCar;
    private bool InCar;
    private bool InDonorPoint;
    private bool isGivingBlood;

    private TopDownCarController nearCar;
    private TopDownCarController controlledCar;
    private Unlock nearestUnlock;
    
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
        InputActions.Character.Interact.started += InteractOnstarted;
        InputActions.Character.Interact.canceled += InteractOncanceled;
    }

    private void Update()
    {
        if (isGivingBlood)
        {
            BloodTransfusionTimer -= Time.deltaTime;
            if (BloodTransfusionTimer <= 0.0f)
            {
                GameManager.Instance.TransferBlood(10);
                BloodTransfusionTimer = BloodTransfusionTotalTime;
            }
        }
    }

    private void InteractOncanceled(InputAction.CallbackContext obj)
    {
        if (InDonorPoint)
        {
            isGivingBlood = false;
            BloodTransfusionTimer = BloodTransfusionTotalTime;
        }
    }

    private void InteractOnstarted(InputAction.CallbackContext obj)
    {
        if (InDonorPoint)
        {
            isGivingBlood = true;
            GameManager.Instance.TransferBlood(10);
            BloodTransfusionTimer = BloodTransfusionTotalTime;
        }
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
        if (nearestUnlock != null)
        {
            if (nearestUnlock.HasEnoughBlood(GameManager.Instance.BloodOnCharacter))
            {
                GameManager.Instance.PayWithBlood(nearestUnlock.Price);
                nearestUnlock.UnlockColliders();
            }
            else
            {
                Debug.Log("Not enough blood!!!");
            }
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
        if (other.GetComponentInParent<TopDownCarController>())
        {
            NearEnterableCar = true;
            nearCar = other.GetComponentInParent<TopDownCarController>();
        }
        else if (other.GetComponent<BloodDonor>())
        {
            InDonorPoint = true;
        }
        else if (other.GetComponent<Unlock>())
        {
            nearestUnlock = other.GetComponent<Unlock>();
        }

        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<TopDownCarController>())
        {
            NearEnterableCar = false;
            nearCar = null;
        }
        else if (other.GetComponent<BloodDonor>())
        {
            InDonorPoint = false;
            isGivingBlood = false;
            BloodTransfusionTimer = BloodTransfusionTotalTime;
        }
        else if (other.GetComponent<Unlock>())
        {
            nearestUnlock = null;
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
