using Unity.LEGO.Behaviours.Actions;
using UnityEditor;
using UnityEngine;

namespace Unity.LEGO.EditorExt 
{
    [CustomEditor(typeof(LegoFoodBehaviour), true)]
    public class LegoFoodEditor : ActionEditor
    {
        LegoFoodBehaviour m_FoodAction;

        SerializedProperty m_SizeProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_FoodAction = (LegoFoodBehaviour)m_Action;

            m_SizeProp = serializedObject.FindProperty("m_Size");
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
                    //DrawConnections(m_FoodAction, m_DependentTriggers, true, Color.green);
                }
            }
        }
    }
}
