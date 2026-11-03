using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Hero : MonoBehaviour
{
   
    static public Hero S { get; private set; }

    [Header("Inscribed")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    [Header("Dynamic")] [Range(0, 4)]
    public float shieldLevel = 1;

    InputAction moveAction;

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
    }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // Get the value of the move action
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += moveInput.x * speed * Time.deltaTime;
        pos.y += moveInput.y * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(moveInput.y * pitchMult, moveInput.x * rollMult, 0); 
    }
}
