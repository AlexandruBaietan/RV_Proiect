using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GoogleARCore.Examples.HelloAR.ARBowling))]
public class ARBowlingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        

        //initialize process
        base.OnInspectorGUI();
        GoogleARCore.Examples.HelloAR.ARBowling arbowling = (GoogleARCore.Examples.HelloAR.ARBowling)target;



        if (GUILayout.Button("CHECK PINS"))
        {
            arbowling.checkPins();
        }

    }




}