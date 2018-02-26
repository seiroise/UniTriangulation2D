using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniTriangulation2D {

	[RequireComponent(typeof(LineRenderer))]
	public class Demo : MonoBehaviour {

		Delaunay2D _delaunay;

		List<Vector2> _points;
		bool _isDragging;

		LineRenderer _lineRenderer;

		void Awake() {
			_points = new List<Vector2>();
			_lineRenderer = GetComponent<LineRenderer>();
		}

		void Update() {

			if(Input.GetMouseButtonDown(0)) {
				ClearPositions();
				_isDragging = true;
				AddPosition(GetMousePosition());
			} else if(Input.GetMouseButtonUp(0)) {
				_isDragging = false;
				_delaunay = new Delaunay2D(_points);
			} else if(Input.GetMouseButton(0)) {
				Vector2 pos = GetMousePosition();
				if((pos - _points[_points.Count - 1]).magnitude > 0.5f) {
					AddPosition(pos);
				}
			}

			if(_delaunay != null) {
				_delaunay.ForeachTriangles((Triangle2D t) => {
					t.DebugDraw();
					t.circumscribedCircle.DebugDraw();
				});
			}
		}

		Vector2 GetMousePosition() {
			Vector3 mpos = Input.mousePosition;
			mpos.z = -Camera.main.transform.position.z;
			return Camera.main.ScreenToWorldPoint(mpos);
		}

		void AddPosition(Vector2 position) {
			_points.Add(position);
			_lineRenderer.positionCount = _points.Count;
			_lineRenderer.SetPosition(_points.Count - 1, position);
		}

		void ClearPositions() {
			_points.Clear();
			_lineRenderer.positionCount = 0;
		}
	}
}