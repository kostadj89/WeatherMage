using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using static PlasticPipe.Server.MonitorStats;

public class PreparePrefabForRagdoll : EditorWindow
{
    private GameObject objectPrefab;
    private string newPrefabPath;
    private string newPrefabName;

    [MenuItem("Tools/Create Prefab For Ragdoll")]
    public static void ShowWindow()
    {
        GetWindow<PreparePrefabForRagdoll>();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Prefab For Ragdoll", EditorStyles.boldLabel);

        newPrefabPath = "E:\\GameDev\\TurnBased3D\\WeatherMage\\Assets\\Prefabs";
        // Show the current path in a label
        EditorGUILayout.LabelField("Current Path: " + newPrefabPath);
        
        objectPrefab = EditorGUILayout.ObjectField("Original unit prefab", objectPrefab, typeof(GameObject), false) as GameObject;

        if(GUILayout.Button("Create a copy for a ragdoll") && objectPrefab !=  null)
        {
            //have to be instantiated because SaveAsPrefabAsset won't work otherwise
            GameObject newPrefab = Instantiate(objectPrefab);
            //= PrefabUtility.SaveAsPrefabAsset()
            RemoveComponentsFromGameObject( newPrefab);

            newPrefabName = objectPrefab.name + "_Ragdoll";
            string assetPath = newPrefabPath + "\\Ragdolls/" + newPrefabName + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(newPrefab, assetPath);

            DestroyImmediate(newPrefab,true);
            Debug.Log(string.Format("New prefab {0} has been created at the path {1}", newPrefabName, assetPath));
        }
    }

    private void RemoveComponentsFromGameObject( GameObject gameObject)
    {
        ShootAction shootAction = gameObject.GetComponent<ShootAction>();
        MoveAction moveAction = gameObject.GetComponent<MoveAction>();
        SpinAction spinAction = gameObject.GetComponent<SpinAction>();
        Unit unit = gameObject.GetComponent<Unit>();
        HealthSystem healthSystem = gameObject.GetComponent<HealthSystem>();
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        UnitAnimationManager unitAnimator = gameObject.GetComponent<UnitAnimationManager>();

        // Remove the components from the GameObject
        if (shootAction != null) DestroyImmediate(shootAction);//,true);
        if (moveAction != null) DestroyImmediate(moveAction);//, true);
        if (spinAction != null) DestroyImmediate(spinAction);//, true);
        if (unit != null) DestroyImmediate(unit);//, true);
        if (healthSystem != null) DestroyImmediate(healthSystem);//, true);
        if (boxCollider != null) DestroyImmediate(boxCollider);//, true);
        if (unitAnimator != null) DestroyImmediate(unitAnimator);//, true);

        //remove selected visual
        GameObject selectedVisual = gameObject.transform.Find("SelectedVisual")?.gameObject;
        if (selectedVisual != null) { DestroyImmediate(selectedVisual); }//, true); }

            //remove Animator component if it exists
            Animator animator = gameObject.GetComponentInChildren<Animator>();
        if (animator != null) { DestroyImmediate(animator); }//, true); }

        }
}
