using System.Collections.Generic;
using Unity.LEGO.Behaviours.Actions;
using Unity.LEGO.Behaviours.Triggers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Unity.LEGO.EditorExt 
{
    [CustomEditor(typeof(FoodAction), true)]
    public class FoodActionEditor : ActionEditor
    {
        FoodAction m_FoodAction;

        SerializedProperty m_SizeProp;

        List<Trigger> m_DependentTriggers = new List<Trigger>();

        protected override void OnEnable()
        {
            base.OnEnable();

            m_FoodAction = (FoodAction)m_Action;

            m_SizeProp = serializedObject.FindProperty("m_Size");

            // Collect Pickup Triggers that depend on this Pickup Action.
            m_DependentTriggers.Clear();

            var pickupTriggers = StageUtility.GetCurrentStageHandle().FindComponentsOfType<FoodTrigger>();

            foreach (var trigger in pickupTriggers)
            {
                if (trigger.GetMode() == FoodTrigger.Mode.SpecificPickups)
                {
                    var specificPickups = trigger.GetSpecificPickupActions();
                    if (specificPickups.Contains(m_FoodAction))
                    {
                        m_DependentTriggers.Add(trigger);
                    }
                }
            }
        }

        protected override void CreateGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.PropertyField(m_ScopeProp);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(m_AudioProp);
            EditorGUILayout.PropertyField(m_AudioVolumeProp);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.PropertyField(m_SizeProp);

            EditorGUI.EndDisabledGroup();
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            if (Event.current.type == EventType.Repaint)
            {
                if (m_FoodAction)
                {
                    DrawConnections(m_FoodAction, m_DependentTriggers, true, Color.green);
                }
            }
        }
    }
}
