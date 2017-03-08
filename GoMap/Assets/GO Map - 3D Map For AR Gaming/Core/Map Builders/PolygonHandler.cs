using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GoMap {
	
	public class PolygonHandler
    {
        public GameObject GameObject { get; set; }
        private List<Vector3> _verts;
		private List<Vector3> properties;

		public PolygonHandler(List<Vector3> verts)
        {
            _verts = verts;
        }

		public GameObject CreateModel(Layer layer, float height)
        {

			SimplePolygon polygon = new GameObject ().AddComponent<SimplePolygon> ();

			if (height > 0) {
				polygon.loadExtruded (_verts, height);
			} else {
				Mesh mesh = polygon.load(_verts);
				bool normalFaceDown = mesh.normals [0].y > 0 ;
				if (!normalFaceDown) {
					polygon.gameObject.AddComponent<ReverseNormals>();
				}
			}	
				
			return polygon.gameObject;
        }
    }
}
