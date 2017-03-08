using Assets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace GoMap
{
	[ExecuteInEditMode]
	[System.Serializable]
	public class GOMap : MonoBehaviour 
	{
		public LocationManager locationManager;
		public int tileBuffer = 2;
		public int zoomLevel = 0;
	//	public bool delayedFeatureLoad;
		public bool useCache = true;
		public string mapzen_api_key = "";
		public Layer[] layers;
		public bool dynamicLoad = true;

		public event OnTileLoad onTileLoad;
		public delegate void OnTileLoad(GOTile current);

		Vector2 Center_tileCoords;
		[HideInInspector]
		public List <GOTile> tiles = new List<GOTile>();

		void Awake () 
	    {
			locationManager.onOriginSet += OnOriginSet;
			locationManager.onLocationChanged += OnLocationChanged;

			if (zoomLevel == 0) {
				zoomLevel = locationManager.zoomLevel;	
			}

			if (mapzen_api_key == null || mapzen_api_key == "") {
				Debug.Log ("GOMap - Mapzen api key is missing, GET iT HERE: https://mapzen.com/developers");
			}
			#if UNITY_WEBPLAYER
				Debug.LogError ("GOMap is NOT supported in the webplayer! Please switch platform in the build settings window.");
			#endif

	    }

		public IEnumerator ReloadMap (Coordinates location, bool delayed) {

			if (!dynamicLoad) {
				yield break;
			}
				
			//Get SmartTiles
			List <Vector2> tileList = location.adiacentNTiles(zoomLevel,tileBuffer);
							
			// Create new tiles
			foreach (Vector2 tileCoords in tileList) {

				if (!isSmartTileAlreadyCreated (tileCoords, zoomLevel)) {
					
					GOTile adiacentSmartTile = createSmartTileObject (tileCoords, zoomLevel);
					adiacentSmartTile.tileCenter = new Coordinates (tileCoords, zoomLevel);
					adiacentSmartTile.diagonalLenght = adiacentSmartTile.tileCenter.diagonalLenght(zoomLevel);
						
					if (onTileLoad != null) {
						onTileLoad (adiacentSmartTile);
					}

					#if !UNITY_WEBPLAYER

					if (FileHandler.Exist (adiacentSmartTile.gameObject.name) && useCache) {
						yield return adiacentSmartTile.StartCoroutine(adiacentSmartTile.LoadTileData(this, adiacentSmartTile.tileCenter, zoomLevel,layers,delayed));
					} else {
						adiacentSmartTile.StartCoroutine(adiacentSmartTile.LoadTileData(this, adiacentSmartTile.tileCenter, zoomLevel,layers,delayed));
					}
					#endif
				}
			}
				
			//Destroy far tiles
			List <Vector2> tileListForDestroy = location.adiacentNTiles(zoomLevel,tileBuffer+1);
			yield return StartCoroutine (DestroyTiles(tileListForDestroy));

		}

		IEnumerator DestroyTiles (List <Vector2> list) {

			List <string> tileListNames = new List <string> ();
			foreach (Vector2 v in list) {
				tileListNames.Add (v.x + "-" + v.y + "-" + zoomLevel);
			}

			List <GOTile> toDestroy = new List<GOTile> ();
			foreach (GOTile tile in tiles) {
				if (!tileListNames.Contains (tile.name)) {
					toDestroy.Add (tile);
				}
			}
			for (int i = 0; i < toDestroy.Count; i++) {
				GOTile tile = toDestroy [i];
				tiles.Remove (tile);
				GameObject.Destroy (tile.gameObject,i);
			}

			yield return null;
		}

		void OnLocationChanged (Coordinates currentLocation) {
			StartCoroutine(ReloadMap (currentLocation,true));
		}

		void OnOriginSet (Coordinates currentLocation) {
			StartCoroutine(ReloadMap (currentLocation,false));
		}

		bool isSmartTileAlreadyCreated (Vector2 tileCoords, int Zoom) {
			
			string name = tileCoords.x+ "-" + tileCoords.y+ "-" + zoomLevel;
			return transform.Find (name);
		}

		GOTile createSmartTileObject (Vector2 tileCoords, int Zoom) {
		
			GameObject tileObj = new GameObject(tileCoords.x+ "-" + tileCoords.y+ "-" + zoomLevel);
			tileObj.transform.parent = gameObject.transform;
			GOTile tile = tileObj.AddComponent<GOTile> ();

			tiles.Add(tile);
			return tile;
		}

		public void dropPin(double lat, double lng, GameObject go) {

			Transform pins = transform.FindChild ("Pins");
			if (pins == null) {
				pins = new GameObject ("Pins").transform;
				pins.parent = transform;
			}

			Coordinates coordinates = new Coordinates (lat, lng,0);
			go.transform.localPosition = coordinates.convertCoordinateToVector(0);
			go.transform.parent = pins;	
		}
	}

	[System.Serializable]
	public class Layer
	{
		public string name;
		public string json;
		public bool isPolygon;
		public float defaultHeight;
		public bool useRealHeight = false;
		public float defaultWidth;
		public float defaultOutlineWidth;
		public float distanceFromFloor;
		public Material defaultMaterial;
		public Material defaultOutline;
		public Material defaultRoof;
		public RenderingOptions [] renderingOptions;

		public string[] useOnly;
		public string[] avoid;
		public bool useTunnels = true;
		public bool useBridges = true;

		public bool startInactive;
		public bool disabled = false;

		public bool navObstacle;
	}

	[System.Serializable]
	public class RenderingOptions
	{
		public string kind;
		public Material material;
		public Material outlineMaterial;
		public Material roofMaterial;
		public float lineWidth;
		public float outlineWidth;
		public float polygonHeight;
		public float distanceFromFloor;

	}

}