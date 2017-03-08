using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using MiniJSON;

namespace GoMap
{
	public class GOTile : MonoBehaviour
	{
		public Coordinates tileCenter;
		public float diagonalLenght;

		public Rect Rect;
		private object mapData;
		ParseJob job;

		public IEnumerator ParseJson (string data) {

			job = new ParseJob();
			job.InData = data;
			job.Start();

			yield return StartCoroutine(job.WaitFor());
		}

		public IEnumerator LoadTileData(object m, Coordinates tilecenter, int zoom, Layer[] layers, bool delayedLoad)
		{

			#if !UNITY_WEBPLAYER

			GOMap w = (GOMap)m;

			Vector2 realPos = tileCenter.tileCoordinates (zoom);

			var tileurl = realPos.x + "/" + realPos.y;

//			var baseUrl = "https://vector.mapzen.com/osm/";
			var baseUrl = "http://tile.mapzen.com/mapzen/vector/v1/";
			List <string> layerNames = new List<string>();
			for (int i = 0; i < layers.ToList().Count; i++) {
				if (layers [i].disabled == false) {
					layerNames.Add(layers [i].json);
				}
			}
			layerNames.RemoveAll(str => String.IsNullOrEmpty(str));
			var url = baseUrl + string.Join(",",layerNames.ToArray())+"/"+zoom+"/";
			var completeurl = url + tileurl + ".json"; 

			if (w.mapzen_api_key != null && w.mapzen_api_key != "") {
				completeurl = completeurl + "?api_key=" + w.mapzen_api_key;
			}


			if (w.useCache && FileHandler.Exist(gameObject.name))
			{
				yield return StartCoroutine (ParseJson(FileHandler.LoadText (gameObject.name)));
			}
			else
			{
				//Debug.Log (completeurl);

				var www = new WWW(completeurl);
				yield return www;

				if (www.error == null && www.text.Length > 0) {
					FileHandler.SaveText (gameObject.name, www.text);
					yield return StartCoroutine (ParseJson(www.text));
				} else {
					Debug.LogWarning("Tile data missing");
					((GOMap)m).tiles.Remove(this);
					GameObject.Destroy(this.gameObject);
					yield break;
				}


			}

			mapData = job.OutData;

			transform.position = tilecenter.convertCoordinateToVector();
			foreach (Layer layer in layers) {
				
				IDictionary layerData = null;
				try {
					if (layerNames.Count () == 1) {
						layerData = (IDictionary)mapData;
					} else {
						layerData = (IDictionary)((IDictionary)mapData) [layer.json];
					}
				}
				finally {
				
				}

//				IDictionary layerData = layerNames.Count() == 1? (IDictionary) mapData : (IDictionary)((IDictionary)mapData) [layer.json];

				if (!layer.disabled && w.dynamicLoad) {
					yield return StartCoroutine( BuildTile (layerData,layer,delayedLoad));
				} 
			}
			#else 
			yield return null;
			#endif
		}

		private IEnumerator BuildTile(IDictionary mapData, Layer layer, bool delayedLoad)
		{ 

			GameObject parent = new GameObject ();
			parent.name = layer.name;
			parent.transform.parent = this.transform;
			parent.SetActive (!layer.startInactive);

			if (mapData == null) {
				Debug.LogWarning ("Map Data is null!");
				yield break;
			}

			IList features = (IList)mapData ["features"];
			if (features == null)
				yield break;

			foreach (IDictionary geo in features) {
				
				IDictionary geometry = (IDictionary)geo ["geometry"];
				IDictionary properties = (IDictionary)geo ["properties"];
				string type = (string)geometry ["type"];
				string kind = (string)properties ["kind"];

				if (layer.useOnly.Length > 0 && !layer.useOnly.Contains (kind)) {
					continue;
				}
				if (layer.avoid.Length > 0 && layer.avoid.Contains (kind)) {
					continue;
				}

				if (type == "MultiLineString" || (type == "Polygon" && !layer.isPolygon)) {
					IList lines = new List<object>();
					lines = (IList)geometry ["coordinates"];
					foreach (IList coordinates in lines) {
						if (delayedLoad)
							yield return StartCoroutine (CreateLine (parent, kind, type, coordinates, properties, layer, delayedLoad));
						else
							StartCoroutine (CreateLine (parent, kind, type, coordinates, properties, layer, delayedLoad));
					}
				} 

				else if (type == "LineString") {
					IList coordinates = (IList)geometry ["coordinates"];
					if (delayedLoad)
						yield return StartCoroutine (CreateLine (parent, kind, type, coordinates, properties, layer, delayedLoad));
					else
						StartCoroutine (CreateLine (parent, kind, type, coordinates, properties, layer, delayedLoad));
				} 

				else if (type == "Polygon") {
					IList lines = new List<object>();
					lines = (IList)geometry["coordinates"];
					foreach (IList coordinates in lines) {
						if (delayedLoad)
							yield return StartCoroutine (CreatePolygon (parent, kind, type, coordinates, properties, layer, delayedLoad));
						else
							StartCoroutine (CreatePolygon (parent, kind, type, coordinates, properties, layer, delayedLoad));			
					}
				}

				else if (type == "MultiPolygon") {
					IList lines = new List<object>();
					lines = (IList)geometry["coordinates"];
					foreach (IList poly in lines) {
						foreach (IList coordinates in poly) {
							if (delayedLoad)
								yield return StartCoroutine (CreatePolygon (parent, kind, type, coordinates, properties, layer, delayedLoad));
							else
								StartCoroutine (CreatePolygon (parent, kind, type, coordinates, properties, layer, delayedLoad));							}
					}
				}
			}
		}

		private IEnumerator CreateLine(GameObject parent, string kind, string type, IList coordinates, IDictionary properties, Layer layer, bool delayedLoad)
		{

			bool isBridge = properties.Contains ("is_bridge") && properties ["is_bridge"].ToString() == "yes";
			bool isTunnel = properties.Contains ("is_tunnel") && properties ["is_tunnel"].ToString() == "yes";
			bool isLink = properties.Contains ("is_link") && properties ["is_link"].ToString() == "yes";

			if ((isBridge && !layer.useBridges) || (isTunnel && !layer.useTunnels) || (isLink && !layer.useBridges)) {
				yield break;
			}

			var l = new List<Vector3>();
			for (int i = 0; i < coordinates.Count; i++)
			{
				IList c = (IList)coordinates[i];
				Coordinates coords = new Coordinates ((double)c[1], (double)c[0],0);
				l.Add(coords.convertCoordinateToVector(layer.distanceFromFloor));

			}

			GameObject road = new GameObject (layer.json);
			RoadPolygon roadPolygon = road.AddComponent<RoadPolygon>();
			road.transform.parent = parent.transform;
			try
			{
				

				Int64 sort;
				if (properties.Contains("sort_key")) {
					sort = (Int64)properties["sort_key"];
				} else sort = (Int64)properties["sort_rank"];
				
			roadPolygon.Initialize(l, kind,layer,(int)sort);

//				Attributes attributes = road.AddComponent<Attributes>();
//				attributes.useName = true;
//				attributes.loadWithDictionary((Dictionary<string,object>)properties);
			}
			catch (Exception ex)
			{
				Debug.Log(ex);
			}
			if (delayedLoad)
				yield return null;
		}

		private IEnumerator CreatePolygon(GameObject parent, string kind, string type, IList coordinates, IDictionary properties, Layer layer, bool delayedLoad)
		{

			var l = new List<Vector3>();
			for (int i = 0; i < coordinates.Count - 1; i++)
			{
				IList c = (IList)coordinates[i];
				Coordinates coords = new Coordinates ((double)c[1], (double)c[0],0);
				Vector3 p = coords.convertCoordinateToVector (0);
				if (l.Contains (p)) {
					//That's totally empirical =(
					p.x = p.x + 0.01f;
				}
				l.Add (p);
			}

			GameObject polygon = null;
			try
			{

			PolygonHandler poly = new PolygonHandler(l);

			RenderingOptions renderingOptions = null;
			foreach (RenderingOptions r in layer.renderingOptions) {
				if (r.kind == kind) {
					renderingOptions = r;
					break;
				}
			}
			float height = layer.defaultHeight;
			float defaultY = layer.distanceFromFloor ;
			Material material = layer.defaultMaterial;
			Material roofMat = layer.defaultRoof == null ? layer.defaultMaterial : layer.defaultRoof;

			if (renderingOptions != null) {
				height = renderingOptions.polygonHeight;
				material = renderingOptions.material;
				defaultY = layer.distanceFromFloor;
				roofMat = renderingOptions.roofMaterial;

			}

			Int64 sort;
			if (properties.Contains("sort_key")) {
				sort = (Int64)properties["sort_key"];
			} else sort = (Int64)properties["sort_rank"];

			if (material == null) {
					yield break;
			}
			material.renderQueue = -(int)sort;

			if (defaultY == 0f) {
				defaultY = sort / 1000.0f;
			}

			if (layer.useRealHeight && properties.Contains("height")) {
				double h = (double)properties["height"];
				height = (float)h;
			}
			if (layer.useRealHeight && properties.Contains("min_height")) {
				double hm = (double)properties["min_height"];
				defaultY = (float)hm;
				height = (float)height-(float)hm;
			} 
				
			polygon = poly.CreateModel(layer,height);
			polygon.name = layer.name;
			polygon.transform.parent = parent.transform;

			if (layer.useRealHeight) {
				GameObject roof = poly.CreateModel(layer,0);
				roof.name = "roof";
				roof.transform.parent = polygon.transform;
				roof.GetComponent<MeshRenderer> ().material = roofMat;
				roof.transform.position = new Vector3 (roof.transform.position.x,height+0.1f,roof.transform.position.z);

				if (properties.Contains ("roof_material")) {
					roof.name = "roofmat_"+(string)properties["roof_material"];
				}

				if (properties.Contains ("roof_color")) {
					Debug.LogError ("Roof color: " + (string)properties["roof_color"]);
					polygon.name = "roofx";
				}

			}

			Vector3 pos = polygon.transform.position;
			pos.y = defaultY;
			polygon.transform.position = pos;
			polygon.transform.localPosition = pos;

			Attributes attributes = polygon.AddComponent<Attributes>();
			attributes.type = type;
			attributes.loadWithDictionary((Dictionary<string,object>)properties);
//
			polygon.GetComponent<Renderer>().material = material;

			}
			catch (Exception ex)
			{
				GameObject.Destroy (polygon);
				Debug.Log(ex);
			}
			if (delayedLoad)
				yield return null;
		}

		void OnDestroy() {
			//Debug.Log ("Destroy tile: "+gameObject.name);
		}
	}
}
