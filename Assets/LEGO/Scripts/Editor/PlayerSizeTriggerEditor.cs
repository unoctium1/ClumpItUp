using System.Collections;
using System.Collections.Generic;
using Unity.LEGO.Behaviours.Triggers;
using UnityEditor;
using UnityEngine;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(PlayerSizeTrigger), true)]
    public class PlayerSizeTriggerEditor : TriggerEditor
    {

        SerializedProperty m_TargetSize;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_TargetSize = serializedObject.FindProperty("m_TargetSize");
        }

        protected override void CreateGUI()
        {
            CreateTargetGUI();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.PropertyField(m_TargetSize);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(m_RepeatProp);

            CreateConditionsGUI();
        }

    }
}
