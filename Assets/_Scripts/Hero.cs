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

    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    [Header("Dynamic")]
    [Range(0, 4)]
    private float _shieldLevel = 1;
    [Tooltip("This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;
    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // Create a WeaponFireDelegate event named fireEvent.
    public event WeaponFireDelegate fireEvent;

    InputAction moveAction;
    InputAction shootAction;

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
        shootAction = InputSystem.actions.FindAction("Attack");
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

        if (shootAction.IsPressed() && fireEvent != null)
        {
            fireEvent();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //Debug.Log("Shield trigger hit by: "+ go.gameObject.name);
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {  // If the shield was triggered by an enemy
            shieldLevel--;        // Decrease the level of the shield by 1
            Destroy(go);          // and Destroy the enemy       
        }
        else
        {
            Debug.LogWarning("Shield trigger hit by non-Enemy: " + go.name);
        }
    }

    public float shieldLevel
    {
        get { return (_shieldLevel); }
        private set
        {
            _shieldLevel = Mathf.Min(value, 4);
            // If the shield is going to be set to less than zero…
            if (value < 0)
            {
                Destroy(this.gameObject);  // Destroy the Hero
                Main.HERO_DIED();
            }
        }
    }

}
