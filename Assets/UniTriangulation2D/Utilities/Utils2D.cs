using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniTriangulation2D {

	public static class Utils2D {

		/// <summary>
		/// 引数の座標群から最大値と最小値を計算し、格納する。
		/// </summary>
		/// <param name="points">Points.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public static void ComputeBound(List<Vector2> points, out Vector2 min, out Vector2 max) {
			min = max = points[0];
			for(int i = 1; i < points.Count; ++i) {
				min.x = Mathf.Min(min.x, points[i].x);
				min.y = Mathf.Min(min.y, points[i].y);

				max.x = Mathf.Max(max.x, points[i].x);
				max.y = Mathf.Max(max.y, points[i].y);
			}
		}

		/// <summary>
		/// 頂点群とある点との内外判定を行い、結果を返す。
		/// 参考: https://wrf.ecse.rpi.edu//Research/Short_Notes/pnpoly.html
		/// </summary>
		/// <returns>The pnpoly.</returns>
		/// <param name="verts">Vertices.</param>
		/// <param name="test">Test.</param>
		public static bool Pnpoly(List<Vector2> verts, Vector2 test) {
			int i, j;
			bool c = false;
			for(i = 0, j = verts.Count - 1; i < verts.Count; j = i++) {
				if(
					((verts[i].y > test.y) != (verts[j].y > test.y)) &&
					(test.x < (verts[j].x - verts[i].x) * (test.y - verts[i].y) / (verts[j].y - verts[i].y) + verts[i].x)
				) {
					c = !c;
				}
			}
			return c;
		}

		/// <summary>
		/// 三角形の入力からメッシュを作成する。
		/// </summary>
		/// <returns>The to mesh.</returns>
		/// <param name="triangles">Triangles.</param>
		/// <param name="color">Color.</param>
		public static Mesh TrianglesToMesh(List<Triangle2D> triangles, Color color) {
			Dictionary<Vector2, int> point2Index = new Dictionary<Vector2, int>();
			List<Vector3> vertices = new List<Vector3>();

			for(var i = 0; i < triangles.Count; ++i) {
				var t = triangles[i];
				if(!point2Index.ContainsKey(t.p0)) {
					point2Index.Add(t.p0, point2Index.Count);
					vertices.Add(t.p0);
				}
				if(!point2Index.ContainsKey(t.p1)) {
					point2Index.Add(t.p1, point2Index.Count);
					vertices.Add(t.p1);
				}
				if(!point2Index.ContainsKey(t.p2)) {
					point2Index.Add(t.p2, point2Index.Count);
					vertices.Add(t.p2);
				}
			}

			Color32 vColor = color;
			Color32[] colors = new Color32[vertices.Count];
			for(var i = 0; i < colors.Length; ++i) {
				colors[i] = vColor;
			}

			int[] indices = new int[triangles.Count * 3];
			int index;
			for(var i = 0; i < triangles.Count; ++i) {
				var t = triangles[i];
				if(point2Index.TryGetValue(t.p0, out index)) {
					indices[i * 3] = index;
				}
				if(point2Index.TryGetValue(t.p1, out index)) {
					indices[i * 3 + 1] = index;
				}
				if(point2Index.TryGetValue(t.p2, out index)) {
					indices[i * 3 + 2] = index;
				}
			}

			var mesh = new Mesh();
			mesh.name = "Unnamed Mesh";
			mesh.SetVertices(vertices);
			mesh.colors32 = colors;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();

			return mesh;
		}

		/// <summary>
		/// 入力された輪郭座標から交点を求める。
		/// </summary>
		/// <returns>The intersect.</returns>
		/// <param name="contour">Contour.</param>
		public static List<Vector2> Intersect(List<Vector2> contour) {
			List<Vector2> intersections = new List<Vector2>();
			Vector2 t;
			for(int i = 0, j = contour.Count - 1; i < contour.Count; j = i++) {
				for(int k = contour.Count - 1, l = 0; k > i; l = k--) {
					if(TryIntersect(contour[i], contour[j], contour[k], contour[l], out t)) {
						intersections.Add(t);
					}
				}
			}
			return intersections;
		}

		/// <summary>
		/// 二つの線分の交差判定を行う。
		/// http://www.hiramine.com/programming/graphics/2d_segmentintersection.html
		/// </summary>
		/// <returns>The intersect.</returns>
		/// <param name="from1">From1.</param>
		/// <param name="to1">To1.</param>
		/// <param name="from2">From2.</param>
		/// <param name="to2">To2.</param>
		public static bool TryIntersect(Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2, out Vector2 intersection) {
			float b = (to1.x - from1.x) * (to2.y - from2.y) - (to1.y - from1.y) * (to2.x - from2.x);
			if(b == 0f) {
				intersection = Vector2.zero;
				return false;
			}

			Vector2 vff = from2 - from1;

			float r = ((to2.y - from2.y) * vff.x - (to2.x - from2.x) * vff.y) / b;
			float s = ((to1.y - from1.y) * vff.x - (to1.x - from1.x) * vff.y) / b;
			intersection = from1 + r * (to1 - from1);

			return !(r < 0f || 1f < r || s < 0f || 1f < s);
		}
	}
}