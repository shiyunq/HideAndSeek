using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;
    Vector3 velocity;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;
    bool disabled;
    public event System.Action OnReachedEndOfLevel;

    // Start is called before the first frame update
    void Start()
    {
        Guard.onGuardHasSpottedPlayer += Disable;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = Vector3.zero;
        if (!disabled) move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButton("Jump"))
        {
            Disable();
            transform.localScale = new Vector3(1f, .5f, 1f);
        }

        if (Input.GetButtonUp("Jump"))
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Enable();
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Disable()
    {
        disabled = true;
    }

    private void Enable()
    {
        disabled = false;
    }

    private void OnDestroy()
    {
        Guard.onGuardHasSpottedPlayer -= Disable;
    }

    void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel != null)
                OnReachedEndOfLevel();
        }
    }
}
