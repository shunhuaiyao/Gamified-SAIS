using System;
using System.Collections.Generic;
using Assets.Helpers;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(SimplePolygon))]
public class ObjectBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		SimplePolygon myScript = (SimplePolygon)target;
		if(GUILayout.Button("Rebuild mesh"))
		{
			myScript.Rebuild();
		}
	}
}
#endif


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimplePolygon : MonoBehaviour
{
	public bool hasDuplicates;
	public List<Vector3> verts;
	MeshFilter filter;

	public Mesh load (List<Vector3> _verts) {

		verts = _verts;

		filter = gameObject.GetComponent<MeshFilter> ();
		if (filter == null) {
			filter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
		}
			
		Mesh mesh = CreateMesh(verts.ToList());
		filter.sharedMesh = mesh;
		return mesh;
	}

	public Mesh loadExtruded (List<Vector3> _verts, float height) {

		verts = _verts;

		filter = gameObject.GetComponent<MeshFilter> ();
		if (filter == null) {
			filter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
		}

		List <Vector3> vertices = verts.ToList ();

		Mesh mesh = CreateMesh(vertices);
		filter.sharedMesh = SimpleExtruder.Extrude (mesh, gameObject,height);
		return mesh;
	}


	public void Rebuild () {
		Debug.Log ("Rebuild mesh");
		filter = GetComponent<MeshFilter> ();
		load (verts);
	}

	public Mesh CreateMesh(List<Vector3> verts)
	{

		Triangulator triangulator = new Triangulator(verts.Select(x => x.ToVector2xz()).ToArray());
		Mesh mesh = new Mesh();

		List<Vector3> vertices = verts;
		List<int> indices = triangulator.Triangulate().ToList();

		var n = vertices.Count;
		for (int index = 0; index < n; index++)
		{
			var v = vertices[index];
			vertices.Add(new Vector3(v.x, v.y, v.z));
		}

		for (int i = 0; i < n - 1; i++)
		{
			indices.Add(i);
			indices.Add(i + n);
			indices.Add(i + n + 1);
			indices.Add(i);
			indices.Add(i + n + 1);
			indices.Add(i + 1);
		}

		indices.Add(n - 1);
		indices.Add(n);
		indices.Add(0);

		indices.Add(n - 1);
		indices.Add(n + n - 1);
		indices.Add(n);

		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();

		Vector2[] uvs = new Vector2[vertices.ToArray().Length];

		for (int i=0; i < uvs.Length; i++) {
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		}
		mesh.uv = uvs;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize ();

		return mesh;
	}

}

