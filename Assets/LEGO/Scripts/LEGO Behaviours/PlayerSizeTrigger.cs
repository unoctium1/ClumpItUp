using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Katamari;

namespace Unity.LEGO.Behaviours.Triggers
{
    public class PlayerSizeTrigger : Trigger
    {
        public float m_TargetSize = 1f;

        private float m_PreviousProgress;

        protected void OnValidate()
        {
            m_TargetSize = Mathf.Max(1f, m_TargetSize);
        }

        protected override void Start()
        {
            base.Start();

            Goal = (int)(m_TargetSize * 100f);
            Progress = (int)(Player.Instance.Scale * 100f);

        }

        protected void Update()
        {
            Progress = (int)(Player.Instance.Scale * 100f);

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
    }
}
