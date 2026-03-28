using graphicbox2d;
using graphicbox2d.グローバル変数;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.グラフィック計算
{
    internal class CalConvert
    {
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
        /// 表示グリッド座標をクライアント座標に変換します。
        /// </summary>
        /// <param name="GridPoint">変換対象の表示グリッド座標 (X, Y)</param>
        /// <param name="ClientCenterPoint">クライアント座標系の中心点</param>
        /// <returns>変換後のクライアント座標</returns>
        /// <remarks>
        /// Y座標はクライアント座標系に合わせて反転されます。
        /// </remarks>
        public static Point ConvertDisplayGridPointToClientPoint(in PointF GridPoint)
        {
            Vector2 ClientPointV = new Vector2();

            ClientPointV.X = GridPoint.X * Global.Graphic2DControl.DisplayGridWidth;
            ClientPointV.Y = -1 * GridPoint.Y * Global.Graphic2DControl.DisplayGridWidth;

            // 更に始点座標を足す
            ClientPointV = ClientPointV + Global.Graphic2DControl.DisplayCenterPoint.ToVector2();

            return ClientPointV.ToPoint();
        }

        /// <summary>
        /// クライアント座標を表示グリッド座標に変換します。
        /// </summary>
        /// <param name="ClientPoint">変換対象のクライアント座標</param>
        /// <returns>変換後の表示グリッド座標。</returns>
        /// <remarks>
        /// Y座標は表示グリッド座標系に合わせて反転されます。
        /// </remarks>
        public static PointF ConvertClientPointToDisplayGridPoint(in Point ClientPoint)
        {
            Vector2 GridV = new Vector2();

            GridV.X = (float)(ClientPoint.X - (float)Global.Graphic2DControl.DisplayCenterPoint.X) / Global.Graphic2DControl.DisplayGridWidth;
            GridV.Y = -1 * (float)(ClientPoint.Y - (float)Global.Graphic2DControl.DisplayCenterPoint.Y) / Global.Graphic2DControl.DisplayGridWidth;

            return GridV.ToPointF();
        }

        /// <summary>
        /// クライアント座標のマウス移動量を表示グリッド座標のマウス移動量に変換します。
        /// </summary>
        /// <param name="ClientPoint">マウス移動量（クライアント座標）</param>
        /// <returns>マウス移動量（表示グリッド座標）</returns>
        public static PointF ConvertClientMouseMovementToDisplayGridMouseMovement(in Point ClientPoint)
        {
            Vector2 GridV = new Vector2();

            GridV.X = ConvertClientLengthToDisplayGridLength(ClientPoint.X);
            GridV.Y = -1 * ConvertClientLengthToDisplayGridLength(ClientPoint.Y);

            return GridV.ToPointF();
        }

        /// <summary>
        /// クライアント座標の長さを表示グリッド座標の長さに変換します。
        /// </summary>
        /// <param name="Length">クライアント座標の長さ</param>
        /// <returns>表示グリッド座標の長さ</returns>
        public static float ConvertClientLengthToDisplayGridLength(int Length)
        {
            return Length / (float)Global.Graphic2DControl.DisplayGridWidth;
        }

        /// <summary>
        /// 表示グリッド座標の長さをクライアント座標の長さに変換します。
        /// </summary>
        /// <param name="Length">表示グリッド座標の長さ</param>
        /// <returns>クライアント座標の長さ</returns>
        public static int ConvertDisplayGridLengthToClientLength(float Length)
        {
            return (int)(Length * (float)Global.Graphic2DControl.DisplayGridWidth);
        }

        /// <summary>
        /// グリッド座標の PointF をクライアント座標の PointF に変換
        /// </summary>
        /// <param name="DisplayGridPoint">表示グリッド座標の点（PointF）</param>
        /// <returns>クライアント座標に変換された PointF</returns>
        public static SKPoint ConvertDisplayGridPointToClientPoint(PointF DisplayGridPoint)
        {
            Vector2 cliGridV = new Vector2();
            cliGridV.X = DisplayGridPoint.X * Global.Graphic2DControl.DisplayGridWidth;
            cliGridV.Y = -1 * DisplayGridPoint.Y * Global.Graphic2DControl.DisplayGridWidth;
            cliGridV += Global.Graphic2DControl.DisplayCenterPoint.ToVector2();
            return cliGridV.ToSKPoint();
        }

        /// <summary>
        /// 表示グリッド座標 (x, y) をクライアント座標 (outX, outY) に変換
        /// </summary>
        /// <param name="x">表示グリッド座標 X</param>
        /// <param name="y">表示グリッド座標 Y</param>
        /// <param name="outX">クライアント座標 X 出力</param>
        /// <param name="outY">クライアント座標 Y 出力</param>
        public static void ConvertDisplayGridPointToClientPoint(float x, float y, out float outX, out float outY)
        {
            outX = x * Global.Graphic2DControl.DisplayGridWidth;
            outY = -1 * y * Global.Graphic2DControl.DisplayGridWidth;
            outX += Global.Graphic2DControl.DisplayCenterPoint.X;
            outY += Global.Graphic2DControl.DisplayCenterPoint.Y;
        }

        /// <summary>
        /// グリッド座標の PointF をクライアント座標の PointF に変換
        /// </summary>
        /// <param name="GridPoints">表示グリッド座標の点リスト（PointF）</param>
        /// <returns>クライアント座標に変換された PointFリスト</returns>
        public static SKPoint[] ConvertDisplayGridPointToClientPoint(PointF[] GridPoints)
        {
            SKPoint[] ClientPoints = new SKPoint[GridPoints.Length];

            for (int i = 0; i < GridPoints.Length; i++)
            {
                ClientPoints[i] = ConvertDisplayGridPointToClientPoint(GridPoints[i]);
            }

            return ClientPoints;
        }

        /// <summary>
        /// クライアント座標の長さをグリッド座標の長さに変換します。
        /// </summary>
        /// <param name="Length">クライアント座標の長さ</param>
        /// <returns>グリッド座標の長さ</returns>
        public static float ConvertClientLengthToGridLength(int Length)
        {
            return Length / (float)Global.Graphic2DControl.GridWidth;
        }

        /// <summary>
        /// グリッド座標の長さをクライアント座標の長さに変換します。
        /// </summary>
        /// <param name="Length">グリッド座標の長さ</param>
        /// <returns>クライアント座標の長さ</returns>
        public static int ConvertGridLengthToClientLength(float Length)
        {
            return (int)(Length * (float)Global.Graphic2DControl.GridWidth);
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
    }
}
