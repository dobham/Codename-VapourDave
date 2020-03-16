using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpSpeed = 10.0f;

    public float gravity = 9.8f;
    public float interpolationSmoothness = 0.98f;

    [SerializeField]
    float mouseSpeed;

    Vector3 camRot = Vector3.zero;

    float mouseX;
    float mouseY;

    public GameObject cameraObject;

    Inputactions inputAction;
    Vector2 moveInput;
    Vector2 lookInput;

    Rigidbody rb;
    public Vector3 currentNormal;

    bool grounded;

    private void Awake()
    {
        inputAction = new Inputactions();
        inputAction.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputAction.Player.Look.canceled += ctx => lookInput = ctx.ReadValue<Vector2>();

        inputAction.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputAction.Player.Move.canceled += ctx => moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraObject = GetComponentInChildren<Camera>().gameObject;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.75f)) {
            grounded = true;
        }

        //transform.LookAt(transform.position+Vector3.Cross(transform.right,hit.normal), hit.normal);
        if (transform.up != hit.normal && grounded) {

            Quaternion lookRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal);
            float magnitude = Quaternion.Angle(Quaternion.Euler(Vector3.Cross(transform.right, hit.normal)), Quaternion.Euler(hit.normal)) / 180.0f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, interpolationSmoothness * magnitude);
            currentNormal = hit.normal;
        }

        transform.position += transform.forward * speed * moveInput.y * Time.deltaTime;
        transform.position += transform.right * speed * moveInput.x * Time.deltaTime;

        rb.AddForce(-transform.up * gravity * rb.mass * Time.deltaTime * 10);
    }

    void Update()
    {
        mouseY = lookInput.y * mouseSpeed * Time.deltaTime;
        mouseX = lookInput.x * mouseSpeed * Time.deltaTime;

        camRot.x -= mouseY;
        camRot.x = Mathf.Clamp(camRot.x, -90, 90);
        cameraObject.transform.localEulerAngles = camRot; //Set directly soas not to interfere with locking

        transform.Rotate(0, mouseX, 0, Space.Self);

        
    }
}
