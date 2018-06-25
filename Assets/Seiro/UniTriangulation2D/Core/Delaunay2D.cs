using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace Seiro.UniTriangulation2D
{


    /// <summary>
    /// ドロネー三角形分割クラス
    /// </summary>
    public class Delaunay2D
    {

        HashSet<Triangle2D> triangles { get; set; }
        List<Triangle2D> tempAdding { get; set; }
        List<Triangle2D> tempRemoving { get; set; }

        Dictionary<Triangle2D, bool> temp { get; set; }

        Triangle2D hugeTriangle { get; set; }

        /*
		 * Constructor
		 */

        /// <summary>
        /// 座標群からドロネー三角形分割を行う。
        /// </summary>
        /// <param name="points">Points.</param>
        public Delaunay2D(List<Vector2> points)
        {
            triangles = new HashSet<Triangle2D>();
            tempAdding = new List<Triangle2D>();
            tempRemoving = new List<Triangle2D>();

            temp = new Dictionary<Triangle2D, bool>();

            Triangulate(points);

            RemoveSkinnyTriangles();
        }

        /*
		 * Static methods
		 */

        /// <summary>
        /// 輪郭線情報から三角形分割を行う。
        /// </summary>
        /// <returns>The contour.</returns>
        /// <param name="contour">Contour.</param>
        public static Delaunay2D Contour(List<Vector2> contour)
        {

            var d = new Delaunay2D(contour);
            d.RemoveExternalTriangles(contour);

            return d;
        }

        /*
		 * Methods
		 */

        /// <summary>
        /// 引数の座標群から一連の三角形分割処理を行う
        /// </summary>
        /// <returns>The triangulate.</returns>
        /// <param name="points">Points.</param>
        void Triangulate(List<Vector2> points)
        {
            if (points == null)
            {
                throw new System.NullReferenceException();
            }
            if (points.Count <= 0)
            {
                throw new System.ArgumentException("座標は一つ以上必要です。");
            }

            hugeTriangle = GetHugeTriangle(points);
            triangles.Add(hugeTriangle);
            for (var i = 0; i < points.Count; ++i)
            {
                AddPoint(points[i]);
            }

            RemoveCommonPointWithHugeTriangle();
        }

        /// <summary>
        /// 座標を追加する
        /// </summary>
        /// <param name="point">Point.</param>
        public void AddPoint(Vector2 point)
        {
            // 現在までの三角形の外接円に包括されているかを確認する。
            foreach (var t in triangles)
            {
                if (t.circumscribedCircle.Overlap(point))
                {
                    SplitTriangle(t, point);
                    tempRemoving.Add(t);
                }
            }
            SolveTempTriangles();
        }

        /// <summary>
        /// 引数の座標群から全体を包括する三角形を作成し、返す。
        /// </summary>
        /// <returns>The huge triangle.</returns>
        /// <param name="points">Points.</param>
        Triangle2D GetHugeTriangle(List<Vector2> points)
        {
            Vector2 min, max;
            Utils2D.ComputeBound(points, out min, out max);

            Vector2 center = (min + max) * 0.5f;
            float radius = (max - min).magnitude * 0.5f + 0.1f; // minとmaxの差が0になるのを回避するための"0.1f"
            float scale = Mathf.Sqrt(3f);

            return new Triangle2D(
                center + new Vector2(radius * scale, radius),
                center + new Vector2(-radius * scale, radius),
                center + new Vector2(0f, -radius * 2f)
            );
        }

        /// <summary>
        /// 一時的に保持されてる三角形を解決する
        /// </summary>
        void SolveTempTriangles()
        {
            for (var i = 0; i < tempRemoving.Count; ++i)
            {
                triangles.Remove(tempRemoving[i]);
            }

            for (var i = 0; i < tempAdding.Count; ++i)
            {
                if (!temp.ContainsKey(tempAdding[i]))
                {
                    temp.Add(tempAdding[i], false);
                }
                else
                {
                    temp[tempAdding[i]] = true;
                }
            }
            foreach (var pair in temp)
            {
                if (!pair.Value)
                {
                    triangles.Add(pair.Key);
                }
            }
            temp.Clear();
            tempRemoving.Clear();
            tempAdding.Clear();
        }

        /// <summary>
        /// 追加用の一時保持配列に三角形を追加する
        /// </summary>
        /// <param name="t">T.</param>
        void AddTempAdding(Triangle2D t)
        {
            tempAdding.Add(t);
        }

        /// <summary>
        /// 三角形を指定した座標で3分割する
        /// </summary>
        /// <param name="t">T.</param>
        /// <param name="point">Point.</param>
        void SplitTriangle(Triangle2D t, Vector2 point)
        {
            AddTempAdding(new Triangle2D(point, t.p0, t.p1));
            AddTempAdding(new Triangle2D(point, t.p1, t.p2));
            AddTempAdding(new Triangle2D(point, t.p2, t.p0));
        }

        /// <summary>
        /// 内部で保持している三角形それぞれにactionを行う
        /// </summary>
        /// <param name="action">Action.</param>
        public void ForeachTriangles(System.Action<Triangle2D> action)
        {
            foreach (var t in triangles)
            {
                action(t);
            }
        }

        /// <summary>
        /// 外接三角形と共有する頂点を持つ三角形を取り除く
        /// Triangulate関数の後に呼び出すこと。
        /// </summary>
        void RemoveCommonPointWithHugeTriangle()
        {
            foreach (var t in triangles)
            {
                if (hugeTriangle.HasCommonPoints(t))
                {
                    tempRemoving.Add(t);
                }
            }

            SolveTempTriangles();
        }

        /// <summary>
        /// 入力された頂点群をポリゴンとし、その外部にある三角形を取り除く
        /// Triangulate関数の後に呼び出すこと。
        /// </summary>
        void RemoveExternalTriangles(List<Vector2> points)
        {
            ForeachTriangles((Triangle2D t) =>
            {
                if (!Utils2D.Pnpoly(points, t.g))
                {
                    tempRemoving.Add(t);
                }
            });

            SolveTempTriangles();
        }

        /// <summary>
        /// 面積が極端に小さな三角形を取り除く
        /// Triangulate関数の後に呼び出すこと。
        /// </summary>
        void RemoveSkinnyTriangles()
        {
            ForeachTriangles((Triangle2D t) =>
            {
                if (t.s <= 0f)
                {
                    tempRemoving.Add(t);
                }
            });

            SolveTempTriangles();
        }

        /// <summary>
        /// 頂点カラーを指定して三角形情報からメッシュに変換する。
        /// </summary>
        /// <returns>The mesh.</returns>
        /// <param name="color">Color.</param>
        public Mesh ToMesh(Color color)
        {
            return Utils2D.TrianglesToMesh(new List<Triangle2D>(triangles), color);
        }
    }
}