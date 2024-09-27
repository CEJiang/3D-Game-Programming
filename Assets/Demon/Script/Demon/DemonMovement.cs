using System;
using UnityEngine;

public class DemonMovement : MonoBehaviour
{
    public static float MPlayerSpeed {get; set;}
    public static float MPlayerJumpForce {get; set;}
    public static float MMouseSensitivity {get; set;}
    private Transform mCameraTransform;
    private Rigidbody mRb;
    private float mRotationX = 0.0f;
    private float mRotationY = 0.0f;
    private bool isGrounded = true;

    Animator MAnimator;

    void Start() {
        MPlayerSpeed = 7.0f;  // 調整速度
        MPlayerJumpForce = 5.0f;
        MMouseSensitivity = 750.0f;

        Cursor.lockState = CursorLockMode.Locked;
        mCameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform;
        mRb = gameObject.GetComponent<Rigidbody>();
        MAnimator = gameObject.GetComponent<Animator>();
        
    }

    void Update() {
        AnimatorStateInfo mStateInfo = MAnimator.GetCurrentAnimatorStateInfo(0);
        float mMoveHorizontal = Input.GetAxis("Horizontal");
        float mMoveVertical = Input.GetAxis("Vertical");
        float mMoveMouseX = Input.GetAxis("Mouse X") * MMouseSensitivity * Time.deltaTime;
        float mMoveMouseY = Input.GetAxis("Mouse Y") * MMouseSensitivity * Time.deltaTime;

        Vector3 mRight = transform.right;
        Vector3 mForward = transform.forward;
        Vector3 mMovement = (mMoveHorizontal * mRight + mMoveVertical * mForward).normalized;
        mRb.MovePosition(transform.position + MPlayerSpeed * Time.deltaTime * mMovement);

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            MAnimator.SetBool("isShift", true);
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift)) {
            MAnimator.SetBool("isShift", false);
        }
        if(mStateInfo.IsName("Stand To Roll")) {
            mRb.MovePosition(transform.position + 10.0f * Time.deltaTime * mForward.normalized);
            MAnimator.speed = 1.3f;
        }
        else {
            MAnimator.speed = 1.0f;
        }
        float speed = new Vector3(mMoveHorizontal, 0, mMoveVertical).magnitude;

        MAnimator.SetFloat("speed", speed);

        if (Input.GetKey(KeyCode.W)) {
            MAnimator.SetBool("isWalking", true);
        }
        else {
            MAnimator.SetBool("isWalking", false);
        }
        if (Input.GetKey(KeyCode.S)) {
            MAnimator.SetBool("isWalkingBackward", true);
        }
        else {
            MAnimator.SetBool("isWalkingBackward", false);
        }
        if(Input.GetMouseButton(0)) {
            MAnimator.SetBool("isAttack", true);
        }
        else {
            MAnimator.SetBool("isAttack", false);
        }

        mRotationX -= mMoveMouseY;
        mRotationY += mMoveMouseX;
        mRotationX = Mathf.Clamp(mRotationX, -90.0f, 90.0f);
        // camera rotate
        mCameraTransform.localRotation = Quaternion.Euler(mRotationX, 0.0f, 0.0f);

        // gameobject rotate
        transform.localRotation = Quaternion.Euler(0.0f, mRotationY, 0.0f);

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            mRb.AddForce(MPlayerJumpForce * Vector3.up, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
            MAnimator.SetBool("isGrounded", true);
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
            MAnimator.SetBool("isGrounded", false);
        }
    }
}
