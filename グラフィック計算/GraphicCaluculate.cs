using graphicbox2d.オブジェクトマネージャー;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;
using static graphicbox2d.グラフィック計算.CalConvert;
using graphicbox2d.グローバル変数;

namespace graphicbox2d.グラフィック計算
{
    /// <summary>
    /// 図形の交差判定や色々な計算を行うクラス
    /// </summary>
    internal static class GraphicCaluculate
    {
        // ===============================================================================
        // 交差判定系関数
        // ===============================================================================

        /// <summary>
        /// 2つの線分の交点を計算します。（無限に延長した直線として交点を求めます）
        /// </summary>
        /// <param name="line1">交点を求めるLine2Dオブジェクト1。</param>
        /// <param name="line2">交点を求めるLine2Dオブジェクト2。</param>
        /// <returns>交点座標を表す <see cref="Vector2"/>。</returns>
        /// <remarks>
        /// 各線分を直線方程式に変換し、連立方程式を解くことで交点を算出します。  
        /// y = k1 × (x - a1) + b1  
        /// y = k2 × (x - a2) + b2  
        /// の解が交点となります。
        /// </remarks>
        public static Vector2 GetLineCrossPoint(Line2D line1, Line2D line2)
        {
            // 線分本体を表すベクトル
            Vector2 LineVector1;

            // 線分の始点座標ベクトル
            Vector2 LineStartPoint1;

            ConvertLine2DToVector2(line1, out LineStartPoint1, out LineVector1);

            // 線分本体を表すベクトル
            Vector2 LineVector2;

            // 線分の始点座標ベクトル
            Vector2 LineStartPoint2;

            ConvertLine2DToVector2(line2, out LineStartPoint2, out LineVector2);

            double k1 = LineVector1.Y / LineVector1.X;
            double a1 = LineStartPoint1.X;
            double b1 = LineStartPoint1.Y;

            double k2 = LineVector2.Y / LineVector2.X;
            double a2 = LineStartPoint2.X;
            double b2 = LineStartPoint2.Y;

            // この連立方程式の解が交点である。
            // y = k1×(x - a1) + b1
            // y = k2×(x - a2) + b2

            // 連立方程式の解
            double intersX = (k1 * a1 - k2 * a2 - b1 + b2) / (k1 - k2);
            double intersY = k1 * (intersX - a1) + b1;

            return new Vector2((float)intersX, (float)intersY);
        }

        /// <summary>
        /// ポリゴンと円の交差判定を行う関数。
        /// ポリゴンの頂点群と円の中心・半径を入力し、
        /// 円とポリゴンが交差しているかどうかを判定する。
        /// </summary>
        /// <param name="Points">
        /// ポリゴンを構成する頂点群（順序付き）。  
        /// 2点以下の場合は判定を行わず <c>false</c> を返す。
        /// </param>
        /// <param name="CircleCenter">円の中心座標</param>
        /// <param name="CircleR">円の半径</param>
        /// <param name="Ret">
        /// 判定結果を返す列挙値。以下の値を取りうる:
        /// <list type="bullet">
        ///   <item><description><c>None</c> : 交差していない</description></item>
        ///   <item><description><c>InPolygon</c> : 円の中心がポリゴン内に含まれる</description></item>
        ///   <item><description><c>InCircle</c> : ポリゴンの頂点が円の内側に含まれる</description></item>
        ///   <item><description><c>Cross</c> : 円とポリゴンの辺が交差している</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// 円とポリゴンが交差している場合は <c>true</c>、  
        /// 交差していない場合は <c>false</c> を返す。
        /// </returns>
        public static bool IsCrossPolygonAndCircle(in List<PointF> Points, in PointF CircleCenter, float CircleR, out eIsCrossPolygonAndCircle Ret)
        {
            if (Points.Count <= 2)
            {
                // ポリゴンの点が2点以下の場合、交差判定を行わない
                Ret = eIsCrossPolygonAndCircle.None;

                return false;
            }

            bool IsHit;

            // ------------------------------
            //　①円の中心座標がポリゴン内に含まれるか判定する
            // ------------------------------
            IsHit = IsContainPointInPolygon(Points, CircleCenter);

            if (IsHit == true)
            {
                // ポリゴン内に円の中心座標が含まれている場合、ヒットしていると判定
                Ret = eIsCrossPolygonAndCircle.InPolygon;

                return true;
            }

            // ------------------------------
            //　②ポリゴンの点が円の内側に存在しているか判定する
            // ------------------------------
            IsHit = IsContainPointsInCircle(CircleCenter, CircleR, Points);

            if (IsHit == true)
            {
                // ポリゴンの点が円の内側に存在している場合、ヒットしていると判定
                Ret = eIsCrossPolygonAndCircle.InCircle;

                return true;
            }

            // ------------------------------
            //　③ポリゴンの各辺と円が交差しているか判定
            // ------------------------------
            for (int i = 0; i < Points.Count; i++)
            {
                PointF pt1 = Points[i];

                int nextIndex = i + 1;

                if (i == Points.Count - 1)
                {
                    nextIndex = 0;
                }

                PointF pt2 = Points[nextIndex];

                bool IsCross = IsCrossLineAndCircle(pt1, pt2, CircleCenter, CircleR);

                if (IsCross == true)
                {
                    // ポリゴンの辺と円が交差している場合、ヒットしていると判定
                    Ret = eIsCrossPolygonAndCircle.Cross;

                    return true;
                }
            }

            // 交差していない
            Ret = eIsCrossPolygonAndCircle.None;

            return false;
        }

        /// <summary>
        /// ポリゴンの各辺と円の輪郭線が交差しているかどうかを判定します。
        /// 以下の条件を判定します:
        /// 1. ポリゴンの辺と円が交差している、または接している場合
        /// 2. ポリゴンの辺上に円の中心が存在する場合
        /// いずれにも該当しない場合は交差なしと判定します。
        /// </summary>
        /// <param name="Points">ポリゴンの頂点リスト（3点以上必要）</param>
        /// <param name="CircleCenter">円の中心座標</param>
        /// <param name="CircleR">円の半径</param>
        /// <param name="lineWidth">ポリゴン辺の線幅</param>
        /// <param name="Ret">
        /// 判定結果を返す列挙値。以下の値を取りうる:
        /// <list type="bullet">
        ///   <item><description><c>None</c> : 交差なし</description></item>
        ///   <item><description><c>Cross</c> : ポリゴンの辺と円が交差または接触</description></item>
        ///   <item><description><c>OnLine</c> : 円の中心がポリゴンの辺上にある</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// true : ポリゴンの辺と円が交差または接触している、または円の中心が辺上にある場合  
        /// false: 交差なしの場合
        /// </returns>
        public static bool IsCrossPolygonAndCircleLine(in List<PointF> Points, in PointF CircleCenter, float CircleR, float lineWidth, out eIsCrossPolygonAndCircleLine Ret)
        {
            if (Points.Count <= 2)
            {
                // ポリゴンの点が2点以下の場合、交差判定を行わない
                Ret = eIsCrossPolygonAndCircleLine.None;

                return false;
            }

            // ------------------------------
            //　ポリゴンの各辺と円が交差しているか判定
            // ------------------------------
            for (int i = 0; i < Points.Count; i++)
            {
                PointF pt1 = Points[i];

                int nextIndex = i + 1;

                if (i == Points.Count - 1)
                {
                    nextIndex = 0;
                }

                PointF pt2 = Points[nextIndex];

                eIsCrossLineAndCircle isCross;

                IsCrossLineAndCircle(pt1, pt2, lineWidth, CircleCenter, CircleR, out isCross);

                // ポリゴンの辺と円が交差しているまたは、辺と円が接している
                if (isCross == eIsCrossLineAndCircle.Cross || isCross == eIsCrossLineAndCircle.Contact)
                {
                    Ret = eIsCrossPolygonAndCircleLine.Cross;
                    return true;
                }
                // ポリゴンの辺上に円の中心がある
                else if (isCross == eIsCrossLineAndCircle.OnCenter)
                {
                    Ret = eIsCrossPolygonAndCircleLine.OnLine;
                    return true;
                }
            }

            // 交差していない
            Ret = eIsCrossPolygonAndCircleLine.None;

            return false;
        }

        /// <summary>
        /// 指定された曲線（PointFリストで構成される折れ線）と、
        /// 円（中心座標と半径、および線幅を考慮した円形領域）が交差しているかを判定します。
        /// </summary>
        /// <param name="Points">判定対象となる曲線の頂点座標リスト。2点以上必要です。</param>
        /// <param name="CircleCenter">円の中心座標。</param>
        /// <param name="CircleR">円の半径。</param>
        /// <param name="lineWidth">曲線の線幅。判定時に半径として考慮されます。</param>
        /// <param name="Ret">
        /// 判定結果を返す列挙値。以下の値を返します：
        /// <list type="bullet">
        ///   <item><description><c>None</c> - 交差していない</description></item>
        ///   <item><description><c>Cross</c> - 曲線と円が交差または接触している</description></item>
        ///   <item><description><c>OnLine</c> - 円の中心が曲線上に存在する</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// 曲線と円が交差または接触している場合は <c>true</c>、
        /// 交差していない場合は <c>false</c> を返します。
        /// </returns>
        public static bool IsCrossCurvePointsAndCircleLine(in List<PointF> Points, in PointF CircleCenter, float CircleR, float lineWidth, out eIsCrossPolygonAndCircleLine Ret)
        {
            if (Points.Count <= 2)
            {
                // ポリゴンの点が2点以下の場合、交差判定を行わない
                Ret = eIsCrossPolygonAndCircleLine.None;

                return false;
            }

            bool IsHit;

            // ------------------------------
            // ①線の太さ÷2を半径とした円に点が含まれているか判定
            // ------------------------------
            float OnLineCircleR = lineWidth / 2f;

            IsHit = IsContainPointsInCircle(CircleCenter, OnLineCircleR, Points);

            if (IsHit == true)
            {
                // ポリゴン内に円の中心座標が含まれている場合、ヒットしていると判定
                Ret = eIsCrossPolygonAndCircleLine.OnLine;
                return true;
            }

            // ------------------------------
            // ②マウスヒット範囲を半径とした円に点が含まれているか判定
            // ------------------------------
            IsHit = IsContainPointsInCircle(CircleCenter, OnLineCircleR, Points);

            if (IsHit == true)
            {
                // ポリゴン内に円の中心座標が含まれている場合、ヒットしていると判定
                Ret = eIsCrossPolygonAndCircleLine.Cross;
                return true;
            }

            // ------------------------------
            //　曲線の各辺と円が交差しているか判定
            // ------------------------------
            for (int i = 0; i < Points.Count-1; i++)
            {
                PointF pt1 = Points[i];

                int nextIndex = i + 1;

                PointF pt2 = Points[nextIndex];

                eIsCrossLineAndCircle isCross;

                IsCrossLineAndCircle(pt1, pt2, lineWidth, CircleCenter, CircleR, out isCross);

                // ポリゴンの辺と円が交差しているまたは、辺と円が接している
                if (isCross == eIsCrossLineAndCircle.Cross || isCross == eIsCrossLineAndCircle.Contact)
                {
                    Ret = eIsCrossPolygonAndCircleLine.Cross;
                    return true;
                }
                // ポリゴンの辺上に円の中心がある
                else if (isCross == eIsCrossLineAndCircle.OnCenter)
                {
                    Ret = eIsCrossPolygonAndCircleLine.OnLine;
                    return true;
                }
            }

            // 交差していない
            Ret = eIsCrossPolygonAndCircleLine.None;

            return false;
        }

        /// <summary>
        /// ポリゴンと円の交差判定を行う関数。
        /// ポリゴンの頂点群と円の中心・半径を入力し、
        /// 円とポリゴンが交差しているかどうかを判定する。
        /// </summary>
        /// <param name="Points">
        /// ポリゴンを構成する頂点群（順序付き）。  
        /// 2点以下の場合は判定を行わず <c>false</c> を返す。
        /// </param>
        /// <param name="CircleCenter">円の中心座標</param>
        /// <param name="CircleR">円の半径</param>
        /// <returns>
        /// 円とポリゴンが交差している場合は <c>true</c>、  
        /// 交差していない場合は <c>false</c> を返す。
        /// </returns>
        public static bool IsCrossPolygonAndCircle(in List<PointF> Points, in PointF CircleCenter, float CircleR)
        {
            eIsCrossPolygonAndCircle Dummy;
            return IsCrossPolygonAndCircle(Points, CircleCenter, CircleR, out Dummy);
        }

        /// <summary>
        /// 線分と円の交差判定を行う関数。
        /// 線分の始点・終点と円の中心・半径を入力し、
        /// 線分が円と交差するかどうかを判定する。
        /// </summary>
        /// <param name="lineStart">線分の始点座標</param>
        /// <param name="lineEnd">線分の終点座標</param>
        /// <param name="lineWidth">線分の太さ</param>
        /// <param name="CircleCenter">円の中心座標</param>
        /// <param name="CircleR">円の半径</param>
        /// <param name="Ret">
        /// 判定結果を返す列挙値。以下の値を取りうる:
        /// <list type="bullet">
        ///   <item><description><c>None</c> : 交差なし</description></item>
        ///   <item><description><c>OnCenter</c> : 円の中心が線分上にある</description></item>
        ///   <item><description><c>Contact</c> : 線分と円が接する（接点を持つ）</description></item>
        ///   <item><description><c>Cross</c> : 線分と円が交差する</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// 線分と円が交差している場合は <c>true</c>、
        /// 交差していない場合は <c>false</c> を返す。
        /// </returns>
        public static bool IsCrossLineAndCircle(in PointF lineStart, in PointF lineEnd, float lineWidth, in PointF CircleCenter, float CircleR, out eIsCrossLineAndCircle Ret)
        {
            // 判定結果を初期化
            Ret = eIsCrossLineAndCircle.None;

            // 線分の始点・終点をベクトル化
            Vector2 lineStartV = lineStart.ToVector2();
            Vector2 lineEndV = lineEnd.ToVector2();

            // 線分本体を表すベクトル
            Vector2 LineVector = lineEndV - lineStartV;

            // 線分と直行（直角に交わる）ベクトルを作成
            Vector2 LineVector2 = new Vector2(-1 * LineVector.Y, LineVector.X);

            // ベクトルの長さを円の半径と同じ値にする
            LineVector2 = CircleR * Vector2.Normalize(LineVector2);

            double k1;
            double k2;

            Vector2 CircleCenterV = CircleCenter.ToVector2();

            // ベクトルの交差係数を求める
            GetVectorCrossKeisu(lineStartV, LineVector, CircleCenterV, LineVector2, out k1, out k2);

            // 延長・縮小係数が1より大きい場合、線分と円は交差していない
            if (1 < Math.Abs(k1) || 1 < Math.Abs(k2))
            {
                return false;
            }

            // 線分の延長・縮小係数が0未満の場合、線分と円は交差していない
            if (k1 < 0)
            {
                return false;
            }

            // 交差の仕方を判定

            float dist;

            if (lineWidth == 0)
            {   // 線の太さが0の場合、判定距離を0にする
                // (数学的に、完全に誤差なく円の中心が線上にある）
                dist = 0;
            }
            else
            {
                // 円の中心から線への垂線の長さを求める
                dist = Math.Abs(Vector2.Dot(Vector2.Normalize(LineVector2), CircleCenterV - (lineStartV + (float)k1 * LineVector)));
            }

            // 円の中心から線への垂線の長さ線の太さ÷2以下である（線の太さの範囲内に円の中心がある）
            if (Comp.IsAtMost(dist, (float)lineWidth / 2f) == true)
            {
                // 円の中心が線上にある
                Ret = eIsCrossLineAndCircle.OnCenter;
            }
            // 円の中心から線への垂線の長さが円の半径と等しい
            else if (Comp.IsEqual(1, k2) == true)
            {
                Ret = eIsCrossLineAndCircle.Contact;
            }
            // それ以外は交差
            else
            {
                Ret = eIsCrossLineAndCircle.Cross;
            }

            return true;
        }

        /// <summary>
        /// 線分と円の交差判定を行う関数。
        /// 線分の始点・終点と円の中心・半径を入力し、
        /// 線分が円と交差するかどうかを判定する。
        /// </summary>
        /// <param name="lineStart">線分の始点座標</param>
        /// <param name="lineEnd">線分の終点座標</param>
        /// <param name="lineWidth">線分の太さ</param>
        /// <param name="CircleCenter">円の中心座標</param>
        /// <param name="CircleR">円の半径</param>
        /// <returns>
        /// 線分と円が交差している場合は <c>true</c>、
        /// 交差していない場合は <c>false</c> を返す。
        /// </returns>
        public static bool IsCrossLineAndCircle(in PointF lineStart, in PointF lineEnd, float lineWidth, in PointF CircleCenter, float CircleR)
        {
            eIsCrossLineAndCircle Dummy;
            return IsCrossLineAndCircle(lineStart, lineEnd, lineWidth, CircleCenter, CircleR, out Dummy);
        }

        /// <summary>
        /// 線分と円の交差判定を行う関数。
        /// 線分の始点・終点と円の中心・半径を入力し、
        /// 線分が円と交差するかどうかを判定する。
        /// </summary>
        /// <param name="lineStart">線分の始点座標</param>
        /// <param name="lineEnd">線分の終点座標</param>
        /// <param name="CircleCenter">円の中心座標</param>
        /// <param name="CircleR">円の半径</param>
        /// <returns>
        /// 線分と円が交差している場合は <c>true</c>、
        /// 交差していない場合は <c>false</c> を返す。
        /// </returns>
        public static bool IsCrossLineAndCircle(in PointF lineStart, in PointF lineEnd, in PointF CircleCenter, float CircleR)
        {
            float lineWidth = 0;
            return IsCrossLineAndCircle(lineStart, lineEnd, lineWidth, CircleCenter, CircleR);
        }

        /// <summary>
        /// 2つの円の交差判定を行う関数。  
        /// 円の中心座標と半径を入力し、円同士が交差しているかどうかを判定する。
        /// </summary>
        /// <param name="Circle1Center">円1の中心座標</param>
        /// <param name="R1">円1の半径</param>
        /// <param name="Circle2Center">円2の中心座標</param>
        /// <param name="R2">円2の半径</param>
        /// <param name="Ret">
        /// 判定結果を返す列挙値。以下の値を取りうる:
        /// <list type="bullet">
        ///   <item><description><c>None</c> : 交差していない</description></item>
        ///   <item><description><c>Cross</c> : 円同士が交差している</description></item>
        ///   <item><description><c>Circ2PtInCirc1</c> : 円2の中心が円1の内側に含まれる</description></item>
        ///   <item><description><c>Circ1PtInCirc2</c> : 円1の中心が円2の内側に含まれる</description></item>
        ///   <item><description><c>BothIn</c> : 両方の円の中心がお互いの円の内側に含まれる</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// 円同士が交差している場合は <c>true</c>、  
        /// 交差していない場合は <c>false</c> を返す。
        /// </returns>
        public static bool IsCrossCircles(PointF Circle1Center, float R1, PointF Circle2Center, float R2, out eIsCrossCircles Ret)
        {
            Vector2 Circle1CenterV = Circle1Center.ToVector2();
            Vector2 Circle2CenterV = Circle2Center.ToVector2();

            // 判定対象の円の半径の合計
            float SumR = R1 + R2;

            //2つの円の中心座標の距離を求める
            float dist = Vector2.Distance(Circle1CenterV, Circle2CenterV);

            // 両方の円の中心がお互いの円の内側に含まれている
            if (Comp.IsAtMost(dist, R1) == true && Comp.IsAtMost(dist, R2) == true)
            {
                Ret = eIsCrossCircles.BothIn;

                return true;
            }
            // 円2の中心が円1の内側に含まれている
            else if (Comp.IsAtMost(dist, R1) == true)
            {
                Ret = eIsCrossCircles.Circ2PtInCirc1;

                return true;
            }
            // 円1の中心が円2の内側に含まれている
            else if (Comp.IsAtMost(dist, R2) == true)
            {
                Ret = eIsCrossCircles.Circ1PtInCirc2;

                return true;
            }
            // 交差している
            else if (Comp.IsAtMost(dist, SumR) == true)
            {
                Ret = eIsCrossCircles.Cross;

                return true;
            }
            // 交差していない
            else
            {
                Ret = eIsCrossCircles.None;

                return false;
            }
        }

        /// <summary>
        /// 円弧と円の交差判定を行うメソッド。
        /// 与えられた円弧（中心座標・半径・開始角度・終了角度）と、
        /// 別の円（中心座標・半径）が交差しているかどうかを判定する。
        /// </summary>
        /// <param name="Circle1Center">円弧の中心座標。</param>
        /// <param name="R1">円弧の半径。</param>
        /// <param name="StartAngle">円弧の開始角度（度数法）。</param>
        /// <param name="EndAngle">円弧の終了角度（度数法）。</param>
        /// <param name="Circle2Center">判定対象となる円の中心座標。</param>
        /// <param name="R2">判定対象となる円の半径。</param>
        /// <param name="Ret">
        /// 判定結果を示す列挙値（eIsCrossArcAndCircle）。
        /// - ArcPtInCirc : 円弧の点が円の内部にある
        /// - CircPtInArc : 円の点が円弧の内部にある
        /// - Cross       : 円弧と円が交差している
        /// - None        : 交差なし
        /// </param>
        /// <returns>
        /// true : 交差または包含がある場合  
        /// false : 交差も包含もない場合
        /// </returns>
        /// <remarks>
        /// 判定手順:
        /// 1. 円弧の中心から円の中心へのベクトル角度を計算し、円弧範囲内か判定。
        /// 2. 範囲内なら通常の円同士の交差判定を行う。
        /// 3. 範囲外なら円弧の始点・終点を線分とみなし、円との交差判定を行う。
        /// </remarks>

        public static bool IsCrossArcAndCircle(PointF Circle1Center, float R1, float StartAngle, float EndAngle, PointF Circle2Center, float R2, out eIsCrossArcAndCircle Ret)
        {
            Vector2 Circle1CenterV = Circle1Center.ToVector2();
            Vector2 Circle2CenterV = Circle2Center.ToVector2();

            Vector2 Center1ToCenter2 = Circle2CenterV - Circle1CenterV;

            // 距離が２つの円の半径の和より大きい場合、交差しない
            float dist = Vector2.Distance(Circle1CenterV, Circle2CenterV);
            if (Comp.IsAtLeast(dist, R1 + R2) == true)
            {
                Ret = eIsCrossArcAndCircle.None;
                return false;
            }

            // 2つの円の中心座標がなす角度を求める
            float towRadian = (float)GetAngle(Center1ToCenter2);

            // 度数法に変換
            float towAngle = RadianToDegree(towRadian);

            // 中点１→中点２のベクトルが円弧の範囲内にあるか判定
            if (Comp.IsAtMost(StartAngle, towAngle) == true && Comp.IsAtMost(towAngle, EndAngle) == true)
            {
                // 範囲内なら、通常の円同士の交差判定を行う
                eIsCrossCircles RetCircles;
                IsCrossCircles(Circle1Center, R1, Circle2Center, R2, out RetCircles);

                switch(RetCircles)
                {
                    case eIsCrossCircles.Circ1PtInCirc2:
                        Ret = eIsCrossArcAndCircle.ArcPtInCirc;
                        return true;
                    case eIsCrossCircles.Circ2PtInCirc1:
                        Ret = eIsCrossArcAndCircle.CircPtInArc;
                        return true;
                    case eIsCrossCircles.Cross:
                        Ret = eIsCrossArcAndCircle.Cross;
                        return true;
                    default:
                        Ret = eIsCrossArcAndCircle.None;
                        return false;
                }
            }
            else
            {
                // 範囲外なら円弧の線分と円の交差判定を行う

                eIsCrossLineAndCircle RetLine;

                PointF ArcStartPt = new PointF(
                    Circle1Center.X + R1 * (float)Math.Cos(DegreeToRadian(StartAngle)),
                    Circle1Center.Y + R1 * (float)Math.Sin(DegreeToRadian(StartAngle))
                    );

                IsCrossLineAndCircle(Circle1Center, ArcStartPt, 0, Circle2Center, R2, out RetLine);

                switch (RetLine)
                {
                    case eIsCrossLineAndCircle.OnCenter:
                        Ret = eIsCrossArcAndCircle.CircPtInArc;
                        return true;
                    case eIsCrossLineAndCircle.Contact:
                    case eIsCrossLineAndCircle.Cross:
                        Ret = eIsCrossArcAndCircle.Cross;
                        return true;
                    default:
                        break;
                }

                PointF ArcEndPt = new PointF(
                    Circle1Center.X + R1 * (float)Math.Cos(DegreeToRadian(EndAngle)),
                    Circle1Center.Y + R1 * (float)Math.Sin(DegreeToRadian(EndAngle))
                    );


                IsCrossLineAndCircle(Circle1Center, ArcEndPt, 0, Circle2Center, R2, out RetLine);

                switch (RetLine)
                {
                    case eIsCrossLineAndCircle.OnCenter:
                        Ret = eIsCrossArcAndCircle.CircPtInArc;
                        return true;
                    case eIsCrossLineAndCircle.Contact:
                    case eIsCrossLineAndCircle.Cross:
                        Ret = eIsCrossArcAndCircle.Cross;
                        return true;
                    default:
                        break;
                }

                Ret = eIsCrossArcAndCircle.None;
                return false;
            }
        }

        /// <summary>
        /// 2つの円の輪郭線が交差しているかどうかを判定します。
        /// 以下の条件を判定します:
        /// 1. 円1の中心が円2の輪郭線上にある場合
        /// 2. 円2の中心が円1の輪郭線上にある場合
        /// 3. 円1と円2の輪郭線が交差している場合
        /// いずれにも該当しない場合は交差なしと判定します。
        /// </summary>
        /// <param name="Circle1Center">円1の中心座標</param>
        /// <param name="R1">円1の半径</param>
        /// <param name="Witdh1">円1の輪郭線の幅</param>
        /// <param name="Circle2Center">円2の中心座標</param>
        /// <param name="R2">円2の半径</param>
        /// <param name="Witdh2">円2の輪郭線の幅</param>
        /// <param name="Ret">
        /// 判定結果を返す列挙値:
        ///   Circ1PtOnCirc2 : 円1の中心が円2の輪郭線上
        ///   Circ2PtOnCirc1 : 円2の中心が円1の輪郭線上
        ///   Cross          : 2つの円の輪郭線が交差
        ///   None           : 交差なし
        /// </param>
        /// <returns>
        /// true : 交差または中心が相手の輪郭線上にある場合  
        /// false: 交差なしの場合
        /// </returns>
        public static bool IsCrossCirclesOutLine(PointF Circle1Center, float R1, float Witdh1, PointF Circle2Center, float R2, float Witdh2, out eIsCrossCirclesOutLine Ret)
        {
            Vector2 Circle1CenterV = Circle1Center.ToVector2();
            Vector2 Circle2CenterV = Circle2Center.ToVector2();

            //2つの円の中心座標の距離を求める
            float dist = Vector2.Distance(Circle1CenterV, Circle2CenterV);

            float RangeMin;
            float RangeMax;

            //円1の中心が円2の輪郭線上にあるか判定
            RangeMin = R2 - Witdh2;
            RangeMax = R2 + Witdh2;

            if (Comp.IsAtMost(RangeMin, dist) == true && Comp.IsAtMost(dist, RangeMax) == true)
            {
                Ret = eIsCrossCirclesOutLine.Circ1PtOnCirc2;
                return true;
            }

            // 円2の中心が円1の輪郭線上にあるか判定
            RangeMin = R1 - Witdh1;
            RangeMax = R1 + Witdh1;
            if (Comp.IsAtMost(RangeMin, dist) == true && Comp.IsAtMost(dist, RangeMax) == true)
            {
                Ret = eIsCrossCirclesOutLine.Circ2PtOnCirc1;
                return true;
            }

            // 円の輪郭線が交差しているかの判定
            RangeMin = Math.Abs(R1 - R2);
            RangeMax = R1 + R2;

            if (Comp.IsAtMost(RangeMin, dist) == true && Comp.IsAtMost(dist, RangeMax) == true)
            {
                Ret = eIsCrossCirclesOutLine.Cross;
                return true;
            }

            Ret = eIsCrossCirclesOutLine.None;
            return false;
        }

        /// <summary>
        /// 円弧のアウトラインと円の交差判定を行うメソッド。
        /// 与えられた円弧（中心座標・半径・開始角度・終了角度・線幅）と、
        /// 別の円（中心座標・半径）が交差しているかどうかを判定する。
        /// </summary>
        /// <param name="Circle1Center">円弧の中心座標。</param>
        /// <param name="R1">円弧の半径。</param>
        /// <param name="StartAngle">円弧の開始角度（度数法）。</param>
        /// <param name="EndAngle">円弧の終了角度（度数法）。</param>
        /// <param name="LineWidth">円弧の線幅。</param>
        /// <param name="IsDrawSideLines">円弧の側面線を描画するかどうかのフラグ。</param>
        /// <param name="Circle2Center">判定対象となる円の中心座標。</param>
        /// <param name="R2">判定対象となる円の半径。</param>
        /// <param name="Ret">
        /// 判定結果を示す列挙値（eIsCrossArcAndCircleOutLine）。
        /// - ArcPtOnCirc : 円弧の点が円の外周上にある  
        /// - CircPtOnArc : 円の点が円弧の外周上にある  
        /// - Cross       : 円弧と円が交差している  
        /// - None        : 交差なし  
        /// </param>
        /// <returns>
        /// true : 交差または接触がある場合  
        /// false : 交差も接触もない場合
        /// </returns>
        /// <remarks>
        /// 判定手順:
        /// 1. 円弧の中心から円の中心へのベクトル角度を計算し、円弧範囲内か判定。  
        /// 2. 範囲内なら円同士のアウトライン交差判定を行う。  
        /// 3. 範囲外なら円弧の始点・終点を線分とみなし、円との交差判定を行う。  
        /// </remarks>

        public static bool IsCrossArcAndCircleOutLine(PointF Circle1Center, float R1, float StartAngle, float EndAngle, float LineWidth, bool IsDrawSideLines,  PointF Circle2Center, float R2, out eIsCrossArcAndCircleOutLine Ret)
        {
            Vector2 Circle1CenterV = Circle1Center.ToVector2();
            Vector2 Circle2CenterV = Circle2Center.ToVector2();


            Vector2 Center1ToCenter2 = Circle2CenterV - Circle1CenterV;

            // 距離が２つの円の半径の和より大きい場合、交差しない
            float dist = Vector2.Distance(Circle1CenterV, Circle2CenterV);
            if (Comp.IsAtLeast(dist, R1 + R2) == true)
            {
                Ret = eIsCrossArcAndCircleOutLine.None;
                return false;
            }

            // 2つの円の中心座標がなす角度を求める
            float towRadian = (float)GetAngle(Center1ToCenter2);

            // 度数法に変換
            float towAngle = RadianToDegree(towRadian);

            // 中点１→中点２のベクトルが円弧の範囲内にあるか判定
            if (Comp.IsAtMost(StartAngle, towAngle) == true && Comp.IsAtMost(towAngle, EndAngle) == true)
            {
                // 範囲内なら、通常の円同士の交差判定を行う
                eIsCrossCirclesOutLine RetCircles;
                IsCrossCirclesOutLine(Circle1Center, R1, LineWidth, Circle2Center, R2, 0, out RetCircles);

                switch (RetCircles)
                {
                    case eIsCrossCirclesOutLine.Circ1PtOnCirc2:
                        Ret = eIsCrossArcAndCircleOutLine.ArcPtOnCirc;
                        return true;
                    case eIsCrossCirclesOutLine.Circ2PtOnCirc1:
                        Ret = eIsCrossArcAndCircleOutLine.CircPtOnArc;
                        return true;
                    case eIsCrossCirclesOutLine.Cross:
                        Ret = eIsCrossArcAndCircleOutLine.Cross;
                        return true;
                }
            }

            // 円弧のサイドラインも描画していない場合処理を終了
            if (IsDrawSideLines == false)
            {
                Ret = eIsCrossArcAndCircleOutLine.None;
                return false;
            }

            // 範囲内で交差しなかった、または範囲外なら円弧のサイドライン線分と円の交差判定を行う

            eIsCrossLineAndCircle RetLine;

            PointF ArcStartPt = new PointF(
                Circle1Center.X + R1 * (float)Math.Cos(DegreeToRadian(StartAngle)),
                Circle1Center.Y + R1 * (float)Math.Sin(DegreeToRadian(StartAngle))
                );

            IsCrossLineAndCircle(Circle1Center, ArcStartPt, LineWidth, Circle2Center, R2, out RetLine);

            switch (RetLine)
            {
                case eIsCrossLineAndCircle.OnCenter:
                    Ret = eIsCrossArcAndCircleOutLine.CircPtOnArc;
                    return true;
                case eIsCrossLineAndCircle.Contact:
                case eIsCrossLineAndCircle.Cross:
                    Ret = eIsCrossArcAndCircleOutLine.Cross;
                    return true;
                default:
                    break;
            }

            PointF ArcEndPt = new PointF(
                Circle1Center.X + R1 * (float)Math.Cos(DegreeToRadian(EndAngle)),
                Circle1Center.Y + R1 * (float)Math.Sin(DegreeToRadian(EndAngle))
                );


            IsCrossLineAndCircle(Circle1Center, ArcEndPt, LineWidth, Circle2Center, R2, out RetLine);

            switch (RetLine)
            {
                case eIsCrossLineAndCircle.OnCenter:
                    Ret = eIsCrossArcAndCircleOutLine.CircPtOnArc;
                    return true;
                case eIsCrossLineAndCircle.Contact:
                case eIsCrossLineAndCircle.Cross:
                    Ret = eIsCrossArcAndCircleOutLine.Cross;
                    return true;
                default:
                    break;
            }

            Ret = eIsCrossArcAndCircleOutLine.None;
            return false;
        }

        /// <summary>
        /// 2つの円の交差判定を行う関数。  
        /// 円の中心座標と半径を入力し、円同士が交差しているかどうかを判定する。
        /// </summary>
        /// <param name="Circle1Center">円1の中心座標</param>
        /// <param name="R1">円1の半径</param>
        /// <param name="Circle2Center">円2の中心座標</param>
        /// <param name="R2">円2の半径</param>
        /// <returns>
        /// 円同士が交差している場合は <c>true</c>、  
        /// 交差していない場合は <c>false</c> を返す。
        /// </returns>
        public static bool IsCrossCircles(PointF Circle1Center, float R1, PointF Circle2Center, float R2)
        {
            eIsCrossCircles Dummy;
            return IsCrossCircles(Circle1Center, R1, Circle2Center, R2, out Dummy);
        }

        // ===============================================================================
        // 内包判定系関数
        // ===============================================================================

        /// <summary>
        /// 点がポリゴン内に含まれるか判定する
        /// </summary>
        /// <param name="Points">ポリゴン頂点リスト</param>
        /// <param name="point">点</param>
        /// <returns>true:含む false:含まない</returns>
        public static bool IsContainPointInPolygon(in List<PointF> Points, in PointF point)
        {
            return Ray_Casting(in Points, in point);
        }

        /// <summary>
        /// レイ-キャスティング法で点がポリゴン内に含まれるか判定する
        /// 判定を高速化するためのアルゴリズムである。
        /// 完全に速度重視のため、非常に可読性は低い
        /// アルゴリズムの詳細は以下を参照
        /// https://ページ作成中
        /// </summary>
        /// <param name="Points">ポリゴン</param>
        /// <param name="point">点</param>
        /// <returns>true;交差 false:交差していない</returns>
        private static bool Ray_Casting(in List<PointF> Points, in PointF point)
        {
            float y = point.Y;

            // 交差フラグ（交差点数が偶数なら交差していない、奇数なら交差している）
            bool IsCross = false;

            for (int i = 0; i < Points.Count; i++)
            {
                PointF pt1 = Points[i];

                int nextIndex = i + 1;

                if (i == Points.Count - 1)
                {
                    nextIndex = 0;
                }

                PointF pt2 = Points[nextIndex];

                // 線分のY座標範囲内にポイントのY座標があるか確認
                // 線の始点 < ポイントのY座標 < 線の終点
                //   または
                // 線の始点 > ポイントのY座標 > 線の終点
                // を判定する。これを徹底的に高速化した結果
                // 判定処理が以下の式になる。（めっちゃわかりにくい）
                // if ((pt1.Y > y) != (pt2.Y > y))
                if (pt1.Y > y != pt2.Y > y)
                {
                    float intersectX;

                    if (Comp.IsEqual(pt2.X - pt1.X, 0) == true)
                    {
                        // 線分が垂直の場合、交点のX座標は線分のX座標と同じ
                        intersectX = pt1.X;
                    }
                    else
                    {
                        // 線分と水平線の交点のX座標を計算
                        // 線分の方程式を作成する。
                        float k = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);

                        // ポイントのy座標 = k ×（x - 線分始点x座標） + 線分始点y座標
                        // この方程式の解が交点のX座標になる。
                        intersectX = (point.Y + k * pt1.X - pt1.Y) / k;
                    }

                    // 交点がポイントのX座標より右側にある場合、交差していると判定
                    if (intersectX > point.X)
                    {
                        // 交差フラグを反転
                        IsCross = !IsCross;
                    }
                }
            }

            return IsCross;
        }

        /// <summary>
        /// 指定された点群のいずれかが円の内部に含まれているかを判定します。
        /// </summary>
        /// <param name="CircleCenter">円の中心座標。</param>
        /// <param name="CircleR">円の半径。</param>
        /// <param name="Points">判定対象の点群。</param>
        /// <returns>
        /// true: 少なくとも1点が円の内部に含まれている場合  
        /// false: すべての点が円の外側にある場合
        /// </returns>
        /// <remarks>
        /// 各点と円の中心との距離を計算し、半径未満であれば「円内」と判定します。
        /// </remarks>
        public static bool IsContainPointsInCircle(PointF CircleCenter, float CircleR, in List<PointF> Points)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                PointF pt = Points[i];

                float dist = Vector2.Distance(new Vector2(CircleCenter.X, CircleCenter.Y), new Vector2(pt.X, pt.Y));

                if (dist < CircleR)
                {
                    return true;
                }
            }

            return false;
        }



        // ===============================================================================
        // その他幾何学計算系関数
        // ===============================================================================

        /// <summary>
        /// 指定された点が線分（始点→終点の方向）に対して左側・右側・線上のどこに位置するかを判定します。
        /// </summary>
        /// <param name="line">判定対象の線分。</param>
        /// <param name="point">判定対象の点。</param>
        /// <returns>
        /// <see cref="LineLR.Left"/> : 点が線分の左側にある場合  
        /// <see cref="LineLR.Right"/> : 点が線分の右側にある場合  
        /// <see cref="LineLR.OnVector"/> : 点が線分上にある場合
        /// </returns>
        /// <remarks>
        /// 始点から終点へのベクトルと、始点から判定点へのベクトルの角度を計算して判定します。
        /// </remarks>
        public static LineLR GetLineLR(Line2D line, Point2D point)
        {
            Vector2 StartPoint = line.Start.ToVector2();

            Vector2 EndPoint_A = line.End.ToVector2();

            Vector2 EndPoint_B = new Vector2 (point.X, point.Y);

            double Angle = GetAngle(StartPoint, EndPoint_A, EndPoint_B);

            if (Comp.IsEqual(0, Angle) == true || Comp.IsEqual(Math.PI, Angle) == true)
            {
                return LineLR.OnVector;
            }
            else if (0 < Angle && Angle <= Math.PI)
            {
                return LineLR.Left;
            }
            else
            {
                return LineLR.Right;
            }
        }

        /// <summary>
        /// ポリゴンを指定した倍率でスケーリングします。
        /// </summary>
        /// <param name="Points">スケーリング対象のポリゴン頂点群。</param>
        /// <param name="PolygonCenterPoint">ポリゴンの中心座標。</param>
        /// <param name="scale">スケーリング倍率。1.0f で等倍、2.0f で2倍拡大。</param>
        /// <param name="ScalePoints">スケーリング後の頂点群を返す出力パラメータ。</param>
        /// <remarks>
        /// 各頂点を中心点からのベクトルとして扱い、スケーリングを適用した後に中心座標を基準に戻します。  
        /// これにより、ポリゴン全体が中心点を基準に拡縮されます。
        /// </remarks>
        public static void GetScalingPolygon(in List<PointF> Points, in PointF PolygonCenterPoint, float scale, out List<PointF> ScalePoints)
        {
            ScalePoints = new List<PointF>();

            // 各頂点を中心点からのベクトルとして扱い、スケーリングを適用
            for (int i = 0; i < Points.Count; i++)
            {
                // 中心座標を図形の中心に移動させる
                Vector2 offsetPoint = Points[i].ToVector2() - PolygonCenterPoint.ToVector2();

                // スケーリングを実行して、中心座標を元に戻す
                Vector2 SacledPoint = scale * offsetPoint + PolygonCenterPoint.ToVector2();

                // リストに格納
                ScalePoints.Add(SacledPoint.ToPointF());
            }
        }

        /// <summary>
        /// マウスポイントに最も近いオブジェクトを取得する関数。
        /// </summary>
        /// <param name="HitOnObjects">オブジェクトリスト</param>
        /// <param name="X">マウスX座標</param>
        /// <param name="Y">マウスY座標</param>
        /// <returns>マウスポイントに最も近いオブジェクト</returns>
        public static Object2D GetNearestMousePointObjest(in List<Object2D> HitOnObjects, float X, float Y)
        {
            // 複数ヒットしている場合は、最も近いオブジェクトを選択する
            float MinDistance = float.MaxValue;
            Object2D NearestObject = null;

            foreach (Object2D obj in HitOnObjects)
            {
                float Distance = obj.GetDistanceHitMousePoint(X, Y);

                if (Distance < MinDistance)
                {
                    MinDistance = Distance;
                    NearestObject = obj;
                }
            }
            return NearestObject;
        }

        /// <summary>
        /// ベクトルAとベクトルBがどれくらい延長・縮小した場所で交差するかを求める
        /// </summary>
        /// <param name="a_0">ベクトルAの始点座標</param>
        /// <param name="a">ベクトルA</param>
        /// <param name="b_0">ベクトルBの始点座標</param>
        /// <param name="b">ベクトルB</param>
        /// <param name="k_a">ベクトルAの延長・縮小係数</param>
        /// <param name="k_b">ベクトルBの延長・縮小係数</param>
        /// <remarks>
        /// ■ 延長縮小係数 ■
        /// 0          : ベクトルの始点で交差している
        /// 0～1       : ベクトルの線分上で交差している
        /// 1          : ベクトルの終点で交差している
        /// 1より大きい: ベクトルを延長した先で交差している
        /// マイナス   : ベクトルを逆方向に延長した先で交差している
        /// </remarks>
        /// <returns>true:正常終了 false:異常終了</returns>
        public static bool GetVectorCrossKeisu(in Vector2 a_0, in Vector2 a, in Vector2 b_0, in Vector2 b, out double k_a, out double k_b)
        {
            k_a = 0;
            k_b = 0;

            if (a.X * b.Y - a.Y * b.X == 0)
            {
                return false;
            }

            k_a = ((b_0.X - a_0.X) * b.Y - (b_0.Y - a_0.Y) * b.X) / (a.X * b.Y - a.Y * b.X);
            k_b = ((a_0.X - b_0.X) * a.Y - (a_0.Y - b_0.Y) * a.X) / (b.X * a.Y - b.Y * a.X);

            return true;
        }

        /// <summary>
        /// 2つのベクトル間の角度を求める
        /// </summary>
        /// <param name="StartPoint">共通の始点</param>
        /// <param name="End_A">ベクトルA</param>
        /// <param name="End_B">ベクトルB</param>
        /// <returns>角度（弧度法）</returns>
        public static double GetAngle(Vector2 StartPoint, Vector2 End_A, Vector2 End_B)
        {
            Vector2 A = End_A - StartPoint;
            Vector2 B = End_B - StartPoint;

            return GetAngle(A, B);
        }

        /// <summary>
        /// 2つのベクトル間の角度を求める
        /// </summary>
        /// <param name="a">ベクトルA</param>
        /// <param name="b">ベクトルB</param>
        /// <returns>角度（弧度法）</returns>
        public static double GetAngle(Vector2 a, Vector2 b)
        {
            float dot = Vector2.Dot(a, b);
            float magA = a.Length();
            float magB = b.Length();

            if (magA == 0 || magB == 0)
            {
                return double.NaN;
            }

            double cosTheta = dot / (magA * magB);

            double angleRad = Math.Acos(cosTheta);

            if (a.Y < 0)
            {
                angleRad = 2 * Math.PI - angleRad;
            }

            return angleRad;
        }

        /// <summary>
        /// ベクトルとX軸のなす角度を求める
        /// </summary>
        /// <param name="a">ベクトル</param>
        /// <returns>角度（弧度法）</returns>
        public static double GetAngle(Vector2 a)
        {

            double cosTheta = Vector2.Normalize(a).X;

            double angleRad = Math.Acos(cosTheta);

            if (a.Y < 0)
            {
                angleRad = 2 * Math.PI - angleRad;
            }

            return angleRad;
        }

        /// <summary>
        /// ポリゴンの中心点を計算
        /// </summary>
        /// <param name="Points">ポリゴン座標</param>
        /// <returns>中心点</returns>
        public static Vector2 CaluculateCenterPoint(PointF[] Points)
        {
            return CaluculateCenterPoint(Points.ToList());
        }

        /// <summary>
        /// ポリゴンの中心点を計算
        /// </summary>
        /// <param name="Points">ポリゴン座標</param>
        /// <returns>中心点</returns>
        public static Vector2 CaluculateCenterPoint(List<PointF> Points)
        {
            if (Points.Count == 0)
            {
                return new Vector2(float.NaN, float.NaN);
            }

            // 中心点を計算する
            Vector2 AllSumVec = new Vector2();

            foreach (var pt in Points)
            {
                AllSumVec.X += pt.X;
                AllSumVec.Y += pt.Y;
            }

            return AllSumVec / Points.Count;
        }

        /// <summary>
        /// ポリゴンの中心点と外接円の半径を同時に取得する
        /// </summary>
        /// <param name="Points">ポリゴン頂点配列</param>
        /// <param name="centerPoint">ポリゴンの中心点</param>
        /// <param name="circumCircleR">外接円の半径</param>
        internal static void GetCenterPointAndCircumCircleR(PointF[] Points, out Vector2 centerPoint, out float circumCircleR)
        {
            centerPoint = CaluculateCenterPoint(Points);
            circumCircleR = CaluculateCircumCircleR(centerPoint, Points);
        }

        /// <summary>
        /// ポリゴンの中心点と外接円の半径を同時に取得する
        /// </summary>
        /// <param name="Points">ポリゴン頂点リスト</param>
        /// <param name="centerPoint">ポリゴンの中心点</param>
        /// <param name="circumCircleR">外接円の半径</param>
        internal static void GetCenterPointAndCircumCircleR(List<PointF> Points, out Vector2 centerPoint, out float circumCircleR)
        {
            centerPoint = CaluculateCenterPoint(Points);
            circumCircleR = CaluculateCircumCircleR(centerPoint, Points);
        }

        /// <summary>
        /// ポリゴンの外接円の半径を計算
        /// </summary>
        /// <param name="PolygonCenterPoint">ポリゴンの中心</param>
        /// <param name="Points">ポリゴン座標</param>
        /// <returns>外接円の半径</returns>
        public static float CaluculateCircumCircleR(Vector2 PolygonCenterPoint, PointF[] Points)
        {
            return  CaluculateCircumCircleR(PolygonCenterPoint, Points.ToList());
        }

        /// <summary>
        /// ポリゴンの外接円の半径を計算
        /// </summary>
        /// <param name="PolygonCenterPoint">ポリゴンの中心</param>
        /// <param name="Points">ポリゴン座標</param>
        /// <returns>外接円の半径</returns>
        public static float CaluculateCircumCircleR(Vector2 PolygonCenterPoint, List<PointF> Points)
        {
            // 中心座標から最も遠くに離れているポイントを取得する
            float MaxDistance = 0F;

            if (Points.Count == 0)
            {
                return 0;
            }

            foreach (PointF point in Points)
            {
                float distance = Vector2.Distance(PolygonCenterPoint, point.ToVector2());

                if (MaxDistance < distance)
                {
                    MaxDistance = distance;
                }
            }

            return MaxDistance;
        }

        // ===============================================================================
        // 数式計算関数
        // ===============================================================================

        /// <summary>
        /// 数式を計算して、指定範囲の点群を取得する非同期メソッド。
        /// </summary>
        /// <param name="Susiki"></param>
        /// <param name="StartX"></param>
        /// <param name="EndX"></param>
        /// <param name="CalculateInterval"></param>
        /// <param name="ObjectID"></param>
        /// <returns></returns>
        public static async Task<PointF[]> SusikiCaluculate(string Susiki, double StartX, double EndX, double CalculateInterval, int ObjectID = int.MinValue)
        {
            PointF[] points =  await SusikiCalculater.Caluculate(Susiki, StartX, EndX, CalculateInterval);

            return points;
        }
    }
}
