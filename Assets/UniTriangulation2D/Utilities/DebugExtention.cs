using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniTriangulation2D {

	public static class DebugExtention {

		static readonly float PI_2 = Mathf.PI * 2f;
		static readonly Matrix4x4 MAT_IDENTITY = Matrix4x4.identity;
		static readonly Quaternion QUAT_ROTATION_N30 = Quaternion.AngleAxis(-30f, Vector3.forward);
		static readonly Quaternion QUAT_ROTATION_P30 = Quaternion.AngleAxis(30f, Vector3.forward);

		public static void DrawCircle2D(Vector3 center, float radius, Color color, int res = 8) {

			float angleStep = PI_2 / res;
			float angle = 0f;

			for(int i = 0; i < res; ++i) {
				Vector3 p0 = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
				angle += angleStep;
				Vector3 p1 = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
				Debug.DrawLine(p0, p1, color);
			}
		}

		public static void DrawTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Color color) {
			Debug.DrawLine(p0, p1, color);
			Debug.DrawLine(p1, p2, color);
			Debug.DrawLine(p2, p0, color);
		}

		public static void DrawArrow(Vector3 from, Vector3 to, Color color) {
			var dir = (from - to) * 0.2f;
			Debug.DrawLine(from, to, color);
			Debug.DrawLine(to, to + QUAT_ROTATION_N30 * dir, color);
			Debug.DrawLine(to, to + QUAT_ROTATION_P30 * dir, color);
		}
	}
}