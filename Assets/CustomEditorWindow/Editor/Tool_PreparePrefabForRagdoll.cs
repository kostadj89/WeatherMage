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
            AddComponentsToGameObject(newPrefab);

            //SetupRagdoll(newPrefab);

            newPrefabName = objectPrefab.name + "_Ragdoll";
            string assetPath = newPrefabPath + "\\Ragdolls/" + newPrefabName + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(newPrefab, assetPath);

            DestroyImmediate(newPrefab,true);
            Debug.Log(string.Format("New prefab {0} has been created at the path {1}", newPrefabName, assetPath));
        }
    }

    private void AddComponentsToGameObject(GameObject newPrefab)
    {
        newPrefab.AddComponent<UnitRagdollSetup>();
    }

    private void SetupRagdoll(GameObject newPrefab)
    {
        float totalMass = 20f;
        float strength = 0f;

        GameObject ragdoll = new GameObject("Ragdoll");
        ragdoll.transform.position = newPrefab.transform.position;
        ragdoll.transform.rotation = newPrefab.transform.rotation;

        // Calculate the total mass of the Ragdoll
        float massPerBone = totalMass / 12f;

        // Create a list of bone names to look for
        List<string> boneNames = new List<string> {
            "Hips",
            "UpperLeg_L",
            "LowerLeg_L",
            //"Ankle_L",
            "UpperLeg_R",
            "LowerLeg_R",
            //"Ankle_R",
            "Spine_02",
            "Shoulder_L",
            "Elbow_L",
            "Shoulder_R",
            "Elbow_R",
            "Head"
        };

        // Find all the bones in the newPrefab with the given names
        Dictionary<string, Rigidbody> boneMap = new Dictionary<string, Rigidbody>();
        foreach (string name in boneNames)
        {
            //recursion eeek, find bone with a specific name in gameObject's hierarchy
            Transform boneTransform = FindChildObjectWithNameInHierarchy(newPrefab.transform,name);

            if (boneTransform != null)
            {
                Rigidbody boneRigidbody = boneTransform.GetComponent<Rigidbody>();
                if (boneRigidbody == null)
                {
                    boneRigidbody = boneTransform.gameObject.AddComponent<Rigidbody>();
                }

                Collider boneCollider = boneTransform.GetComponent<Collider>();
                if (boneCollider == null)
                {
                    if (name.Contains("UpperLeg") || name.Contains("Shoulder") || name.Contains("Elbow")|| name.Contains("LowerLeg"))
                    {
                        CapsuleCollider capsuleCollider = boneTransform.gameObject.AddComponent<CapsuleCollider>();
                        //x-axis
                        capsuleCollider.direction = 0;
                        
                        float boneLength = boneTransform.localScale.magnitude;
                        capsuleCollider.height = boneLength / 8;
                        capsuleCollider.radius = boneLength / 30; // adjust this value to fit your specific use case                        
                        // Set the center of the collider to match the position of the bone
                        capsuleCollider.center = new Vector3(0f,0f,0f);
                        boneCollider = capsuleCollider;
                    }

                    else if (name.Contains("Head"))
                    {
                        SphereCollider sphereCollider = boneTransform.gameObject.AddComponent<SphereCollider>();
                        sphereCollider.radius = 0.19f;
                        sphereCollider.center = new Vector3(0f, 0f, 0f);

                        boneCollider = sphereCollider;
                    }
                    else if (name.Contains("Spine") || name.Contains("Hips"))
                    {
                         // X-axis
                         BoxCollider boxCollider = boneTransform.gameObject.AddComponent<BoxCollider>();
                        boxCollider.center = new Vector3(0f, 0f, 0f);
                        boxCollider.size = new Vector3(0.23f, 0.27f, 0.38f);
                        boneCollider = boxCollider;
                    }
                    else
                    {
                        continue;
                    }
                }

                boneCollider.isTrigger = false;

                boneRigidbody.mass = massPerBone;
                boneRigidbody.interpolation = RigidbodyInterpolation.None;
                boneRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

                boneMap.Add(name, boneRigidbody);
            }
        }

        // Create joints between the bones
        CreateJoint(boneMap["Hips"], boneMap["UpperLeg_L"]);
        CreateJoint(boneMap["UpperLeg_L"], boneMap["LowerLeg_L"]);
        //CreateJoint(boneMap["LowerLeg_L"], boneMap["Ankle_L"]);
        //CreateJoint(boneMap["Ankle_L"], null);
        CreateJoint(boneMap["LowerLeg_L"], null);

        CreateJoint(boneMap["Hips"], boneMap["UpperLeg_R"]);
        CreateJoint(boneMap["UpperLeg_R"], boneMap["LowerLeg_R"]);
        //CreateJoint(boneMap["LowerLeg_R"], boneMap["Ankle_R"]);
        //CreateJoint(boneMap["Ankle_R"], null);
        CreateJoint(boneMap["LowerLeg_R"], null);

        CreateJoint(boneMap["Hips"], boneMap["Spine_02"]);
        CreateJoint(boneMap["Spine_02"], null);

        CreateJoint(boneMap["Spine_02"], boneMap["Shoulder_L"]);
        CreateJoint(boneMap["Shoulder_L"], boneMap["Elbow_L"]);
        CreateJoint(boneMap["Elbow_L"], null);

        CreateJoint(boneMap["Spine_02"], boneMap["Shoulder_R"]);
        CreateJoint(boneMap["Shoulder_R"], boneMap["Elbow_R"]);
        CreateJoint(boneMap["Elbow_R"], null);

        CreateJoint(boneMap["Spine_02"], boneMap["Head"]);
        CreateJoint(boneMap["Head"], null);

        // Set the Ragdoll's strength
        foreach (Rigidbody boneRigidbody in boneMap.Values)
        {
            boneRigidbody.maxAngularVelocity = Mathf.Infinity;
            boneRigidbody.angularDrag = strength;
            boneRigidbody.drag = strength;
        }

        // Attach the Ragdoll to the newPrefab
        ragdoll.transform.SetParent(newPrefab.transform);

        // Disable the newPrefab's original Animator component
        Animator animator = newPrefab.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    private Transform FindChildObjectWithNameInHierarchy(Transform parent, string name)
    {
        if (parent.name == name)
        {
            return parent;
        }

        foreach(Transform transform in  parent.transform)
        {
            Transform result = FindChildObjectWithNameInHierarchy(transform, name);
            if (result!=null)
            {
                return result;
            }
        }

        return null;
    }

    private void CreateJoint(Rigidbody bone1, Rigidbody bone2)
    {
        if (bone1 != null && bone2 != null)
        {
            ConfigurableJoint joint = bone1.gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = bone2;

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = Vector3.zero;
            joint.axis = Vector3.zero;
            joint.secondaryAxis = Vector3.zero;

            SoftJointLimit limit = new SoftJointLimit();
            limit.limit = 0.1f;
            joint.linearLimit = limit;
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
        UnitRagdollSpawner unitRagdollSpawner = gameObject.GetComponent<UnitRagdollSpawner>();

        // Remove the components from the GameObject
        if (shootAction != null) DestroyImmediate(shootAction);//,true);
        if (moveAction != null) DestroyImmediate(moveAction);//, true);
        if (spinAction != null) DestroyImmediate(spinAction);//, true);
        if (unit != null) DestroyImmediate(unit);//, true);
        if (healthSystem != null) DestroyImmediate(healthSystem);//, true);
        if (boxCollider != null) DestroyImmediate(boxCollider);//, true);
        if (unitAnimator != null) DestroyImmediate(unitAnimator);//, true);
        if (unitRagdollSpawner != null) DestroyImmediate(unitRagdollSpawner);//, true); 

        //remove selected visual child
        GameObject selectedVisual = gameObject.transform.Find("SelectedVisual")?.gameObject;
        if (selectedVisual != null) { DestroyImmediate(selectedVisual); }//, true); }

        //remove UnitWorldUI child
        GameObject UnitWorldUI = gameObject.transform.Find("UnitWorldUI")?.gameObject;
        if (UnitWorldUI != null) { DestroyImmediate(UnitWorldUI); }//, true); }

        //remove Animator component if it exists
        Animator animator = gameObject.GetComponentInChildren<Animator>();
        if (animator != null) { DestroyImmediate(animator); }//, true); }

        }
}
