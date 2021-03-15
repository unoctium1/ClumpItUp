using LEGOModelImporter;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Katamari
{
    public class Player : MonoBehaviour
    {
        public static Player Instance = null;

        [SerializeField] RollingBall sphere;
        [SerializeField] Transform legoBlockContainer;
        [SerializeField] Cinemachine.CinemachineFreeLook cam;
        [SerializeField, Range(0f, 30f)] float absorptionSpeed = 15f;
        [SerializeField, Range(0f, 1f)] float growthSpeed;
        [SerializeField] float scale = 1f;
        float eatenSize = 0f;
        float oldScale;
        Vector3 relativeScale;
        float relativeScaleFactor;
        const int maxBricks = 40; //increase this on PC, decrease on web

        List<LegoInPlayerBall> bricksEaten = new List<LegoInPlayerBall>();

        readonly int amplitudeID = Shader.PropertyToID("_Amplitude");
        readonly int highlightID = Shader.PropertyToID("_Highlight");


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

            UpdateSize(size);//, bricksEaten[bricksEaten.Count-1].BrickColor);
        }


        public float MinPickupSize { get => scale * 0.75f; }
        public float Scale { get => scale * relativeScaleFactor; }

        private void Start()
        {
            relativeScale = sphere.transform.localScale;
            relativeScaleFactor = relativeScale.x;
            oldScale = scale;
            eatenSize = scale;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 position = sphere.transform.position;

            legoBlockContainer.position = position;

            this.transform.position = position;

            if (scale != oldScale)
            {
                oldScale = scale;

                Vector3 scaleVec = scale * relativeScale;
                cam.transform.localScale = scaleVec;
                sphere.transform.localScale = scaleVec;
                this.transform.localScale = Vector3.one * scale;
            }
            
            if(bricksEaten.Count > maxBricks)
            {
                Debug.Log("Clearing bricks");
                int bricksToDestroy = bricksEaten.Count - maxBricks;
                LegoInPlayerBall[] toDestroy = new LegoInPlayerBall[bricksToDestroy];
                for(int i = 0; i < bricksToDestroy; i++)
                {
                    toDestroy[i] = bricksEaten[i];
                }
                bricksEaten.RemoveRange(0, bricksToDestroy);
                foreach(var brick in toDestroy)
                {
                    Destroy(brick);
                }
                

            }
        }

        private void FixedUpdate()
        {
            if (bricksEaten.Count > 5) // turn off realistic physics once we have more than a few bricks
            {
                Vector3 force = sphere.Velocity * -0.5f;
                foreach (var brick in bricksEaten)
                {

                    brick.UpdateForce(force);
                }
            }
        }

        private void UpdateSize(float sizeToAdd)//, Color brickColor)
        {
            eatenSize += sizeToAdd;
            float endValue = Mathf.Pow(eatenSize, 1f / 3f) * 1.5f;
            sphere.SphereMat.SetFloat(amplitudeID, sizeToAdd);
            var sequence = DOTween.Sequence();
            sequence
                .Append(DOTween.To(() => scale, x => scale = x, endValue, growthSpeed))
                .Join(sphere.SphereMat.DOFloat(0.05f, amplitudeID, growthSpeed));
                //.Join(sphere.SphereMat.DOColor(brickColor, highlightID, growthSpeed));
            sequence.Play();
            sphere.UpdateSize(endValue);
        }
    }
}
