using LEGOModelImporter;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ContextExtensions
{
    [MenuItem("CONTEXT/Model/Move Lights to Top of Hierarchy")]
    static void FetchAllLights(MenuCommand context)
    {
        Model root = (Model)context.context;
        RemoveAllLights(root.transform, root.transform.root);
        
    }

    private static void RemoveAllLights(Transform root, Transform newParent)
    {
        if (root.childCount == 0) return;
        for(int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if(child.gameObject.TryGetComponent(out Light _))
            {
                Debug.Log("Light found");
                child.parent = newParent;
            }
            RemoveAllLights(child, newParent);
        }
    }
}
