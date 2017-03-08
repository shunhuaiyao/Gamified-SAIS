using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Helpers;
using UnityEngine;

namespace GoMap
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RoadPolygon : MonoBehaviour
    {
        public List<Vector3> _verts;

		public void Initialize( List<Vector3> verts, string kind, Layer layer, int sort)
        {
            _verts = verts;

			RenderingOptions renderingOptions = null;
			foreach (RenderingOptions r in layer.renderingOptions) {
				if (r.kind == kind) {
					renderingOptions = r;
					break;
				}
			}
			float width = layer.defaultWidth;
			float defaultY = layer.distanceFromFloor;
			Material material = layer.defaultMaterial;
			Material outlineMaterial = layer.defaultOutline;
			float outlineWidth = width + layer.defaultOutlineWidth;
			if (renderingOptions != null) {
				width = renderingOptions.lineWidth;
				material = renderingOptions.material;
				defaultY = renderingOptions.distanceFromFloor;
				outlineMaterial = renderingOptions.outlineMaterial;
				outlineWidth = width + renderingOptions.outlineWidth;
			}

			if (defaultY == 0f)
				defaultY = sort/1000.0f;

			material.renderQueue = -sort;

			SimpleRoad road = gameObject.AddComponent<SimpleRoad> ();

			Vector3[] vertices = _verts.Select(x => new Vector3(x.x, 0, x.z)).ToArray();
			road.verts = vertices;
			road.width = width;

			road.load ();

			Vector3 position = transform.position;
			position.y = defaultY;
			transform.position = position;

			gameObject.GetComponent<Renderer>().material = material;

			if (outlineMaterial != null) {
				CreateRoadOutline (verts, outlineMaterial, outlineWidth, sort,defaultY);
			}

        }

		GameObject CreateRoadOutline (List<Vector3> verts, Material material, float width, int sort, float y) {

			GameObject outline = new GameObject ("outline");
			outline.transform.parent = transform;

			material.renderQueue = -(sort-1);

			SimpleRoad road = outline.AddComponent<SimpleRoad> ();

			Vector3[] vertices = _verts.Select(x => new Vector3(x.x, 0, x.z)).ToArray();
			road.verts = vertices;
			road.width = width;

			road.load ();

			Vector3 position = outline.transform.position;
			position.y = -0.025f;
			outline.transform.localPosition = position;

			outline.GetComponent<Renderer>().material = material;

			return outline;
		}

    }

}
