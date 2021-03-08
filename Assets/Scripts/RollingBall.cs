﻿using UnityEngine;

namespace Katamari
{
    public class RollingBall : MonoBehaviour
    {
        [SerializeField] private float initialSpeed = 1f;

        private Rigidbody rb;
        private new Renderer renderer;

        private float movementX;
        private float movementY;

        private float speed;

        private Camera cam;
        [SerializeField] private bool cameraRelativeMovement = true;

        public Vector3 Velocity => rb.velocity;

        public Material SphereMat => renderer.material;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            cam = Camera.main;
            renderer = GetComponent<Renderer>();
            speed = initialSpeed;
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

        public void UpdateSize(float size)
        {
            rb.mass = size;
            speed = initialSpeed * size;
        }
    }
}
