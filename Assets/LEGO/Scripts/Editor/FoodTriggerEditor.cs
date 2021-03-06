using System.Collections;
using System.Collections.Generic;
using Unity.LEGO.Behaviours.Actions;
using Unity.LEGO.Behaviours.Triggers;
using UnityEditor;
using UnityEngine;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(FoodTrigger), true)]
    public class FoodTriggerEditor : TriggerEditor
    {
        FoodAction m_FocusedPickup;

        SerializedProperty m_ModeProp;
        SerializedProperty m_AmountModeCountProp;
        SerializedProperty m_SpecificModePickupActionsProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_ModeProp = serializedObject.FindProperty("m_Mode");
            m_AmountModeCountProp = serializedObject.FindProperty("m_AmountModeCount");
            m_SpecificModePickupActionsProp = serializedObject.FindProperty("m_SpecificModePickupActions");
        }

        protected override void CreateGUI()
        {
            CreateTargetGUI();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.PropertyField(m_ModeProp);

            if ((FoodTrigger.Mode)m_ModeProp.enumValueIndex == FoodTrigger.Mode.AmountOfPickups)
            {
                EditorGUILayout.PropertyField(m_AmountModeCountProp, new GUIContent("Pickup Count"));
            }
            else if ((FoodTrigger.Mode)m_ModeProp.enumValueIndex == FoodTrigger.Mode.SpecificPickups)
            {
                if (EditorGUILayout.PropertyField(m_SpecificModePickupActionsProp, new GUIContent("Specific Pickups"), false))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_SpecificModePickupActionsProp.FindPropertyRelative("Array.size"));
                    for (var i = 0; i < m_SpecificModePickupActionsProp.arraySize; ++i)
                    {
                        GUI.SetNextControlName("Pickup " + i);
                        EditorGUILayout.PropertyField(m_SpecificModePickupActionsProp.GetArrayElementAtIndex(i));
                    }
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(m_RepeatProp);

            var previousFocusedPickup = m_FocusedPickup;

            // Find the currently focused Pickup Action.
            var focusedControlName = GUI.GetNameOfFocusedControl();
            var lastSpace = focusedControlName.LastIndexOf(' ');
            if (focusedControlName.StartsWith("Pickup") && lastSpace >= 0)
            {
                var index = int.Parse(focusedControlName.Substring(lastSpace + 1));
                if (index < m_SpecificModePickupActionsProp.arraySize)
                {
                    m_FocusedPickup = (FoodAction)m_SpecificModePickupActionsProp.GetArrayElementAtIndex(index).objectReferenceValue;
                }
                else
                {
                    m_FocusedPickup = null;
                }
            }
            else
            {
                m_FocusedPickup = null;
            }

            if (m_FocusedPickup != previousFocusedPickup)
            {
                SceneView.RepaintAll();
            }

            CreateConditionsGUI();
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            if (Event.current.type == EventType.Repaint)
            {
                if (m_Trigger && (FoodTrigger.Mode)m_ModeProp.enumValueIndex == FoodTrigger.Mode.SpecificPickups)
                {
                    var pickupActionsList = new List<FoodAction>();
                    for (var i = 0; i < m_SpecificModePickupActionsProp.arraySize; i++)
                    {
                        pickupActionsList.Add((FoodAction)m_SpecificModePickupActionsProp.GetArrayElementAtIndex(i).objectReferenceValue);
                    }

                    DrawConnections(m_Trigger, pickupActionsList, false, Color.green, m_FocusedPickup);
                }
            }
        }
    }
}
