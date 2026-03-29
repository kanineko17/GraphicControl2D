using graphicbox2d.グラフィック計算;
using graphicbox2d.その他;
using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Windows.Forms;

namespace graphicbox2d
{
    /// <summary>
    /// 画像図形クラス
    /// </summary>
    public class Image2D : Object2D
    {
        // ===============================================================================
        // 公開プロパティ
        // ===============================================================================

        /// <summary>
        /// 図形の種類
        /// </summary>
        public override eObject2DType m_Type => eObject2DType.Image;

        /// <summary>
        /// 左上X座標
        /// </summary>
        public float X { get; set; } = 0;

        /// <summary>
        /// 左上Y座標
        /// </summary>
        public float Y { get; set; } = 0;

        /// <summary>
        /// 回転角度（度）
        /// </summary>
        public float Angle { get; set; } = 0;

        /// <summary>
        /// スケール
        /// </summary>
        public float Scale 
        {
            get
            {
                return _Scale;
            }
            set
            {
                if (_Bitmap == null)
                {
                    return;
                }

                if (value <= 0)
                {
                    ShowWarningMessage("スケールは0より大きい値を指定してください。");
                    return;
                }

                _Scale = value;
                _Bitmap = SKBitmapUtil.MakeScaleBitMap(OriginalBitmap, _Scale);
            }
        }
        private float _Scale = 1.0f;


        /// <summary>
        /// 幅
        /// </summary>
        public float Width 
        {  
            get
            {
                if (_Bitmap == null)
                {
                    return 0;
                }

                return CalConvert.ConvertClientLengthToGridLength(_Bitmap.Width);
            } 
            set
            {
                if (_Bitmap == null)
                {
                    return;
                }

                int clientWidth = CalConvert.ConvertGridLengthToClientLength(value);

                _Bitmap = SKBitmapUtil.MakeResizeBitMap(OriginalBitmap, clientWidth, _Bitmap.Height);
            }
        }

        /// <summary>
        /// 高さ
        /// </summary>
        public float Height
        {
            get
            {
                if (_Bitmap == null)
                {
                    return 0;
                }

                return CalConvert.ConvertClientLengthToGridLength(_Bitmap.Height);
            }
            set
            {
                if (_Bitmap == null)
                {
                    return;
                }

                int clientHeight = CalConvert.ConvertGridLengthToClientLength(value);

                _Bitmap = SKBitmapUtil.MakeResizeBitMap(OriginalBitmap, _Bitmap.Width, clientHeight);
            }
        }

        /// <summary>
        /// 描画する画像
        /// </summary>
        public SKBitmap Bitmap
        {
            get
            {
                return _Bitmap;
            }
            set
            {
                if(OriginalBitmap != null)
                {
                    OriginalBitmap.Dispose();
                }

                OriginalBitmap = value;
                _Bitmap = OriginalBitmap.Copy();

                // 初期化
                Scale = 1.0f;
            }
        }
        private SKBitmap _Bitmap = null;

        // ===============================================================================
        // 非公開プロパティ
        // ===============================================================================

        /// <summary>
        /// オリジナルのビットマップ
        /// </summary>
        private SKBitmap OriginalBitmap = null;

        /// <summary>
        /// マウスヒット中の拡大率
        /// </summary>
        private float MouseHitBitmapOffset = 1.2f;

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint
        {
            get
            {
                if (Bitmap == null)
                {
                    return default;
                }

                return new Vector2(X + Bitmap.Width / 2f, Y + Bitmap.Height / 2f);
            }
        }

        internal int ClientWidth
        {
            get
            {
                if (_Bitmap == null)
                {
                    return 0;
                }
                return _Bitmap.Width;
            }
        }

        internal int ClientHeight
        {
            get
            {
                if (_Bitmap == null)
                {
                    return 0;
                }
                return _Bitmap.Height;
            }
        }

        // ===============================================================================
        // 公開メソッド
        // ===============================================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Image2D()
        {
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Image2D()
        {
            if (OriginalBitmap != null)
            {
                OriginalBitmap.Dispose();
            }
        }

        /// <summary>
        /// コピーを作成する
        /// </summary>
        /// <returns>コピーしたオブジェクト</returns>
        public override Object2D Clone()
        {
            Image2D clone = new Image2D();

            // 基底クラスのデータをコピー
            this.BaseCopyDataTo(clone);

            // 派生クラスのデータをコピー
            clone.X = this.X;
            clone.Y = this.Y;
            clone.Angle = this.Angle;
            clone._Scale = this._Scale;

            // Bitmap は参照コピー（必要なら DeepCopy に変更）
            clone._Bitmap = this._Bitmap.Copy();
            clone.OriginalBitmap = this.OriginalBitmap.Copy();

            return clone;
        }

        /// <summary>
        /// 画像ファイルを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void LoadImage(string path)
        {
            // ファイルが存在しない
            if (!File.Exists(path))
            {
                ShowWarningMessage("画像ファイルが見つかりません。\n" + path);
                return;
            }

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bitmap = SKBitmap.Decode(fs);

                    if (bitmap == null)
                    {
                        ShowWarningMessage("画像の読み込みに失敗しました。\n対応していない画像形式の可能性があります。");
                        return;
                    }

                    Bitmap = bitmap;
                }
            }
            catch (Exception ex)
            {
                ShowWarningMessage("画像の読み込み中にエラーが発生しました。\n" + ex.Message);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
            IsDisposed = true;

            if (OriginalBitmap != null)
            {
                OriginalBitmap.Dispose();
            }
        }


        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        /// <summary>
        /// マウスヒット中の図形（拡大した図形）を返す。
        /// </summary>
        internal override Object2D GetHitObject()
        {
            Image2D img = (Image2D)this.Clone();

            img.Scale = this.Scale * MouseHitBitmapOffset;

            return img;
        }

        /// <summary>
        /// マウスポイントがこの図形にヒットしているか判定する。
        /// </summary>
        internal override eMouseHitType IsHitMousePoint(PointF MousePoint, float MusePointRange)
        {
            eMouseHitType eMouseHitType;

            Vector2 _CenterPoint;
            float _CircumCircleR;

            PointF[] Points = GetBoundingBox();

            GraphicCaluculate.GetCenterPointAndCircumCircleR(Points, out _CenterPoint, out _CircumCircleR);

            eMouseHitType = CalIsHit.IsHitMouseRangeFillPolygon(_CenterPoint.ToPointF(), _CircumCircleR, Points.ToList(), MousePoint, MusePointRange);

            return eMouseHitType;
        }

        /// <summary>
        /// マウスポイントとこの図形の距離を取得する
        /// </summary>
        internal override float GetDistanceHitMousePoint(float X, float Y)
        {
            Vector2 p = new Vector2(X, Y);
            return Vector2.Distance(p, CenterPoint);
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        internal override void Move(PointF Movement)
        {
            X += Movement.X;
            Y += Movement.Y;
        }

        /// <summary>
        /// 図形を移動させる
        /// </summary>
        internal override void Move(float X, float Y)
        {
            this.X += X;
            this.Y += Y;
        }

        /// <summary>
        /// バウンディングボックスを取得する
        /// </summary>
        internal override PointF[] GetBoundingBox()
        {
            return CalBoundBox.GetBoundingBox(
                X, Y, Width, Height, Angle, eCalculateType.Grid, eRotateType.Center);
        }

        /// <summary>
        /// コピー元のObject2Dデータをコピー先にコピーする。
        /// </summary>
        protected new void BaseCopyDataTo(Object2D target)
        {
            base.BaseCopyDataTo(target);

            Image2D img = (Image2D)target;

            // 派生クラスのデータをコピー
            img.X = this.X;
            img.Y = this.Y;
            img.Angle = this.Angle;
            img._Scale = this._Scale;

            // Bitmap は参照コピー（必要なら DeepCopy に変更）
            img._Bitmap = this._Bitmap.Copy();
            img.OriginalBitmap = this.OriginalBitmap.Copy();
        }

        private void ShowWarningMessage(string text)
        {
            MessageBox.Show(
                text,
                "警告",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        /// <summary>
        /// ビットマップをBase64エンコードされたPNG文字列に変換する。
        /// </summary>
        /// <param name="bitmap">変換対象のSKBitmap</param>
        /// <returns>Base64エンコードされたPNG文字列。bitmapがnullの場合は空文字列。</returns>
        private static string BitmapToBase64(SKBitmap bitmap)
        {
            if (bitmap == null)
            {
                return string.Empty;
            }

            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return Convert.ToBase64String(data.ToArray());
            }
        }

        /// <summary>
        /// Base64エンコードされたPNG文字列をSKBitmapに変換する。
        /// </summary>
        /// <param name="base64">Base64エンコードされたPNG文字列</param>
        /// <returns>デコードされたSKBitmap。base64が空または無効な場合はnull。</returns>
        private static SKBitmap Base64ToBitmap(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return null;
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                return SKBitmap.Decode(bytes);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">出力先ドキュメント</param>
        public override void OutDocument(out Object2D_Document doc)
        {
            Image2D_Document imageDoc = new Image2D_Document();

            imageDoc.IsSelect = this.IsSelect;
            imageDoc.ZOrder   = this.ZOrder;
            imageDoc._ID      = this._ID;

            imageDoc.X     = this.X;
            imageDoc.Y     = this.Y;
            imageDoc.Angle = this.Angle;
            imageDoc.Scale = this._Scale;

            imageDoc.BitmapBase64 = BitmapToBase64(this.OriginalBitmap);

            doc = imageDoc;
        }

        /// <summary>
        /// ドキュメントデータを取り込む
        /// </summary>
        /// <param name="doc">取り込むドキュメント</param>
        public override void ImportDocument(in Object2D_Document doc)
        {
            Image2D_Document imageDoc = (Image2D_Document)doc;

            this.IsSelect = imageDoc.IsSelect;
            this.ZOrder   = imageDoc.ZOrder;
            this._ID      = imageDoc._ID;

            this.X     = imageDoc.X;
            this.Y     = imageDoc.Y;
            this.Angle = imageDoc.Angle;

            SKBitmap bitmap = Base64ToBitmap(imageDoc.BitmapBase64);
            if (bitmap != null)
            {
                // Bitmap セッターは OriginalBitmap をセットし _Scale を 1.0 に初期化する。
                // その後 _Scale と _Bitmap を保存値で上書きして元のスケールを復元する。
                this.Bitmap = bitmap;
                this._Scale = imageDoc.Scale;
                this._Bitmap = SKBitmapUtil.MakeScaleBitMap(this.OriginalBitmap, this._Scale);
            }
        }
    }
}