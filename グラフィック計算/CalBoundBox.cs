using graphicbox2d.オブジェクトマネージャー;
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
using static graphicbox2d.グラフィック計算.CalConvert;

namespace graphicbox2d.グラフィック計算
{
    internal static class CalBoundBox
    {
        /// <summary>
        /// 線図形のバウンディングボックスの幅
        /// </summary>
        private const int LINE_BOUNDING_BOX_WITDH = 4;

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
                length = ConvertClientLengthToGridLength(Width + LINE_BOUNDING_BOX_WITDH);
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

        /// <summary>
        /// 多角形を囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <param name="Points">多角形の頂点座標配列</param>
        /// <returns>バウンディングボックスの4頂点座標（Pointsがnullまたは空ならnull）</returns>
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
        /// 指定された位置・サイズ・角度を持つ矩形のバウンディングボックス（4頂点）を計算して返す。
        /// </summary>
        /// <param name="X">矩形の基準X座標（TopLeft または Center）</param>
        /// <param name="Y">矩形の基準Y座標（TopLeft または Center）</param>
        /// <param name="Width">矩形の幅</param>
        /// <param name="Height">矩形の高さ</param>
        /// <param name="Angle">回転角度（度数法）</param>
        /// <param name="calculateType">
        /// 座標系の種類を指定する。
        /// ・Client : 角度を反転して計算（描画座標系）
        /// ・World  : そのままの角度で計算
        /// </param>
        /// <param name="rotateType">
        /// 回転の基準点を指定する。
        /// ・TopLeft : 左上を基準に回転  
        /// ・Center  : 中心点を基準に回転
        /// </param>
        /// <returns>回転後の矩形を構成する4頂点の PointF 配列</returns>
        public static PointF[] GetBoundingBox(
            float X,
            float Y,
            float Width,
            float Height,
            float Angle,
            eCalculateType calculateType,
            eRotateType rotateType = eRotateType.TopLeft)
        {
            // 4頂点（原点基準）
            PointF[] boundingBoxPoints = null;

            if (calculateType == eCalculateType.Grid)
            {
                boundingBoxPoints = new PointF[]
                {
                    new PointF(0, 0),
                    new PointF(Width, 0),
                    new PointF(Width, -Height),
                    new PointF(0, -Height)
                };
            }
            else if (calculateType == eCalculateType.Client)
            {
                boundingBoxPoints = new PointF[]
                {
                    new PointF(0, 0),
                    new PointF(Width, 0),
                    new PointF(Width, Height),
                    new PointF(0, Height)
                };
            }

            // Client座標系では角度を反転
            if (calculateType == eCalculateType.Client)
            {
                Angle = -Angle;
            }
            // --- 回転処理 ---
            if (!Comp.IsEqual(Angle, 0f))
            {
                // --- Center 回転対応 ---
                float originX = 0f;
                float originY = 0f;

                if (rotateType == eRotateType.Center)
                {
                    originX = Width / 2f;
                    originY = Height / 2f;

                    // MoveBoundingBoxe を使用して中心基準に移動
                    MoveBoundingBox(ref boundingBoxPoints, -originX, -originY);
                }

                using (Matrix matrix = new Matrix())
                {
                    matrix.Rotate(Angle);
                    matrix.TransformPoints(boundingBoxPoints);
                }

                // --- 回転後の位置を戻す ---
                if (rotateType == eRotateType.Center)
                {
                    MoveBoundingBox(ref boundingBoxPoints, originX, originY);
                }

            }

            // --- 出力位置に平行移動 ---
            MoveBoundingBox(ref boundingBoxPoints, X, Y);

            return boundingBoxPoints;
        }

        /// <summary>
        /// 指定された位置・サイズ・角度を持つ矩形のバウンディングボックス（4頂点）を計算して返す。
        /// </summary>
        /// <param name="X">矩形の基準X座標（TopLeft または Center）</param>
        /// <param name="Y">矩形の基準Y座標（TopLeft または Center）</param>
        /// <param name="Width">矩形の幅</param>
        /// <param name="Height">矩形の高さ</param>
        /// <param name="Angle">回転角度（度数法）</param>
        /// <param name="calculateType">
        /// 座標系の種類を指定する。
        /// ・Client : 角度を反転して計算（描画座標系）
        /// ・World  : そのままの角度で計算
        /// </param>
        /// <param name="rotateType">
        /// 回転の基準点を指定する。
        /// ・TopLeft : 左上を基準に回転  
        /// ・Center  : 中心点を基準に回転
        /// </param>
        /// <returns>回転後の矩形を構成する4頂点の SKPoint 配列</returns>
        public static SKPoint[] GetBoundingBoxSK(
            float X,
            float Y,
            float Width,
            float Height,
            float Angle,
            eCalculateType calculateType,
            eRotateType rotateType = eRotateType.TopLeft)
        {
            // 4頂点（原点基準）
            SKPoint[] boundingBoxPoints = null;

            if (calculateType == eCalculateType.Grid)
            {
                boundingBoxPoints = new SKPoint[]
                {
                    new SKPoint(0, 0),
                    new SKPoint(Width, 0),
                    new SKPoint(Width, -Height),
                    new SKPoint(0, -Height)
                };
            }
            else if (calculateType == eCalculateType.Client)
            {
                boundingBoxPoints = new SKPoint[]
                {
                    new SKPoint(0, 0),
                    new SKPoint(Width, 0),
                    new SKPoint(Width, Height),
                    new SKPoint(0, Height)
                };
            }

            // Client座標系では角度を反転
            if (calculateType == eCalculateType.Client)
            {
                Angle = -Angle;
            }

            // --- 回転処理 ---
            if (!Comp.IsEqual(Angle, 0f))
            {
                // --- Center 回転対応 ---
                float originX = 0f;
                float originY = 0f;

                if (rotateType == eRotateType.Center)
                {
                    originX = Width / 2f;
                    originY = Height / 2f;

                    // MoveBoundingBoxe を使用して中心基準に移動
                    MoveBoundingBox(ref boundingBoxPoints, -originX, -originY);
                }

                // 回転行列を適用
                if (!Comp.IsEqual(Angle, 0f))
                {
                    SKMatrix matrix = SKMatrix.CreateRotationDegrees(Angle);
                    boundingBoxPoints = matrix.MapPoints(boundingBoxPoints);
                }

                // --- 回転後の位置を戻す ---
                if (rotateType == eRotateType.Center)
                {
                    MoveBoundingBox(ref boundingBoxPoints, originX, originY);
                }

            }

            // --- 出力位置に平行移動 ---
            MoveBoundingBox(ref boundingBoxPoints, X, Y);

            return boundingBoxPoints;
        }

        /// <summary>
        /// バウンディングボックスを構成する全ての頂点座標を、指定量だけ平行移動する。
        /// </summary>
        /// <param name="BoundBox">
        /// 移動対象となるバウンディングボックスの頂点配列。
        /// ref で渡すことで、配列参照自体の差し替えにも対応可能だが、
        /// 本メソッドでは各要素（頂点）の座標値のみを更新する。
        /// </param>
        /// <param name="moveX">X方向の移動量（正で右、負で左）</param>
        /// <param name="moveY">Y方向の移動量（正で下、負で上）</param>
        public static void MoveBoundingBox(ref PointF[] BoundBox, float moveX, float moveY)
        {
            if (BoundBox == null || BoundBox.Length == 0)
            {
                return;
            }

            for (int i = 0; i < BoundBox.Length; i++)
            {
                BoundBox[i].X += moveX;
                BoundBox[i].Y += moveY;
            }
        }

        /// <summary>
        /// バウンディングボックスを構成する全ての頂点座標を、指定量だけ平行移動する。
        /// </summary>
        /// <param name="BoundBox">
        /// 移動対象となるバウンディングボックスの頂点配列。
        /// ref で渡すことで、配列参照自体の差し替えにも対応可能だが、
        /// 本メソッドでは各要素（頂点）の座標値のみを更新する。
        /// </param>
        /// <param name="moveX">X方向の移動量（正で右、負で左）</param>
        /// <param name="moveY">Y方向の移動量（正で下、負で上）</param>
        public static void MoveBoundingBox(ref SKPoint[] BoundBox, float moveX, float moveY)
        {
            if (BoundBox == null || BoundBox.Length == 0)
            {
                return;
            }

            for (int i = 0; i < BoundBox.Length; i++)
            {
                BoundBox[i].X += moveX;
                BoundBox[i].Y += moveY;
            }
        }

        /// <summary>
        /// テキストを囲むバウンディングボックス（四角形の頂点）を計算して返す
        /// </summary>
        /// <param name="X">テキスト描画位置のX座標</param>
        /// <param name="Y">テキスト描画位置のY座標</param>
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
            eCalculateType calculateType)
        {
            SKFont font = DrawManager.GetSKFont(fontName, fontSize);

            try
            {
                SizeF textSize = CalText.GetTextSize(text, font, calculateType);

                return GetBoundingBox(X, Y, textSize.Width, textSize.Height, Angle, calculateType);
            }
            finally
            {
                font.Dispose();
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
            eCalculateType calculateType)
        {
            SizeF textSize = CalText.GetTextSize(text, font, calculateType);

            return GetBoundingBoxSK(X, Y, textSize.Width, textSize.Height, Angle, calculateType);
        }
    }
}
