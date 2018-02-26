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
	}
}