using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniTriangulation2D {

	public class Triangle2D {

		Circle2D _circumscribedCircle;

		public Vector2 p0 { get; private set; }
		public Vector2 p1 { get; private set; }
		public Vector2 p2 { get; private set; }

		public Circle2D circumscribedCircle {
			get {
				if(_circumscribedCircle == null) _circumscribedCircle = ComputeCircumscribedCircle();
				return _circumscribedCircle;
			}
		}

		public Triangle2D(Vector2 p0, Vector2 p1, Vector2 p2) {
			this.p0 = p0;
			this.p1 = p1;
			this.p2 = p2;
		}

		public override bool Equals(object obj) {
			Triangle2D t = obj as Triangle2D;
			return t.GetHashCode() == GetHashCode();
		}

		public override int GetHashCode() {
			// 頂点の格納順序も一致している必要があるならコメントアウトされてる方を使う。
			// return p0.GetHashCode() ^ (p1.GetHashCode() * 123) ^ (p2.GetHashCode() * 456);
			return p0.GetHashCode() ^ p1.GetHashCode() ^ p2.GetHashCode();
		}

		/// <summary>
		/// 他の三角形と共有点を持つか。
		/// </summary>
		/// <returns><c>true</c>, if common points was hased, <c>false</c> otherwise.</returns>
		/// <param name="t">T.</param>
		public bool HasCommonPoints(Triangle2D t) {
			return (
				p0 == t.p0 || p0 == t.p1 || p0 == t.p2 ||
				p1 == t.p0 || p1 == t.p1 || p1 == t.p2 ||
				p2 == t.p0 || p2 == t.p1 || p2 == t.p2);
		}

		/// <summary>
		/// 外接円を計算し、返す
		/// </summary>
		/// <returns>The circumscribed circle.</returns>
		Circle2D ComputeCircumscribedCircle() {
			float x0 = p0.x;
			float y0 = p0.y;
			float x1 = p1.x;
			float y1 = p1.y;
			float x2 = p2.x;
			float y2 = p2.y;

			Vector2 center;
			float radius;

			float c = 2f * ((x1 - x0) * (y2 - y0) - (y1 - y0) * (x2 - x0));
			float x = ((y2 - y0) * (x1 * x1 - x0 * x0 + y1 * y1 - y0 * y0)
					   + (y0 - y1) * (x2 * x2 - x0 * x0 + y2 * y2 - y0 * y0)) / c;

			float y = ((x0 - x2) * (x1 * x1 - x0 * x0 + y1 * y1 - y0 * y0)
					   + (x1 - x0) * (x2 * x2 - x0 * x0 + y2 * y2 - y0 * y0)) / c;

			center = new Vector2(x, y);
			radius = (center - p0).magnitude;

			return new Circle2D(center, radius);
		}

		public void DebugDraw() {
			DebugExtention.DrawTriangle(p0, p1, p2, Color.green);
		}

		public Triangle2D Copy() {
			return MemberwiseClone() as Triangle2D;
		}
	}
}