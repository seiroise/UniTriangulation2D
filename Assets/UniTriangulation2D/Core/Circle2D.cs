using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniTriangulation2D {

	/// <summary>
	/// 中心と半径をもち円を表現する
	/// </summary>
	public class Circle2D {

		public Vector2 center { get; private set; }
		public float radius { get; private set; }
		float sqrRadius { get; set; }

		public Circle2D(Vector2 center, float radius) {
			this.center = center;
			this.radius = radius;
			this.sqrRadius = radius * radius;
		}

		public void DebugDraw() {
			DebugExtention.DrawCircle2D(center, radius, Color.gray, 32);
		}

		public bool Overlap(Vector2 point) {
			float sqrLen = (point - center).sqrMagnitude;
			return sqrLen <= sqrRadius;
		}
	}
}