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

namespace graphicbox2d
{
    /// <summary>
    /// 図形の交差判定や色々な計算を行うクラス
    /// </summary>
    internal static class GraphicCaluculate
    {
        /// <summary>
        /// グラフィック2Dコントロールオブジェクト
        /// </summary>
        public static Graphic2DControl Graphic2DControl { get; set; }

        /// <summary>
        /// グラフィック描画エンジンオブジェクト
        /// </summary>
        public static GraphicDrawEngine GraphicDrawEngine {get; set; }

        /// <summary>
        /// 線図形のバウンディングボックスの幅
        /// </summary>
        private const int LINE_BOUNDING_BOX_WITDH = 4; 

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
            float towAngle = GraphicCaluculate.RadianToDegree(towRadian);

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
                    Circle1Center.X + R1 * (float)Math.Cos(GraphicCaluculate.DegreeToRadian(StartAngle)),
                    Circle1Center.Y + R1 * (float)Math.Sin(GraphicCaluculate.DegreeToRadian(StartAngle))
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
                    Circle1Center.X + R1 * (float)Math.Cos(GraphicCaluculate.DegreeToRadian(EndAngle)),
                    Circle1Center.Y + R1 * (float)Math.Sin(GraphicCaluculate.DegreeToRadian(EndAngle))
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
            float towAngle = GraphicCaluculate.RadianToDegree(towRadian);

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
                Circle1Center.X + R1 * (float)Math.Cos(GraphicCaluculate.DegreeToRadian(StartAngle)),
                Circle1Center.Y + R1 * (float)Math.Sin(GraphicCaluculate.DegreeToRadian(StartAngle))
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
                Circle1Center.X + R1 * (float)Math.Cos(GraphicCaluculate.DegreeToRadian(EndAngle)),
                Circle1Center.Y + R1 * (float)Math.Sin(GraphicCaluculate.DegreeToRadian(EndAngle))
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
        /// <param name="polygon">ポリゴン</param>
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
                if ((pt1.Y > y) != (pt2.Y > y))
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
        // マウスヒット判定系関数
        // ===============================================================================

        /// <summary>
        /// マウス座標が指定されたポリゴンにヒットしているかを判定します。
        /// </summary>
        /// <param name="PolygonCenterPoint">ポリゴンの外接円の中心座標。</param>
        /// <param name="CircleR">ポリゴンの外接円の半径。</param>
        /// <param name="Points">ポリゴンを構成する頂点座標のリスト。</param>
        /// <param name="MousePoint">判定対象のマウス座標。</param>
        /// <param name="MouseHitRange">マウスのヒット判定範囲（半径）。</param>
        /// <returns>
        /// ヒット判定結果を <see cref="eMouseHitType"/> で返します。  
        /// None: ヒットなし  
        /// MousePointOnObject: マウスポイントがポリゴン内部にある  
        /// CrossMouseRange: ポリゴンの頂点が範囲内に含まれる、または交差している
        /// </returns>
        /// <remarks>
        /// まず外接円とマウスの交差判定を行い、ヒットしていない場合は処理を終了します。  
        /// 外接円とヒットしている場合のみ、ポリゴン本体との詳細な交差判定を行います。
        /// </remarks>
        public static eMouseHitType IsHitMouseRangeFillPolygon(in PointF PolygonCenterPoint, float CircleR, in List<PointF> Points, in PointF MousePoint, float MouseHitRange)
        {
            bool IsHit;

            //------------------------------
            //　①外接円判定
            //------------------------------

            // ポリゴンの外接円とマウスがヒットしているか判定
            IsHit = IsCrossCircles(PolygonCenterPoint, CircleR, MousePoint, MouseHitRange);

            if (IsHit == false)
            {
                // 外接円とマウスがヒットしていなければ、ポリゴン本体もヒットしていないので処理終了
                return eMouseHitType.None;
            }

            // ------------------------------
            // ②外接円とマウスがヒットしている場合、ポリゴン本体とマウスがヒットしているか厳密に判定する
            // ------------------------------
            eIsCrossPolygonAndCircle Ret;

            IsCrossPolygonAndCircle(Points, MousePoint, MouseHitRange, out Ret);

            eMouseHitType hitType;

            switch (Ret)
            {
                // 交差していない
                case eIsCrossPolygonAndCircle.None:
                    hitType = eMouseHitType.None;
                    break;
                // ポリゴン内にマウスがある
                case eIsCrossPolygonAndCircle.InPolygon:
                    hitType = eMouseHitType.MousePointOnObject;
                    break;
                // ポリゴンの頂点がマウスの内側に含まれている
                case eIsCrossPolygonAndCircle.InCircle:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // 接している、または交差している
                case eIsCrossPolygonAndCircle.Cross:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // 想定外
                default:
                    hitType = eMouseHitType.None;
                    break;
            }

            return hitType;
        }

        /// <summary>
        /// ポリゴンの外接円および輪郭線とマウス位置の当たり判定を行う
        /// </summary>
        /// <param name="PolygonCenterPoint">ポリゴンの中心座標</param>
        /// <param name="CircleR">ポリゴン外接円の半径</param>
        /// <param name="Points">ポリゴンを構成する頂点リスト</param>
        /// <param name="lineWidth">ポリゴン輪郭線の太さ</param>
        /// <param name="MousePoint">マウス座標</param>
        /// <param name="MouseHitRange">マウスの当たり判定範囲（半径）</param>
        /// <param name="IsConvertLineWidth">線の太さをグリッド座標サイズに変換するかどうかのフラグ。デフォルトは true。</param>
        /// <returns>
        /// eMouseHitType の判定結果：  
        /// ・None : ヒットしていない  
        /// ・MousePointOnObject : マウスがポリゴンの輪郭線上にある  
        /// ・CrossMouseRange : マウスの当たり判定円がポリゴン輪郭線と交差している  
        /// </returns>

        public static eMouseHitType IsHitMouseRangeLinePolygon(in PointF PolygonCenterPoint, float CircleR, in List<PointF> Points, float lineWidth, in PointF MousePoint, float MouseHitRange)
        {
            bool IsHit;

            // 線幅をグリッド座標サイズに変換
            lineWidth = (float)lineWidth / (float)Graphic2DControl.DisplayGridWidth;

            //------------------------------
            //　①外接円判定
            //------------------------------

            // ポリゴンの外接円とマウスがヒットしているか判定
            IsHit = IsCrossCircles(PolygonCenterPoint, CircleR, MousePoint, MouseHitRange);

            if (IsHit == false)
            {
                // 外接円とマウスがヒットしていなければ、ポリゴン本体もヒットしていないので処理終了
                return eMouseHitType.None;
            }

            // ------------------------------
            // ②外接円とマウスがヒットしている場合、ポリゴン本体とマウスがヒットしているか厳密に判定する
            // ------------------------------
            eIsCrossPolygonAndCircleLine Ret;

            IsCrossPolygonAndCircleLine(Points, MousePoint, MouseHitRange, lineWidth, out Ret);

            eMouseHitType hitType;

            switch (Ret)
            {
                // 交差していない
                case eIsCrossPolygonAndCircleLine.None:
                    hitType = eMouseHitType.None;
                    break;
                // ポリゴンの輪郭線上にマウスがある
                case eIsCrossPolygonAndCircleLine.OnLine:
                    hitType = eMouseHitType.MousePointOnObject;
                    break;
                // 接している、または交差している
                case eIsCrossPolygonAndCircleLine.Cross:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // 想定外
                default:
                    hitType = eMouseHitType.None;
                    break;
            }

            return hitType;
        }

        /// <summary>
        /// マウス座標が指定された線分にヒットしているかを判定します。
        /// </summary>
        /// <param name="lineStart">線分の始点座標。</param>
        /// <param name="lineEnd">線分の終点座標。</param>
        /// <param name="lineWidth">線分の太さ（ピクセル単位）。</param>
        /// <param name="MousePoint">判定対象のマウス座標。</param>
        /// <param name="MouseHitRange">マウスのヒット判定範囲（半径）。</param>
        /// <returns>
        /// ヒット判定結果を <see cref="eMouseHitType"/> で返します。  
        /// None: ヒットなし  
        /// MousePointOnObject: 線上にマウスがある  
        /// CrossMouseRange: 接触または交差している
        /// </returns>
        /// <remarks>
        /// 内部的には線分と円の交差判定を行い、その結果をマウスヒット種別に変換します。
        /// </remarks>
        public static eMouseHitType IsHitMouseRangeLine(in PointF lineStart, in PointF lineEnd, float lineWidth, in PointF MousePoint, float MouseHitRange)
        {
            // 線幅をグリッド座標サイズに変換
            lineWidth = (float)lineWidth / (float)Graphic2DControl.DisplayGridWidth;

            eIsCrossLineAndCircle Ret;

            // 線とマウスの交差判定を行う
            IsCrossLineAndCircle(lineStart, lineEnd, lineWidth, MousePoint, MouseHitRange, out Ret);

            eMouseHitType hitType;

            switch (Ret)
            {
                // 交差していない
                case eIsCrossLineAndCircle.None:
                    hitType = eMouseHitType.None;
                    break;
                // 線上にマウスがある
                case eIsCrossLineAndCircle.OnCenter:
                    hitType = eMouseHitType.MousePointOnObject;
                    break;
                // 接している、または交差している
                case eIsCrossLineAndCircle.Contact:
                case eIsCrossLineAndCircle.Cross:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // 想定外
                default:
                    hitType = eMouseHitType.None;
                    break;
            }

            return hitType;
        }

        /// <summary>
        /// マウス座標が指定された円（塗りつぶしあり）にヒットしているかを判定します。
        /// </summary>
        /// <param name="x">円の中心座標 X。</param>
        /// <param name="y">円の中心座標 Y。</param>
        /// <param name="r">円の半径。</param>
        /// <param name="MousePoint">判定対象のマウス座標。</param>
        /// <param name="MouseHitRange">マウスのヒット判定範囲（半径）。</param>
        /// <returns>
        /// ヒット判定結果を <see cref="eMouseHitType"/> で返します。  
        /// None: ヒットなし  
        /// CrossMouseRange: 接触または交差している  
        /// MousePointOnObject: マウスポイントが円の内側に含まれている
        /// </returns>
        /// <remarks>
        /// 内部的には円同士の交差判定を行い、その結果をマウスヒット種別に変換します。
        /// </remarks>
        public static eMouseHitType IsHitMouseRangeFillCircle(float x, float y, float r, in PointF MousePoint, float MouseHitRange)
        {
            // 判定対象の円の中心座標
            PointF CircVector = new PointF(x, y);
            eIsCrossCircles Ret;

            IsCrossCircles(CircVector, r, MousePoint, MouseHitRange, out Ret);

            eMouseHitType hitType;

            switch (Ret)
            {
                // 交差していない
                case eIsCrossCircles.None:
                    hitType = eMouseHitType.None;
                    break;
                // 接している、または交差している
                case eIsCrossCircles.Cross:
                case eIsCrossCircles.Circ1PtInCirc2:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // マウスポイントが円の内側に含まれている
                case eIsCrossCircles.Circ2PtInCirc1:
                case eIsCrossCircles.BothIn:
                    hitType = eMouseHitType.MousePointOnObject;
                    break;
                // 想定外
                default:
                    hitType = eMouseHitType.None;
                    break;
            }

            return hitType;
        }

        /// <summary>
        /// 円の輪郭線とマウス位置の当たり判定を行う
        /// </summary>
        /// <param name="x">円の中心座標X</param>
        /// <param name="y">円の中心座標Y</param>
        /// <param name="r">円の半径</param>
        /// <param name="lineWidth">円の輪郭線の太さ</param>
        /// <param name="MousePoint">マウス座標</param>
        /// <param name="MouseHitRange">マウスの当たり判定範囲（半径）</param>
        /// <returns>
        /// eMouseHitType の判定結果：  
        /// ・None : ヒットしていない  
        /// ・CrossMouseRange : マウスの当たり判定円が円の輪郭線と交差している  
        /// ・MousePointOnObject : マウスポイントが円の輪郭線上にある  
        /// </returns>

        public static eMouseHitType IsHitMouseRangeLineCircle(float x, float y, float r, float lineWidth, in PointF MousePoint, float MouseHitRange)
        {
            // 線幅をグリッド座標サイズに変換
            lineWidth = (float)lineWidth / (float)Graphic2DControl.DisplayGridWidth;

            // 判定対象の円の中心座標
            PointF CircVector = new PointF(x, y);
            eIsCrossCirclesOutLine Ret;

            IsCrossCirclesOutLine(CircVector, r, lineWidth, MousePoint, MouseHitRange, 0, out Ret);

            eMouseHitType hitType;

            switch (Ret)
            {
                // 交差していない
                case eIsCrossCirclesOutLine.None:
                    hitType = eMouseHitType.None;
                    break;
                // 接している、または交差している
                case eIsCrossCirclesOutLine.Cross:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // マウスポイントが円の輪郭線上にある
                case eIsCrossCirclesOutLine.Circ2PtOnCirc1:
                    hitType = eMouseHitType.MousePointOnObject;
                    break;
                // 想定外
                default:
                    hitType = eMouseHitType.None;
                    break;
            }

            return hitType;
        }

        /// <summary>
        /// マウスポイントと円弧（塗りつぶし領域）の当たり判定を行うメソッド。
        /// 円弧の中心座標・半径・開始角度・終了角度を指定し、
        /// マウスポイントが円弧の内部または近傍にあるかどうかを判定する。
        /// </summary>
        /// <param name="x">円弧の中心座標X。</param>
        /// <param name="y">円弧の中心座標Y。</param>
        /// <param name="r">円弧の半径。</param>
        /// <param name="StartAngle">円弧の開始角度（度数法）。</param>
        /// <param name="EndAngle">円弧の終了角度（度数法）。</param>
        /// <param name="MousePoint">判定対象となるマウスポイント座標。</param>
        /// <param name="MouseHitRange">マウス判定の許容範囲（半径）。</param>
        /// <returns>
        /// eMouseHitType 列挙値:
        /// - None : 当たり判定なし  
        /// - CrossMouseRange : マウス範囲が円弧と交差または接触  
        /// - MousePointOnObject : マウスポイントが円弧内部に含まれる  
        /// </returns>

        public static eMouseHitType IsHitMouseRangeFillArc(float x, float y, float r, float StartAngle, float EndAngle, in PointF MousePoint, float MouseHitRange)
        {
            // 判定対象の円の中心座標
            PointF CircVector = new PointF(x, y);
            eIsCrossArcAndCircle Ret;

            IsCrossArcAndCircle(CircVector, r, StartAngle, EndAngle, MousePoint, MouseHitRange, out Ret);

            eMouseHitType hitType;

            switch (Ret)
            {
                // 交差していない
                case eIsCrossArcAndCircle.None:
                    hitType = eMouseHitType.None;
                    break;
                // 接している、または交差している
                case eIsCrossArcAndCircle.Cross:
                case eIsCrossArcAndCircle.ArcPtInCirc:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // マウスポイントが円弧の内側に含まれている
                case eIsCrossArcAndCircle.CircPtInArc:
                    hitType = eMouseHitType.MousePointOnObject;
                    break;
                // 想定外
                default:
                    hitType = eMouseHitType.None;
                    break;
            }

            return hitType;
        }

        /// <summary>
        /// マウスポイントと円弧（輪郭線）の当たり判定を行うメソッド。
        /// 円弧の中心座標・半径・開始角度・終了角度・線幅を指定し、
        /// マウスポイントが円弧の輪郭線上または近傍にあるかどうかを判定する。
        /// </summary>
        /// <param name="x">円弧の中心座標X。</param>
        /// <param name="y">円弧の中心座標Y。</param>
        /// <param name="r">円弧の半径。</param>
        /// <param name="StartAngle">円弧の開始角度（度数法）。</param>
        /// <param name="EndAngle">円弧の終了角度（度数法）。</param>
        /// <param name="lineWidth">円弧の線幅。</param>
        /// <param name="MousePoint">判定対象となるマウスポイント座標。</param>
        /// <param name="MouseHitRange">マウス判定の許容範囲（半径）。</param>
        /// <returns>
        /// eMouseHitType 列挙値:
        /// - None : 当たり判定なし  
        /// - CrossMouseRange : マウス範囲が円弧と交差または接触  
        /// - MousePointOnObject : マウスポイントが円弧の輪郭線上にある  
        /// </returns>

        public static eMouseHitType IsHitMouseRangeLineArc(float x, float y, float r, float StartAngle, float EndAngle, float lineWidth, bool IsDrawSideLines, in PointF MousePoint, float MouseHitRange)
        {
            // 線幅をグリッド座標サイズに変換
            lineWidth = (float)lineWidth / (float)Graphic2DControl.DisplayGridWidth;

            // 判定対象の円の中心座標
            PointF CircVector = new PointF(x, y);
            eIsCrossArcAndCircleOutLine Ret;

            IsCrossArcAndCircleOutLine(CircVector, r, StartAngle, EndAngle, lineWidth, IsDrawSideLines, MousePoint,  MouseHitRange, out Ret);

            eMouseHitType hitType;

            switch (Ret)
            {
                // 交差していない
                case eIsCrossArcAndCircleOutLine.None:
                    hitType = eMouseHitType.None;
                    break;
                // 接している、または交差している
                case eIsCrossArcAndCircleOutLine.Cross:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // マウスポイントが円弧の輪郭線上にある
                case eIsCrossArcAndCircleOutLine.CircPtOnArc:
                    hitType = eMouseHitType.MousePointOnObject;
                    break;
                // 想定外
                default:
                    hitType = eMouseHitType.None;
                    break;
            }

            return hitType;
        }

        /// <summary>
        /// 折れ線グラフ（点リストで構成されたポリゴンライン）に対して、
        /// 指定されたマウス座標がヒットしているかどうかを判定する。
        /// </summary>
        /// <param name="GraphPoints">グラフを構成する点リスト</param>
        /// <param name="lineWidth">グラフ線の太さ</param>
        /// <param name="MousePoint">判定対象のマウス座標</param>
        /// <param name="MouseHitRange">マウス判定円の半径</param>
        /// <returns>
        /// マウスとグラフの関係を示す列挙値:
        /// <list type="bullet">
        /// <item><description><see cref="eMouseHitType.None"/> : ヒットしていない</description></item>
        /// <item><description><see cref="eMouseHitType.MousePointOnObject"/> : グラフ線上にマウスがある</description></item>
        /// <item><description><see cref="eMouseHitType.CrossMouseRange"/> : マウス判定円がグラフ線と交差している</description></item>
        /// </list>
        /// </returns>
        public static eMouseHitType IsHitMouseRangeLineGraph(in List<PointF> GraphPoints, float lineWidth, in PointF MousePoint, float MouseHitRange)
        {
            // 線幅をグリッド座標サイズに変換
            lineWidth = (float)lineWidth / (float)Graphic2DControl.DisplayGridWidth;

            // ------------------------------
            // グラフ本体とマウスがヒットしているか厳密に判定する
            // ------------------------------
            eIsCrossPolygonAndCircleLine Ret;

            IsCrossCurvePointsAndCircleLine(GraphPoints, MousePoint, MouseHitRange, lineWidth, out Ret);

            eMouseHitType hitType;

            switch (Ret)
            {
                // 交差していない
                case eIsCrossPolygonAndCircleLine.None:
                    hitType = eMouseHitType.None;
                    break;
                // ポリゴンの輪郭線上にマウスがある
                case eIsCrossPolygonAndCircleLine.OnLine:
                    hitType = eMouseHitType.MousePointOnObject;
                    break;
                // 接している、または交差している
                case eIsCrossPolygonAndCircleLine.Cross:
                    hitType = eMouseHitType.CrossMouseRange;
                    break;
                // 想定外
                default:
                    hitType = eMouseHitType.None;
                    break;
            }

            return hitType;
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

            k_a = (((b_0.X - a_0.X) * b.Y - (b_0.Y - a_0.Y) * b.X)) / (a.X * b.Y - a.Y * b.X);
            k_b = (((a_0.X - b_0.X) * a.Y - (a_0.Y - b_0.Y) * a.X)) / (b.X * a.Y - b.Y * a.X);

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
        /// Graphics と同じ描画設定（TextRenderingHint等）で最も正確な文字列の描画矩形を取得する。
        /// MeasureCharacterRanges を使うため、余白が少ない精密な領域を返す。
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="text">文字列</param>
        /// <param name="font">フォント</param>
        /// <returns>描画矩形</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static RectangleF GetTextRectangle(Graphics g, string text, Font font)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            // 描画設定は呼び出し側の Graphics に合わせるのがベスト（ClearType/AntiAlias 等）
            // ここでは最小限の StringFormat を用意
            using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
            {
                sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                sf.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, text.Length) });

                // 十分に大きなレイアウト矩形を用意（折り返しなしの場合は幅は大きめ）
                RectangleF layoutRect = new RectangleF(0, 0, 10000f, 10000f);

                Region[] regions = g.MeasureCharacterRanges(text, font, layoutRect, sf);
                if (regions != null && regions.Length > 0)
                {
                    RectangleF Rect = regions[0].GetBounds(g);

                    // 日本語文字が含まれている場合、幅を少し広げる補正を行う
                    int JapaneseCharCount = CountJapaneseChars(text);

                    Rect.Width += JapaneseCharCount * 1.5f * (font.Size / 24f);

                    return Rect;
                }

                return RectangleF.Empty;
            }
        }

        /// <summary>
        /// 引数の文字列に含まれる日本語文字（ひらがな・カタカナ・漢字）の数を返す。
        /// </summary>
        /// <param name="text">対象文字列</param>
        /// <returns>日本語文字の数</returns>
        public static int CountJapaneseChars(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // 正規表現で日本語文字を判定
            // ひらがな: \u3040-\u309F
            // カタカナ: \u30A0-\u30FF
            // 漢字: \u4E00-\u9FFF （基本漢字）
            // ※必要に応じて拡張漢字領域も追加可能
            Regex regex = new Regex(@"[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FFF]");

            int count = 0;
            foreach (char c in text)
            {
                if (regex.IsMatch(c.ToString()))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Graphics と同じ描画設定（TextRenderingHint等）で最も正確な文字列の描画矩形を取得する。
        /// MeasureCharacterRanges を使うため、余白が少ない精密な領域を返す。
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="font">フォント</param>
        /// <returns>描画矩形</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static RectangleF GetTextRectangle(string text, Font font)
        {

            Graphics g = Graphic2DControl.CreateGraphics();

            try
            {
                return GetTextRectangle(g, text, font);
            }
            finally
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// Graphics と同じ描画設定（TextRenderingHint等）で最も正確な文字列の描画矩形を取得する。
        /// MeasureCharacterRanges を使うため、余白が少ない精密な領域を返す。
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="FontSize">フォントサイズ</param>
        /// <param name="FontName">フォント名</param>
        /// <returns></returns>
        public static RectangleF GetTextRectangle(string text, float FontSize, string FontName)
        {
            using (Font font = new Font(FontName, FontSize))
            {
                return GetTextRectangle(text, font);
            }
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

            return (AllSumVec / Points.Count);
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

        /// <summary>
        /// 弧度法（ラジアン）を度数法（度）に変換する。
        /// </summary>
        /// <param name="radian">角度（ラジアン）</param>
        /// <returns>角度（度数法）</returns>
        public static float RadianToDegree(float radian)
        {
            return radian * 180f / (float)Math.PI;
        }

        /// <summary>
        /// 度数法（度）を弧度法（ラジアン）に変換する。
        /// </summary>
        /// <param name="degree">角度（度数法）</param>
        /// <returns>角度（ラジアン）</returns>
        public static float DegreeToRadian(float degree)
        {
            return degree * (float)Math.PI / 180f;
        }

        // ===============================================================================
        // 座標変換系関数
        // ===============================================================================

        /// <summary>
        /// グリッド座標をクライアント座標に変換します。
        /// </summary>
        /// <param name="GridPoint">変換対象のグリッド座標 (X, Y)</param>
        /// <param name="ClientCenterPoint">クライアント座標系の中心点</param>
        /// <returns>変換後のクライアント座標</returns>
        /// <remarks>
        /// Y座標はクライアント座標系に合わせて反転されます。
        /// </remarks>
        public static Point ConvertGridPointToClientPoint(in PointF GridPoint)
        {
            Vector2 ClientPointV = new Vector2();

            ClientPointV.X = GridPoint.X * Graphic2DControl.DisplayGridWidth;
            ClientPointV.Y = -1 * GridPoint.Y * Graphic2DControl.DisplayGridWidth;

            // 更に始点座標を足す
            ClientPointV = ClientPointV + Graphic2DControl.DisplayCenterPoint.ToVector2();

            return ClientPointV.ToPoint();
        }

        /// <summary>
        /// クライアント座標をグリッド座標に変換します。
        /// </summary>
        /// <param name="ClientPoint">変換対象のクライアント座標</param>
        /// <returns>変換後のグリッド座標。</returns>
        /// <remarks>
        /// Y座標はグリッド座標系に合わせて反転されます。
        /// </remarks>
        public static PointF ConvertClientPointToGridPoint(in Point ClientPoint)
        {
            Vector2 GridV = new Vector2();

            GridV.X =      (float)(ClientPoint.X - (float)Graphic2DControl.DisplayCenterPoint.X) / (float)Graphic2DControl.DisplayGridWidth;
            GridV.Y = -1 * (float)(ClientPoint.Y - (float)Graphic2DControl.DisplayCenterPoint.Y) / (float)Graphic2DControl.DisplayGridWidth;

            return GridV.ToPointF();
        }

        /// <summary>
        /// クライアント座標のマウス移動量をグリッド座標のマウス移動量に変換します。
        /// </summary>
        /// <param name="ClientPoint">マウス移動量（クライアント座標）</param>
        /// <returns>マウス移動量（グリッド座標）</returns>
        public static PointF ConvertClientMouseMovementToGridMouseMovement(in Point ClientPoint)
        {
            Vector2 GridV = new Vector2();

            GridV.X = ConvertClientLengthToGridLength(ClientPoint.X);
            GridV.Y = -1 * ConvertClientLengthToGridLength(ClientPoint.Y);

            return GridV.ToPointF();
        }

        /// <summary>
        /// クライアント座標の長さをグリッド座標の長さに変換します。
        /// </summary>
        /// <param name="Length">クライアント座標の長さ</param>
        /// <returns>グリッド座標の長さ</returns>
        public static float ConvertClientLengthToGridLength(int Length)
        {
            return (float)Length / (float)Graphic2DControl.DisplayGridWidth;
        }

        /// <summary>
        /// グリッド座標の PointF をクライアント座標の PointF に変換
        /// </summary>
        /// <param name="GridPoint">グリッド座標の点（PointF）</param>
        /// <returns>クライアント座標に変換された PointF</returns>
        public static SKPoint ConvertGridPointToClientPoint(PointF GridPoint)
        {
            Vector2 cliGridV = new Vector2();
            cliGridV.X = GridPoint.X * Graphic2DControl.DisplayGridWidth;
            cliGridV.Y = -1 * GridPoint.Y * Graphic2DControl.DisplayGridWidth;
            cliGridV += Graphic2DControl.DisplayCenterPoint.ToVector2();
            return cliGridV.ToSKPoint();
        }

        /// <summary>
        /// グリッド座標 (x, y) をクライアント座標 (outX, outY) に変換
        /// </summary>
        /// <param name="x">グリッド座標 X</param>
        /// <param name="y">グリッド座標 Y</param>
        /// <param name="outX">クライアント座標 X 出力</param>
        /// <param name="outY">クライアント座標 Y 出力</param>
        public static void ConvertGridPointToClientPoint(float x, float y, out float outX, out float outY)
        {
            outX = x * Graphic2DControl.DisplayGridWidth;
            outY = -1 * y * Graphic2DControl.DisplayGridWidth;
            outX += Graphic2DControl.DisplayCenterPoint.X;
            outY += Graphic2DControl.DisplayCenterPoint.Y;
        }

        /// <summary>
        /// グリッド座標の PointF をクライアント座標の PointF に変換
        /// </summary>
        /// <param name="GridPoints">グリッド座標の点リスト（PointF）</param>
        /// <returns>クライアント座標に変換された PointFリスト</returns>
        public static SKPoint [] ConvertGridPointToClientPoint(PointF[] GridPoints)
        {
            SKPoint[] ClientPoints = new SKPoint[GridPoints.Length];

            for (int i = 0; i < GridPoints.Length; i++)
            {
                ClientPoints[i] = ConvertGridPointToClientPoint(GridPoints[i]);
            }

            return ClientPoints;
        }

        /// <summary>
        /// Line2Dオブジェクト をベクトルに変換します。
        /// </summary>
        /// <param name="line">変換対象の線分。</param>
        /// <param name="startVec">線分の始点を表すベクトル。</param>
        /// <param name="lineVec">線分の方向ベクトル（終点 - 始点）。</param>
        /// <remarks>
        /// 始点を基準に、終点との差分ベクトルを算出します。
        /// </remarks>
        public static void ConvertLine2DToVector2(in Line2D line, out Vector2 startVec, out Vector2 lineVec)
        {
            startVec = line.Start.ToVector2();
            lineVec = line.End.ToVector2() - line.Start.ToVector2();
        }

        // ===============================================================================
        // バウンディングボックス取得関数
        // ===============================================================================

        /// <summary>
        /// 線分を囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <param name="Start">線分の始点</param>
        /// <param name="End">線分の終点</param>
        /// <param name="Width">線の幅</param>
        /// <param name="calculateType">座標系の種類（Client または Grid）</param>
        /// <returns>バウンディングボックスの4頂点座標</returns>

        public static PointF[] GetBoundingBoxLine(PointF Start, PointF End, int Width, eCalculateType calculateType = eCalculateType.Client)
        {
            return GetBoundingBoxLine<PointF>(Start.ToVector2(), End.ToVector2(), Width, calculateType);
        }

        /// <summary>
        /// 線分を囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <param name="Start">線分の始点</param>
        /// <param name="End">線分の終点</param>
        /// <param name="Width">線の幅</param>
        /// <param name="calculateType">座標系の種類（Client または Grid）</param>
        /// <returns>バウンディングボックスの4頂点座標</returns>

        public static SKPoint[] GetBoundingBoxLineSK(SKPoint Start, SKPoint End, int Width, eCalculateType calculateType = eCalculateType.Client)
        {
            return GetBoundingBoxLine<SKPoint>(Start.ToVector2(), End.ToVector2(), Width, calculateType);
        }

        /// <summary>
        /// 線分を囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <param name="Start">線分の始点</param>
        /// <param name="End">線分の終点</param>
        /// <param name="Width">線の幅</param>
        /// <param name="calculateType">座標系の種類（Client または Grid）</param>
        /// <returns>バウンディングボックスの4頂点座標</returns>

        private static T[] GetBoundingBoxLine<T>(Vector2 Start, Vector2 End, int Width, eCalculateType calculateType = eCalculateType.Client)
        {
            // 線を囲む四角形の頂点を計算して返す
            // 幅を考慮してバウンディングボックスを計算
            Vector2 line = End - Start;

            // 線分の直行ベクトルを求める
            Vector2 TyokkouVector = new Vector2(-line.Y, line.X);

            float length;

            if (calculateType == eCalculateType.Grid)
            {
                length = GraphicCaluculate.ConvertClientLengthToGridLength(Width + LINE_BOUNDING_BOX_WITDH);
            }
            else
            {
                length = Width + LINE_BOUNDING_BOX_WITDH;
            }


            TyokkouVector = Vector2.Normalize(TyokkouVector) * length;

            if (typeof(T) == typeof(PointF))
            {
                PointF[] boundingBox = new PointF[4];

                boundingBox[0] = (Start + TyokkouVector).ToPointF();
                boundingBox[1] = (End + TyokkouVector).ToPointF();
                boundingBox[2] = (End - TyokkouVector).ToPointF();
                boundingBox[3] = (Start - TyokkouVector).ToPointF();

                return boundingBox as T[];
            }
            else if (typeof(T) == typeof(SKPoint))
            {
                SKPoint[] boundingBox = new SKPoint[4];
                boundingBox[0] = (Start + TyokkouVector).ToSKPoint();
                boundingBox[1] = (End + TyokkouVector).ToSKPoint();
                boundingBox[2] = (End - TyokkouVector).ToSKPoint();
                boundingBox[3] = (Start - TyokkouVector).ToSKPoint();

                return boundingBox as T[];
            }

            return null;
        }

        /// <summary>
        /// 円を囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <typeparam name="T">PointF または SKPoint</typeparam>
        /// <param name="X">円の中心X座標</param>
        /// <param name="Y">円の中心Y座標</param>
        /// <param name="R">円の半径</param>
        /// <returns>バウンディングボックスの4頂点座標</returns>
        public static PointF[] GetBoundingBoxCircle(float X, float Y, float R)
        {
            return new PointF[]
            {
                new PointF(X - R, Y - R),
                new PointF(X + R, Y - R),
                new PointF(X + R, Y + R),
                new PointF(X - R, Y + R)
            };
        }

        /// <summary>
        /// 円を囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <typeparam name="T">PointF または SKPoint</typeparam>
        /// <param name="X">円の中心X座標</param>
        /// <param name="Y">円の中心Y座標</param>
        /// <param name="R">円の半径</param>
        /// <returns>バウンディングボックスの4頂点座標</returns>
        public static SKPoint[] GetBoundingBoxCircleSK(float X, float Y, float R)
        {
            return new SKPoint[]
            {
                new SKPoint(X - R, Y - R),
                new SKPoint(X + R, Y - R),
                new SKPoint(X + R, Y + R),
                new SKPoint(X - R, Y + R)
            };
        }

        /// <summary>
        /// 多角形を囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <param name="Points">多角形の頂点座標配列</param>
        /// <returns>バウンディングボックスの4頂点座標（Pointsがnullまたは空ならnull）</returns>

        public static PointF[] GetBoundingBoxPolygon(PointF[] Points)
        {
            if (Points == null || Points.Length == 0) return null;

            float minX = Points[0].X, maxX = Points[0].X;
            float minY = Points[0].Y, maxY = Points[0].Y;

            foreach (var p in Points)
            {
                if (p.X < minX) minX = p.X;
                if (p.X > maxX) maxX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.Y > maxY) maxY = p.Y;
            }

            return new PointF[]
            {
                new PointF(minX, minY),
                new PointF(maxX, minY),
                new PointF(maxX, maxY),
                new PointF(minX, maxY)
            };
        }

        public static SKPoint[] GetBoundingBoxPolygonSK(SKPoint[] Points)
        {
            if (Points == null || Points.Length == 0) return null;

            float minX = Points[0].X, maxX = Points[0].X;
            float minY = Points[0].Y, maxY = Points[0].Y;

            foreach (var p in Points)
            {
                if (p.X < minX) minX = p.X;
                if (p.X > maxX) maxX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.Y > maxY) maxY = p.Y;
            }

            return new SKPoint[]
            {
                new SKPoint(minX, minY),
                new SKPoint(maxX, minY),
                new SKPoint(maxX, maxY),
                new SKPoint(minX, maxY)
            };
        }

        /// <summary>
        /// テキストを囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <param name="X">テキスト描画位置のX座標</param>
        /// <param name="Y">テキスト描画位置のY座標</param>
        /// <param name="sKTextBlob">描画対象の SKTextBlob</param>
        /// <param name="Angle">回転角度（度数法）</param>
        /// <param name="calculateType">座標系の種類（Client または Grid）</param>
        /// <returns>バウンディングボックスの4頂点座標</returns>
        public static PointF[] GetBoundingBoxText(
            float X,
            float Y,
            string text,
            float fontSize,
            string fontName,
            float Angle,
            eCalculateType calculateType = eCalculateType.Client)
        {
            SKFont font =  DrawManager.GetSKFont(fontName, fontSize);

            SKTextBlob sKTextBlob = SKTextBlob.Create(text, font);

            try
            {
                SKRect textRect = sKTextBlob.Bounds;

                float width = textRect.Width;
                float height = textRect.Height;

                if (calculateType == eCalculateType.Grid)
                {
                    width = ConvertClientLengthToGridLength((int)width);
                    height = ConvertClientLengthToGridLength((int)height);
                }
                else
                {
                    // Client座標系では角度を反転
                    Angle = -Angle;
                }

                // テキスト矩形の4頂点を定義（原点基準）
                PointF[] boundingBoxPoints =
                {
                    new PointF(0, 0),
                    new PointF(width, 0),
                    new PointF(width, -height),
                    new PointF(0, -height)
                };

                // 回転行列を適用
                if (!Comp.IsEqual(Angle, 0f))
                {
                    using (Matrix matrix = new Matrix())
                    {
                        matrix.Rotate(Angle);
                        matrix.TransformPoints(boundingBoxPoints);
                    }
                }

                // 出力位置に平行移動
                for (int i = 0; i < boundingBoxPoints.Length; i++)
                {

                    boundingBoxPoints[i].X += X;
                    boundingBoxPoints[i].Y += Y;
                }

                return boundingBoxPoints;
            }
            finally
            {
                sKTextBlob.Dispose();
            }

        }

        /// <summary>
        /// テキストを囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <param name="X">テキスト描画位置のX座標</param>
        /// <param name="Y">テキスト描画位置のY座標</param>
        /// <param name="sKTextBlob">描画対象の SKTextBlob</param>
        /// <param name="Angle">回転角度（度数法）</param>
        /// <param name="calculateType">座標系の種類（Client または Grid）</param>
        /// <returns>バウンディングボックスの4頂点座標</returns>
        public static SKPoint[] GetBoundingBoxTextSK(
            float X,
            float Y,
            SKFont font,
            string text,
            float Angle,
            eCalculateType calculateType = eCalculateType.Client)
        {
            SKRect textRect;

            float width = font.MeasureText(text, out textRect);

            float height = textRect.Height;

            if (calculateType == eCalculateType.Grid)
            {
                height = -height;
                width = ConvertClientLengthToGridLength((int)width);
                height = ConvertClientLengthToGridLength((int)height);
            }
            else
            {
                // Client座標系では角度を反転
                Angle = -Angle;
            }

            // テキスト矩形の4頂点を定義（原点基準）
            SKPoint[] boundingBoxPoints =
            {
            new SKPoint(0, 0),
            new SKPoint(width, 0),
            new SKPoint(width, height),
            new SKPoint(0, height)
            };

            // 回転行列を適用
            if (!Comp.IsEqual(Angle, 0f))
            {
                SKMatrix matrix = SKMatrix.CreateRotationDegrees(Angle);
                boundingBoxPoints = matrix.MapPoints(boundingBoxPoints);
            }

            // 出力位置に平行移動
            for (int i = 0; i < boundingBoxPoints.Length; i++)
            {

                boundingBoxPoints[i].X += X;
                boundingBoxPoints[i].Y += Y;
            }

            return boundingBoxPoints;
        }

        /// <summary>
        /// 指定したテキストと SKFont を使用して、
        /// 描画時に必要となるテキストの境界矩形（バウンディングボックス）を取得する。
        /// </summary>
        /// <param name="text">計測対象の文字列</param>
        /// <param name="font">使用する SKFont（呼び出し元で生成済み）</param>
        /// <returns>テキストの描画領域を表す SKRect</returns>
        /// <remarks>
        /// ・SKTextBlob を生成して Bounds から矩形を取得  
        /// ・font と SKTextBlob は finally で確実に Dispose  
        /// ・改行を含む場合は SKTextBlob が行単位で処理するため、
        ///   実際の描画位置と一致する矩形が得られる  
        /// </remarks>
        public static SKRect GetTextRect(string text, SKFont font)
        {
            SKTextBlob sKTextBlob = SKTextBlob.Create(text, font);

            try
            {
                SKRect textRect = sKTextBlob.Bounds;

                return textRect;
            }
            finally
            {
                font.Dispose();
                sKTextBlob.Dispose();
            }
        }

        /// <summary>
        /// フォント名とサイズを指定して SKFont を生成し、
        /// テキストの描画領域（バウンディングボックス）を取得する。
        /// </summary>
        /// <param name="text">計測対象の文字列</param>
        /// <param name="fontSize">フォントサイズ（px）</param>
        /// <param name="fontName">フォント名</param>
        /// <returns>テキストの描画領域を表す SKRect</returns>
        /// <remarks>
        /// ・DrawManager.GetSKFont により SKFont を生成  
        /// ・SKTextBlob を使用して Bounds を取得  
        /// ・生成した font と SKTextBlob は finally で Dispose  
        /// ・フォント指定でのテキスト計測に便利なオーバーロード  
        /// </remarks>
        public static SKRect GetTextRect(string text, float fontSize, string fontName)
        {
            SKFont font = DrawManager.GetSKFont(fontName, fontSize);

            SKTextBlob sKTextBlob = SKTextBlob.Create(text, font);

            try
            {
                SKRect textRect = sKTextBlob.Bounds;

                return textRect;
            }
            finally
            {
                font.Dispose();
                sKTextBlob.Dispose();
            }
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
