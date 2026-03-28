using graphicbox2d.グローバル変数;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static graphicbox2d.グラフィック計算.GraphicCaluculate;

namespace graphicbox2d.グラフィック計算
{
    internal static class CalIsHit
    {
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
            lineWidth = (float)lineWidth / Global.Graphic2DControl.DisplayGridWidth;

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
            lineWidth = (float)lineWidth / Global.Graphic2DControl.DisplayGridWidth;

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
            lineWidth = (float)lineWidth / Global.Graphic2DControl.DisplayGridWidth;

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
        /// <param name="IsDrawSideLines">円弧の側面線を描画するかどうかのフラグ。</param>
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
            lineWidth = (float)lineWidth / Global.Graphic2DControl.DisplayGridWidth;

            // 判定対象の円の中心座標
            PointF CircVector = new PointF(x, y);
            eIsCrossArcAndCircleOutLine Ret;

            IsCrossArcAndCircleOutLine(CircVector, r, StartAngle, EndAngle, lineWidth, IsDrawSideLines, MousePoint, MouseHitRange, out Ret);

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
            lineWidth = (float)lineWidth / Global.Graphic2DControl.DisplayGridWidth;

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
    }
}
