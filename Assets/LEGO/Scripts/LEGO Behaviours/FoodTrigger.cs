using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.LEGO.Behaviours.Actions;
using UnityEngine;

namespace Unity.LEGO.Behaviours.Triggers
{
    public class FoodTrigger : Trigger
    {
        public enum Mode
        {
            AllPickups,
            AmountOfPickups,
            SpecificPickups
        }

        [SerializeField, Tooltip("Trigger when all pickups are collected.\nor\nTrigger when an amount of pickups are collected.\nor\nTrigger when specific pickups are collected.")]
        Mode m_Mode = Mode.AllPickups;

        [SerializeField, Tooltip("The amount of pickups to collect.")]
        int m_AmountModeCount = 1;

        [SerializeField, Tooltip("The list of pickups to collect.")]
        List<FoodAction> m_SpecificModePickupActions = new List<FoodAction>();

        List<FoodAction> m_PickupActions = new List<FoodAction>();
        int m_PreviousProgress;

        public Mode GetMode()
        {
            return m_Mode;
        }

        public ReadOnlyCollection<FoodAction> GetSpecificPickupActions()
        {
            return m_SpecificModePickupActions.AsReadOnly();
        }

        protected void OnValidate()
        {
            m_AmountModeCount = Mathf.Max(1, m_AmountModeCount);
        }

        protected override void Start()
        {
            base.Start();

            if (IsPlacedOnBrick())
            {
                // Find relevant Pickup Actions.
                if (m_Mode == Mode.SpecificPickups)
                {
                    m_PickupActions.AddRange(m_SpecificModePickupActions);
                }
                else
                {
                    m_PickupActions.AddRange(FindObjectsOfType<FoodAction>());
                }

                // Set up listener and count number of valid Pickup Actions.
                var validPickupActions = 0;
                foreach (var pickupAction in m_PickupActions)
                {
                    if (pickupAction)
                    {
                        pickupAction.OnCollected += PickupCollected;
                        validPickupActions++;
                    }
                }

                // Register amount of pickups left to collect.
                if (m_Mode == Mode.AmountOfPickups)
                {
                    Goal = m_AmountModeCount;
                }
                else
                {
                    Goal = validPickupActions;
                }
            }
        }

        void Update()
        {
            if (m_PreviousProgress != Progress)
            {
                if (Progress < Goal)
                {
                    OnProgress?.Invoke();
                }

                m_PreviousProgress = Progress;
            }

            if (Progress >= Goal)
            {
                ConditionMet();
            }
        }

        void PickupCollected(FoodAction pickup)
        {
            Progress++;
            pickup.OnCollected -= PickupCollected;
        }

        void OnDestroy()
        {
            foreach (var pickup in m_PickupActions)
            {
                if (pickup)
                {
                    pickup.OnCollected -= PickupCollected;
                }
            }
        }
    }
}
