using UnityEngine;

namespace Katamari
{
    public class RollingBall : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;

        private Rigidbody rb;

        private float movementX;
        private float movementY;

        private Camera cam;
        [SerializeField] private bool cameraRelativeMovement = true;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            cam = Camera.main;

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Vector3 right = cameraRelativeMovement ? cam.transform.right : Vector3.right;
            Vector3 forward = cameraRelativeMovement ? cam.transform.forward : Vector3.forward;

            Vector3 targetDirection = right * Input.GetAxisRaw("Horizontal");
            targetDirection += forward * Input.GetAxisRaw("Vertical");

            rb.AddForce(targetDirection * speed);
        }
    }
}
