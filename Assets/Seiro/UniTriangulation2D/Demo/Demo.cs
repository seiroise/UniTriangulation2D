using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.UniTriangulation2D.Demo
{

    [RequireComponent(typeof(LineRenderer), typeof(MeshRenderer), typeof(MeshFilter))]
    public class Demo : MonoBehaviour
    {

        public Color[] sampleColors;

        public AnimationCurve curve;

        public bool drawDebug = false;

        Delaunay2D _delaunay;

        List<Vector2> _points;
        bool _isDragging;

        LineRenderer _lineRenderer;
        MeshFilter _meshFilter;
        MeshRenderer _meshRenderer;
        Material _material;

        Coroutine _coroutine;

        void Awake()
        {
            _points = new List<Vector2>();
            _lineRenderer = GetComponent<LineRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _material = _meshRenderer.sharedMaterial;
        }

        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                ClearPositions();
                _isDragging = true;
                AddPosition(GetMousePosition());
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;

                var sw = System.Diagnostics.Stopwatch.StartNew();
                _delaunay = Delaunay2D.Contour(_points);
                var mesh = _delaunay.ToMesh(sampleColors[Random.Range(0, sampleColors.Length)]);
                _meshFilter.mesh = mesh;
                sw.Stop();
                Debug.Log(sw.ElapsedMilliseconds);

                var maxDistance = mesh.bounds.size.magnitude * 0.5f + 1f;
                Vector3 center = mesh.bounds.center;

                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }
                _coroutine = StartCoroutine(ShowCotourine(1f, maxDistance, center));

            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 pos = GetMousePosition();
                if ((pos - _points[_points.Count - 1]).magnitude > 0.5f)
                {
                    AddPosition(pos);
                }
            }

            if (_delaunay != null && drawDebug)
            {
                _delaunay.ForeachTriangles((Triangle2D t) =>
                {
                    t.DebugDraw();
                    t.circumscribedCircle.DebugDraw();
                });
            }
        }

        Vector2 GetMousePosition()
        {
            Vector3 mpos = Input.mousePosition;
            mpos.z = -Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(mpos);
        }

        void AddPosition(Vector2 position)
        {
            _points.Add(position);
            _lineRenderer.positionCount = _points.Count;
            _lineRenderer.SetPosition(_points.Count - 1, position);
        }

        void ClearPositions()
        {
            _points.Clear();
            _lineRenderer.positionCount = 0;
        }

        IEnumerator ShowCotourine(float time, float distance, Vector3 center)
        {
            if (_material)
            {
                _material.SetVector("_MaskPosition", center);

                var e = 0f;
                var t = 0f;
                while (e < time)
                {
                    t = e / time;
                    _material.SetFloat("_Distance", curve.Evaluate(t) * distance);
                    e += Time.deltaTime;
                    yield return 0f;
                }
                _material.SetFloat("_Distance", distance);
            }
            _coroutine = null;
        }
    }
}