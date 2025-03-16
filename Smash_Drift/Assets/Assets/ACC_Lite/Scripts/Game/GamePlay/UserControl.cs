using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For user multiplatform control.
/// </summary>
[RequireComponent (typeof (CarController))]
public class UserControl :MonoBehaviour
{

    CarController ControlledCar;
    private ObstGenerator levelGenerator;
    public GameObject tutorialUI;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool Brake { get; private set; }
    
    private Vector2 touchStartPos;
    private bool isTouching = false;
    private float lastHorizontalInput = 0f; // Store last swipe value

    public bool canMove = true;

    
    private void Awake()
    {
        ControlledCar = GetComponent<CarController>();
        levelGenerator = FindObjectOfType<ObstGenerator>();
    }
    

    void Update()
    {
        // Standard input (Keyboard or gamepad)
        if (!isTouching) // Only use keyboard input if there's no touch
        {
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
        }

        Brake = Input.GetButton("Jump");

        // Handle Touch Input
        HandleTouchInput();

        // Apply control to the car
        ControlledCar.UpdateControls(Horizontal, Vertical, Brake);
        
    }
    

    private void HandleTouchInput()
    {
        if (!canMove)
        {
            Vertical = 0;
            Horizontal = 0;
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isTouching = true;
                    if (tutorialUI.activeSelf)
                        tutorialUI.SetActive(false);
                    touchStartPos = touch.position;
                    Vertical = 1; // Keep accelerating while touching
                    break;

                case TouchPhase.Moved:
                    float swipeDeltaX = touch.position.x - touchStartPos.x;
                    lastHorizontalInput = Mathf.Clamp(swipeDeltaX / Screen.width * 2f, -1f, 1f);
                    Horizontal = lastHorizontalInput;
                    break;

                case TouchPhase.Stationary:
                    // Keep last swipe value and continue accelerating
                    Horizontal = lastHorizontalInput;
                    Vertical = 1;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isTouching = false;
                    Horizontal = 0;
                    Vertical = 0; // Stop acceleration when finger is lifted
                    lastHorizontalInput = 0; // Reset stored input
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            levelGenerator.GameOver();
        }
        
        if (other.CompareTag("Finish"))
        {
            levelGenerator.LevelFinish();
            Debug.Log("Finished");
        }
    }

    public void RestartCarSpeed()
    {
        ControlledCar.ResetCarSpeed();
    }
}
