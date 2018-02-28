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
	}
}