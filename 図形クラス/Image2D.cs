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
            get { return GetBitmapValue(eGetValueType.Scale); }
            set { SetBitmapValue(eSetValueType.Scale, value); }
        }
        private float _Scale = 1.0f;


        /// <summary>
        /// 幅
        /// </summary>
        public float Width 
        {
            get { return GetBitmapValue(eGetValueType.Width); }
            set {SetBitmapValue(eSetValueType.Width, value); }
        }

        /// <summary>
        /// 高さ
        /// </summary>
        public float Height
        {
            get { return GetBitmapValue(eGetValueType.Height); }
            set { SetBitmapValue(eSetValueType.Height, value); }
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
                    _Bitmap.Dispose();
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

        enum eSetValueType
        {
            Scale,
            Width,
            Height,
        }

        enum eGetValueType
        {
            Scale,
            Width,
            Height,
            ClientWidth,
            ClientHeight,
        }

        /// <summary>
        /// マウスヒット中の拡大率
        /// </summary>
        private float MouseHitBitmapOffset = 1.05f;

        /// <summary>
        /// オリジナルのビットマップ
        /// </summary>
        private SKBitmap OriginalBitmap = null;

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint => GetCenterPoint();

        internal int ClientWidth
        {
            get { return (int)GetBitmapValue(eGetValueType.ClientWidth);}
        }

        internal int ClientHeight
        {
            get { return (int)GetBitmapValue(eGetValueType.ClientHeight);}
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
        /// 以下の形式に対応している
        /// PNG / JPEG / WEBP / GIF / BMP / ICO / WBMP
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
                _Bitmap.Dispose();
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

        /// <summary>
        /// 中心点を取得する
        /// </summary>
        /// <returns></returns>
        internal Vector2 GetCenterPoint()
        {
            if (Bitmap == null)
            {
                return default;
            }

            PointF[] Points = GetBoundingBox();

            return GraphicCaluculate.CaluculateCenterPoint(Points);
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

        private void SetBitmapValue(eSetValueType type, float value)
        {
            if (_Bitmap == null)
            {
                return;
            }

            switch (type)
            {
                case eSetValueType.Scale:
                    _Scale = value;
                    _Bitmap = SKBitmapUtil.MakeResizeBitMap(OriginalBitmap, ClientWidth, ClientHeight, _Scale);
                    break;
                case eSetValueType.Width:
                    int clientWidth = CalConvert.ConvertGridLengthToClientLength(value);
                    _Bitmap = SKBitmapUtil.MakeResizeBitMap(OriginalBitmap, clientWidth, ClientHeight, Scale);
                    break;
                case eSetValueType.Height:
                    int clientHeight = CalConvert.ConvertGridLengthToClientLength(value);
                    _Bitmap = SKBitmapUtil.MakeResizeBitMap(OriginalBitmap, ClientWidth, clientHeight, Scale);
                    break;
            }
        }

        private float GetBitmapValue(eGetValueType type)
        {
            float value = 0;
            if (_Bitmap == null)
            {
                return value;
            }

            switch (type)
            {
                case eGetValueType.Scale:
                    value = _Scale;
                    break;
                case eGetValueType.Width:
                    value = CalConvert.ConvertClientLengthToGridLength(_Bitmap.Width);
                    break;
                case eGetValueType.Height:
                    value = CalConvert.ConvertClientLengthToGridLength(_Bitmap.Height);
                    break;
                case eGetValueType.ClientWidth:
                    value = _Bitmap.Width;
                    break;
                case eGetValueType.ClientHeight:
                    value = _Bitmap.Height;
                    break;
            }

            return value;
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
            imageDoc._Scale = this._Scale;

            imageDoc._BitmapBase64 = SKBitmapUtil.SKBitmapToBase64(this._Bitmap);
            imageDoc.OriginalBitmapBase64 = SKBitmapUtil.SKBitmapToBase64(this.OriginalBitmap);

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
            this._Scale = imageDoc._Scale;

            this._Bitmap = SKBitmapUtil.Base64ToSKBitmap(imageDoc._BitmapBase64);
            this.OriginalBitmap = SKBitmapUtil.Base64ToSKBitmap(imageDoc.OriginalBitmapBase64);
        }
    }
}