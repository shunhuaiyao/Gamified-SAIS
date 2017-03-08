using UnityEngine;
using UnityEditor;
using System.Collections;

namespace GoMap
{
	public static class GoMapMenu 
	{
		[MenuItem("GameObject/3D Object/GO Map")]
		public static void AddMap()
		{

			GameObject locationManager = CreateGameObjectInScene("Location Manager");
			CenterOnScreen(locationManager, 0);
			GameObject newMap = CreateGameObjectInScene("Map");
			CenterOnScreen(newMap, 0);
			GOMap map = newMap.AddComponent<GOMap> ();
			map.locationManager = locationManager.AddComponent<LocationManager>();
		
			Layer buildings = new Layer ();
			buildings.name = "Buildings";
			buildings.json = "buildings";
			buildings.isPolygon = true;
			buildings.defaultHeight = 2;

			Layer roads = new Layer ();
			roads.name = "Roads";
			roads.json = "roads";
			roads.isPolygon = false;
			roads.defaultWidth = 2;

			map.layers = new Layer[] {buildings, roads};
		
			Camera.main.transform.position = new Vector3 (0, 600, 100);
			Camera.main.transform.eulerAngles = new Vector3 (70, 180, 0);

		}

		public static GameObject CreateGameObjectInScene(string name)
		{
			GameObject go = new GameObject(GetRealName(name));
			if (Selection.activeGameObject != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
				if (assetPath.Length == 0) 
				{
					go.transform.parent = Selection.activeGameObject.transform;
					go.layer = Selection.activeGameObject.layer;
				}
			}
			
			ResetLocalTransform(go);
			return go;
		}

		public static string GetRealName(string name)
		{
			string realName = name;
			int counter = 0;
			while (GameObject.Find(realName) != null)
			{ 
				realName = name + counter++; 
			}
			return realName;
		}

		public static void ResetLocalTransform(GameObject go)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;	
		}

		public static void CenterOnScreen( GameObject obj, float depth) 
		{
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null) return;
			Camera sceneCam = sceneView.camera;
			Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0f));
			obj.transform.position = new Vector3(Mathf.Round(spawnPos.x), Mathf.Round(spawnPos.y), depth);
			Selection.activeGameObject = obj;
		}
	}
}
	