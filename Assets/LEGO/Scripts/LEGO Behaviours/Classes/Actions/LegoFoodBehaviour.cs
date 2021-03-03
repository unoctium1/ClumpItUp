using LEGOModelImporter;
using System.Collections;
using System.Collections.Generic;
using Katamari;
using Unity.LEGO.Behaviours.Triggers;
using UnityEngine;

namespace Unity.LEGO.Behaviours.Actions
{
    public class LegoFoodBehaviour : Action
    {
        [SerializeField, Range(0f, 15f)] private float m_Size;

        bool m_Initialised;
        bool m_Collected;

        List<LEGOBehaviour> m_Behaviours = new List<LEGOBehaviour>();
        List<Collider> m_Colliders = new List<Collider>();
        protected HashSet<SensoryCollider> m_ActiveColliders = new HashSet<SensoryCollider>();

        protected override void Start()
        {
            base.Start();

            if (IsPlacedOnBrick())
            {

                // Add SensoryCollider to all brick colliders.
                foreach (var brick in m_ScopedBricks)
                {
                    foreach (var part in brick.parts)
                    {
                        foreach (var collider in part.colliders)
                        {
                            m_Colliders.Add(collider);
                            var sensoryCollider = LEGOBehaviourCollider.Add<SensoryCollider>(collider, m_ScopedBricks, 0.64f);
                            SetupSensoryCollider(sensoryCollider);

                            // Make the original collider a trigger.
                            //collider.isTrigger = true;
                        }
                    }
                }
                // Disconnect from all bricks not in scope.
                foreach (var brick in m_ScopedBricks)
                {
                    brick.DisconnectInverse(m_ScopedBricks);
                }

                // Make invisible.
                foreach (var partRenderer in m_scopedPartRenderers)
                {
                    partRenderer.enabled = false;
                }

                // Find all LEGOBehaviours in scope.
                foreach (var brick in m_ScopedBricks)
                {
                    m_Behaviours.AddRange(brick.GetComponentsInChildren<LEGOBehaviour>());
                }
            }
        }

        protected void Update()
        {
            if (m_Active)
            {
                if (!m_Initialised)
                {
                    // Make visible.
                    foreach (var partRenderer in m_scopedPartRenderers)
                    {
                        partRenderer.enabled = true;
                    }

                    m_Initialised = true;
                }

                if (!m_Collected)
                {
                    // Check if picked up.
                    if (m_ActiveColliders.Count > 0)
                    {

                        foreach (var collider in m_Colliders)
                        {
                            collider.isTrigger = true;
                        }

                        PlayAudio(spatial: false, destroyWithAction: false);

                        // Delay destruction of LEGOBehaviours one frame to allow multiple Pickup Actions to be collected.
                        m_Collected = true;

                    }
                }
                else
                {
                    BrickExploder.SeperateConnectedBricks(m_Brick, out var bricks, 0.5f);
                    Player.Instance.AddSize(m_Size, bricks);
                }
            }
        }

        protected void SetupSensoryCollider(SensoryCollider collider)
        {
            collider.OnSensorActivated += SensoryColliderActivated;
            collider.OnSensorDeactivated += SensoryColliderDeactivated;

            collider.Sense = SensoryTrigger.Sense.Player;
        }

        void SensoryColliderActivated(SensoryCollider collider, Collider other)
        {
            //Debug.Log($"This size: {m_Size}, player pickup size: {Player.Instance.MinPickupSize}");
            if (Player.Instance.MinPickupSize > m_Size)
            {
                m_ActiveColliders.Add(collider);
            }
        }

        void SensoryColliderDeactivated(SensoryCollider collider)
        {
            m_ActiveColliders.Remove(collider);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Set original collider back to non-trigger if initialised and not collected.
            if (m_Initialised && !m_Collected)
            {
                foreach (var brick in m_ScopedBricks)
                {
                    foreach (var part in brick.parts)
                    {
                        foreach (var collider in part.colliders)
                        {
                            if (collider)
                            {
                                collider.isTrigger = false;
                            }
                        }
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Vector3 center = Vector3.zero;
            int count = 0;
            foreach (var brick in GetScopedBricks())
            {

                count++;
                center += brick.transform.position;
                //Debug.Log($"Count: {count}, Center: {center}");
            }

            center /= count;
            center += transform.TransformVector(m_BrickPivotOffset);
            Gizmos.DrawWireSphere(center, m_Size * 1.5f);

        }
    }
}
