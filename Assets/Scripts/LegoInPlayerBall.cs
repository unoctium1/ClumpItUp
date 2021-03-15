using System.Collections.Generic;
using UnityEngine;
using LEGOModelImporter;
using DG.Tweening;

namespace Katamari
{
    public class LegoInPlayerBall : MonoBehaviour
    {
        List<Collider> m_Colliders = new List<Collider>();
        Collider col;
        Rigidbody rb;
        Renderer r;
        Brick brick;
        //Transform parent; // Since we can't scale lego, use this to essentially parent it

        const int Layer = 14;

        public Color BrickColor => r.sharedMaterial.color;

        //protected new Scope m_Scope = Scope.Brick;

        public void Initialize(Transform parent, Vector3 position, Brick brick, float speed)
        {
            //Debug.Log($"Children: {transform.childCount}, parent: {transform.parent}");
            transform.parent = parent;
            this.brick = brick;
            rb = GetComponent<Rigidbody>();
            //r = GetComponent<Renderer>();
            rb.mass = 0.1f;
            rb.isKinematic = true;
            foreach (var part in brick.parts)
            {
                foreach (var collider in part.colliders)
                {
                    m_Colliders.Add(collider);
                    //Debug.Log(collider.isTrigger);
                }
            }
            float duration = Vector3.Distance(transform.localPosition, position) / speed;
            Vector3 midPoint = (transform.localPosition + position) * 0.5f;
            midPoint.y += 2f;
            Vector3[] path = new Vector3[] { midPoint, position };
            var sequence = DOTween.Sequence();
            sequence
                .Append(transform.DOLocalMove(position, duration).SetEase(Ease.InOutSine))
                .Join(transform.DOLocalRotate(Random.insideUnitSphere * 100f, duration))
                .OnComplete(AfterMove);
            sequence.Play();

        }

        private void AfterMove()
        {
            rb.angularVelocity = Vector3.zero;
            this.gameObject.layer = Layer;
            foreach (var col in m_Colliders)
            {
                col.gameObject.layer = Layer;
                col.isTrigger = false;
            }
            rb.isKinematic = false;
        }

        public void UpdateForce(Vector3 force)
        {
            rb.AddForce(force);
        }


        protected void Update()
        {

            if (Vector3.Distance(transform.position, Player.Instance.transform.position) > Player.Instance.Scale)
            {
                Vector3 position = transform.parent.position;
                this.transform.position = position;
                rb.velocity = Vector3.zero;
                rb.Sleep();
            }
        }

        private void OnDestroy()
        {
            Destroy(this.gameObject);
        }
    }
}
