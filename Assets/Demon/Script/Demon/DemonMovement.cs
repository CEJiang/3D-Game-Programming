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
        MPlayerSpeed = 5.0f;  // 調整速度
        MPlayerJumpForce = 5.0f;
        MMouseSensitivity = 750.0f;

        Cursor.lockState = CursorLockMode.Locked;
        mCameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform;
        mRb = gameObject.GetComponent<Rigidbody>();
        MAnimator = gameObject.GetComponent<Animator>();
    }

    void Update() {
        float mMoveHorizontal = Input.GetAxis("Horizontal");
        float mMoveVertical = Input.GetAxis("Vertical");
        float mMoveMouseX = Input.GetAxis("Mouse X") * MMouseSensitivity * Time.deltaTime;
        float mMoveMouseY = Input.GetAxis("Mouse Y") * MMouseSensitivity * Time.deltaTime;

        Vector3 mRight = mCameraTransform.right;
        Vector3 mForward = mCameraTransform.forward;
        Vector3 mMovement = (mMoveHorizontal * mRight + mMoveVertical * mForward).normalized;
        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            MAnimator.SetBool("isShift", true);
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift)) {
            MAnimator.SetBool("isShift", false);
        }
        float speed = new Vector3(mMoveHorizontal, 0, mMoveVertical).magnitude;

        MAnimator.SetFloat("speed", speed);
        MAnimator.SetBool("isWalking", speed > 0.0f);

        if (speed > 0.0f) {
            mRb.MovePosition(transform.position + MPlayerSpeed * Time.deltaTime * mMovement);
        }

        mRotationX -= mMoveMouseY;
        mRotationY += mMoveMouseX;
        mRotationX = Mathf.Clamp(mRotationX, -90.0f, 90.0f);
        mCameraTransform.localRotation = Quaternion.Euler(mRotationX, 0.0f, 0.0f);
        transform.localRotation = Quaternion.Euler(0.0f, mRotationY, 0.0f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            mRb.AddForce(MPlayerJumpForce * Vector3.up, ForceMode.Impulse);
            isGrounded = false;
            MAnimator.SetBool("isGround", false);
        }
        
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
            MAnimator.SetBool("isGround", true);
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
            MAnimator.SetBool("isGround", false);
        }
    }
}
