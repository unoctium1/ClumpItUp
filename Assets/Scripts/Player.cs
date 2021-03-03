using LEGOModelImporter;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Katamari
{
    public class Player : MonoBehaviour
    {
        public static Player Instance = null;

        [SerializeField] Transform playerSphere;
        [SerializeField] Renderer sphereRenderer;
        [SerializeField] Rigidbody sphereRb;
        [SerializeField] Transform legoBlockContainer;
        [SerializeField] Cinemachine.CinemachineFreeLook cam;
        [SerializeField, Range(0f, 30f)] float absorptionSpeed = 15f;
        [SerializeField, Range(0f, 1f)] float growthSpeed;
        [SerializeField] float scale = 1f;
        float eatenSize = 0f;
        Material sphereMat;
        float oldScale;
        Vector3 relativeScale;
        float relativeScaleFactor;

        List<LegoInPlayerBall> bricksEaten = new List<LegoInPlayerBall>();

        readonly int amplitudeID = Shader.PropertyToID("_Amplitude");


        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public void AddSize(float size, HashSet<Brick> bricks)
        {
            foreach (Brick b in bricks)
            {

                var component = b.gameObject.AddComponent<LegoInPlayerBall>();
                Vector3 localPosition = Random.insideUnitSphere * 0.5f * scale;
                component.Initialize(legoBlockContainer, localPosition, b, absorptionSpeed);
                bricksEaten.Add(component);
            }
            UpdateSize(size);
        }

        public float MinPickupSize { get => scale * 0.5f; }
        public float Scale { get => scale * relativeScaleFactor; }

        private void Start()
        {
            relativeScale = playerSphere.localScale;
            relativeScaleFactor = relativeScale.x;
            oldScale = scale;
            eatenSize = scale;
            sphereMat = sphereRenderer.material;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 position = playerSphere.position;

            legoBlockContainer.position = position;

            this.transform.position = position;

            if (scale != oldScale)
            {
                oldScale = scale;

                Vector3 scaleVec = scale * relativeScale;
                cam.transform.localScale = scaleVec;
                playerSphere.localScale = scaleVec;
                this.transform.localScale = Vector3.one * scale;
            }
        }

        private void FixedUpdate()
        {
            if (bricksEaten.Count > 5) // turn off realistic physics once we have more than a few bricks
            {
                Vector3 force = sphereRb.velocity * -0.5f;
                foreach (var brick in bricksEaten)
                {

                    brick.UpdateForce(force);
                }
            }
        }

        private void UpdateSize(float sizeToAdd)
        {
            eatenSize += sizeToAdd;
            float endValue = Mathf.Pow(eatenSize, 1f / 3f) * 1.5f;
            sphereMat.SetFloat(amplitudeID, sizeToAdd);
            var sequence = DOTween.Sequence();
            sequence
                .Append(DOTween.To(() => scale, x => scale = x, endValue, growthSpeed))
                .Join(sphereMat.DOFloat(0.05f, amplitudeID, growthSpeed));
            sequence.Play();
            sphereRb.mass = endValue;
        }
    }
}
