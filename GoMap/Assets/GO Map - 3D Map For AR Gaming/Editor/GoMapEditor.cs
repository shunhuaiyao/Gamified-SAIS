using UnityEngine;
using UnityEditor;
using System.Collections;

#if UNITY_EDITOR

namespace GoMap {

	[CustomEditor(typeof(GOMap))]
	public class GOMapEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

	//		Map map = (Map)target;
	//		if(GUILayout.Button("Build scene (edit mode)") && !Application.isPlaying)
	//		{
	//			Debug.Log ("Build scene");
	//			map.StartCoroutine (map.ReloadMap(map.locationManager.demo_CenterWorldCoordinates));
	//		}
		}
	}
}
#endif


	