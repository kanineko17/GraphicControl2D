using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphicbox2d
{
    // ===============================================================================
    //   公開 enum
    // ===============================================================================
    /// <summary>
    /// 図形の種類
    /// </summary>
    public enum eObject2DType
    {
        /// <summary>未定義</summary>
        None,
        /// <summary>点図形</summary>
        Point, 
        /// <summary>線分図形</summary>
        Line, 
        /// <summary>円図形</summary>
        Circle,
        /// <summary>多角形図形</summary>
        Polygon,
        /// <summary>矢印図形</summary>
        Arrow,
        /// <summary>テキスト図形</summary>
        Text,
        /// <summary>円弧図形</summary>
        Arc,
        /// <summary>グラフ図形</summary>
        Graph,
        /// <summary>数式グラフ図形</summary>
        MathGraph,
        /// <summary>グループ図形</summary>
        Group,
        /// <summary>イメージ図形</summary>
        Image
    }

    /// <summary>
    /// 線種
    /// </summary>
    public enum LineStyle
    {
        /// <summary>実線（途切れのない通常の線）</summary>
        Solid,
        /// <summary>破線（一定の長さの線と空白を繰り返す）</summary>
        Dash,
        /// <summary>点線（短い線と空白を繰り返す）</summary>
        Dot,
        /// <summary>一点鎖線（長い線と点を交互に繰り返す）</summary>
        DashDot,
        /// <summary>二点鎖線（長い線と点2つを交互に繰り返す）</summary>
        DashDotDot,
        /// <summary>カスタム線種（任意のパターンを定義可能）</summary>
        Custom,
        /// <summary>セレクトボックス線（図形選択用）</summary>
        SelectBoxDash,
    }

    /// <summary>
    /// グリッドテキストの位置を表す列挙体。
    /// </summary>
    public enum eGridTextPosition
    {
        /// <summary>左上に配置</summary>
        TopLeft,
        /// <summary>右上に配置</summary>
        TopRight,
        /// <summary>左下に配置</summary>
        BottomLeft,
        /// <summary>右下に配置</summary>
        BottomRight,
    }

    /// <summary>
    /// 操作モード
    /// </summary>
    public enum eGraphic2DControlMode
    {
        /// <summary>なし</summary>
        Default,
        /// <summary>オブジェクトを選択・移動を可能にします。</summary>
        Select,
        /// <summary>格子点による選択・移動を可能にします。</summary>
        Snap,
    }

    // ===============================================================================
    //   非公開 enum（DLL内でのみ公開）
    // ===============================================================================
    /// <summary>
    /// 点と線との位置関係を表す列挙体。
    /// </summary>
    internal enum LineLR
    {
        /// <summary>
        /// ベクトルの線上に点が存在する
        /// </summary>
        OnVector,
        /// <summary>ベクトルの左側に点が存在する</summary>
        Left,
        /// <summary>ベクトルの右側に点が存在する</summary>
        Right
    }

    /// <summary>
    /// 線分と円の交差判定結果を表す列挙体。
    /// </summary>
    internal enum eIsCrossLineAndCircle
    {
        /// <summary>交差していない</summary>
        None,
        /// <summary>交差している</summary>
        Cross,
        /// <summary>円と線が接している</summary>
        Contact,
        /// <summary>円の中心が線上にある</summary>
        OnCenter
    }

    /// <summary>
    /// ポリゴンと円の交差判定結果を表す列挙体。
    /// </summary>
    internal enum eIsCrossPolygonAndCircle
    {
        /// <summary>交差していない</summary>
        None,
        /// <summary>円の中心がポリゴン上にある</summary>
        InPolygon,
        /// <summary>ポリゴンの点が円の中にある</summary>
        InCircle,
        /// <summary>円とポリゴンの線が交差している</summary>
        Cross
    }

    /// <summary>
    /// ポリゴンと円の交差判定結果を表す列挙体。
    /// </summary>
    internal enum eIsCrossPolygonAndCircleLine
    {
        /// <summary>交差していない</summary>
        None,
        /// <summary>円の中心がポリゴンの線上にある</summary>
        OnLine,
        /// <summary>円とポリゴンの線が交差している</summary>
        Cross
    }

    /// <summary>
    /// 円同士の交差判定結果を表す列挙体。
    /// </summary>
    internal enum eIsCrossCircles
    {
        /// <summary> 交差していない </summary>
        None,
        /// <summary> 円1の中心が円2の中にある </summary>
        Circ1PtInCirc2,
        /// <summary> 円2の中心が円1の中にある </summary>
        Circ2PtInCirc1,
        /// <summary> 両方の円の中心が互いの中にある </summary>
        BothIn,
        /// <summary> 円同士が交差している </summary>
        Cross,
    }

    /// <summary>
    /// 円弧と円の交差判定結果を表す列挙体。
    /// </summary>
    internal enum eIsCrossArcAndCircle
    {
        /// <summary> 交差していない </summary>
        None,
        /// <summary> 円弧の中心が円の中にある </summary>
        ArcPtInCirc,
        /// <summary> 円の中心が円弧の中にある </summary>
        CircPtInArc,
        /// <summary> 円弧と円が交差している </summary>
        Cross,
    }

    /// <summary>
    /// 円同士の交差判定結果を表す列挙体。
    /// </summary>
    internal enum eIsCrossCirclesOutLine
    {
        /// <summary> 交差していない </summary>
        None,
        /// <summary> 円1の中心が円2の輪郭線上にある </summary>
        Circ1PtOnCirc2,
        /// <summary> 円2の中心が円1の輪郭線上にある </summary>
        Circ2PtOnCirc1,
        /// <summary> 円同士が交差している </summary>
        Cross,
    }

    /// <summary>
    /// 円同士の交差判定結果を表す列挙体。
    /// </summary>
    internal enum eIsCrossArcAndCircleOutLine
    {
        /// <summary> 交差していない </summary>
        None,
        /// <summary> 円弧の中心が円の輪郭線上にある </summary>
        ArcPtOnCirc,
        /// <summary> 円の中心が円弧の輪郭線上にある </summary>
        CircPtOnArc,
        /// <summary> 円弧と円が交差している </summary>
        Cross,
    }

    /// <summary>
    /// オブジェクトのマウスヒット判定結果を表す列挙体。
    /// </summary>
    internal enum eMouseHitType
    {
        /// <summary> ヒットしていない </summary>
        None,
        /// <summary> マウス範囲とオブジェクトが交差している </summary>
        CrossMouseRange,
        /// <summary> オブジェクトの内部にマウスポイントがある </summary>
        MousePointOnObject,
    }

    /// <summary>
    /// 計算タイプ
    /// </summary>
    internal enum eCalculateType
    {
        /// <summary> クライアント座標系 </summary>
        Client,
        /// <summary> グリッド座標系 </summary>
        Grid
    }

    internal enum eRotateType
    {
        TopLeft,
        Center
    }

    // ===============================================================================
    //   公開 構造体
    // ===============================================================================

    /// <summary>
    /// グラフィックコントロール2Dのマウスイベント拡張データ
    /// </summary>
    public struct Graphic2DMouseEventExtensionData
    {
        /// <summary>
        /// 一番最後に取得したマウスポイント座標を保持する
        /// </summary>
        public Point Last_MouseDown_Point;

        /// <summary>
        /// 一番最後に取得したマウスアップポイント座標を保持する
        /// </summary>
        public Point Last_MouseUp_Point;

        /// <summary>
        /// 一番最後に取得したマウスダウンポイント座標を保持する
        /// </summary>
        public Point Last_Move_Point;
    }

    /// <summary>
    /// 線分キャップ（矢印など）のサイズ情報を表す構造体
    /// </summary>
    public struct CapSize : IEquatable<CapSize>
    {
        /// <summary>
        /// キャップの幅
        /// </summary>
        public int Width;

        /// <summary>
        /// キャップの高さ
        /// </summary>
        public int Height;

        /// <summary>
        /// CapProperty 同士の等価判定
        /// </summary>
        /// <param name="other">比較対象</param>
        /// <returns>等しい場合 true</returns>
        public bool Equals(CapSize other)
        {
            return Width.Equals(other.Width) &&
                   Height.Equals(other.Height);
        }

        /// <summary>
        /// オブジェクトとしての等価判定
        /// </summary>
        /// <param name="obj">比較対象</param>
        /// <returns>等しい場合 true</returns>
        public override bool Equals(object obj)
        {
            return obj is CapSize other && Equals(other);
        }

        /// <summary>
        /// ハッシュコードを取得する
        /// </summary>
        /// <returns>ハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }
    }


    // ===============================================================================
    //   非公開 構造体（DLL内でのみ公開）
    // ===============================================================================

    /// <summary>
    /// テキストデータ
    /// </summary>
    internal struct TextData
    {
        /// <summary>
        /// 出力テキスト
        /// </summary>
        public string Text;

        /// <summary>
        /// 出力座標
        /// </summary>
        public SKPoint TextPoint;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">出力テキスト</param>
        /// <param name="point">出力座標</param>
        public TextData(string text, SKPoint point)
        {
            Text = text;
            TextPoint = point;
        }
    }

    /// <summary>
    /// テキストデータ
    /// </summary>
    internal struct TextData_Old
    {
        /// <summary>
        /// 出力テキスト
        /// </summary>
        public string Text;

        /// <summary>
        /// 出力座標
        /// </summary>
        public PointF TextPoint;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">出力テキスト</param>
        /// <param name="point">出力座標</param>
        public TextData_Old(string text, PointF point)
        {
            Text = text;
            TextPoint = point;
        }
    }
}
