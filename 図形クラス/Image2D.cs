using graphicbox2d.グラフィック計算;
using graphicbox2d.グローバル変数;
using graphicbox2d.その他;
using Newtonsoft.Json.Linq;
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
            get { return _Scale; }
            set { _Scale = value; UpdateDrawBitmap(); }
        }
        private float _Scale = 1.0f;


        /// <summary>
        /// 幅
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { _Width = value; UpdateDrawBitmap(); }
        }
        private float _Width = 1.0f;

        /// <summary>
        /// 高さ
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { _Height = value; UpdateDrawBitmap(); }
        }
        private float _Height = 1.0f;

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
                if (_OriginalBitmap != null)
                {
                    _OriginalBitmap.Dispose();
                    _Bitmap.Dispose();
                    _HitBitmap.Dispose();
                }

                if (value == null)
                {
                    _OriginalBitmap = null;
                    _Bitmap = null;
                    _HitBitmap = null;
                    return;
                }

                _OriginalBitmap = value;
                UpdateDrawBitmap();
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
        private SKBitmap _OriginalBitmap = null;

        /// <summary>
        /// ヒット中ビットマップ
        /// </summary>
        private SKBitmap _HitBitmap = null;

        /// <summary>
        /// ユーザーズーム
        /// </summary>
        internal float CashUserZoom = Graphic2DControl.UserZoom;

        /// <summary>
        /// 図形の中心点
        /// </summary>
        internal override Vector2 CenterPoint => GetCenterPoint();

        internal int ClientWidth => (int)CalConvert.ConvertGridLengthToClientLength(_Width);

        internal int ClientHeight => (int)CalConvert.ConvertGridLengthToClientLength(_Height);

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
            if (_OriginalBitmap != null)
            {
                _OriginalBitmap.Dispose();
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
            clone._OriginalBitmap = this._OriginalBitmap.Copy();
            clone._HitBitmap = this._HitBitmap.Copy();

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

            if (_OriginalBitmap != null)
            {
                _OriginalBitmap.Dispose();
                _Bitmap.Dispose();
                _HitBitmap.Dispose();
            }
        }

        // ===============================================================================
        // 非公開メソッド
        // ===============================================================================

        internal SKBitmap GetDrawBitmap()
        {
            if (_Bitmap == null)
            {
                return null;
            }

            if (Graphic2DControl.UserZoom == CashUserZoom)
            {
                return _Bitmap;
            }
            else
            {
                // ユーザーズームが変わっている場合はビットマップを更新してから返す
                CashUserZoom = Graphic2DControl.UserZoom;

                UpdateDrawBitmap();
                return _Bitmap;
            }
        }

        internal SKBitmap GetDrawHitBitmap()
        {
            if (_HitBitmap == null)
            {
                return null;
            }
            if (Graphic2DControl.UserZoom == CashUserZoom)
            {
                return _HitBitmap;
            }
            else
            {
                // ユーザーズームが変わっている場合はビットマップを更新してから返す
                CashUserZoom = Graphic2DControl.UserZoom;

                UpdateDrawBitmap();
                return _HitBitmap;
            }
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
            img._Width = this._Width;
            img._Height = this._Height;

            // Bitmap は参照コピー（必要なら DeepCopy に変更）
            img._Bitmap = this._Bitmap.Copy();
            img._OriginalBitmap = this._OriginalBitmap.Copy();
            img._HitBitmap = this._HitBitmap.Copy();
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

        private void UpdateDrawBitmap()
        {

            if (_Bitmap != null)
            {
                _Bitmap.Dispose();
                _Bitmap = null;
            }

            if (_HitBitmap != null)
            {
                _HitBitmap.Dispose();
                _HitBitmap = null;
            }

            if(_OriginalBitmap == null)
            {
                return;
            }

            _Bitmap = SKBitmapUtil.MakeResizeBitMap(_OriginalBitmap, ClientWidth, ClientHeight, Graphic2DControl.UserZoom * _Scale);
            _HitBitmap = SKBitmapUtil.MakeScaleBitMap(_Bitmap, MouseHitBitmapOffset);
        }

        /// <summary>
        /// ドキュメントデータを書き出す
        /// </summary>
        /// <param name="doc">出力先ドキュメント</param>
        public override void OutDocument(out Object2D_Document doc)
        {
            Image2D_Document imageDoc = new Image2D_Document();

            imageDoc.IsSelect = this.IsSelect;
            imageDoc.ZOrder = this.ZOrder;
            imageDoc._ID = this._ID;

            imageDoc.X = this.X;
            imageDoc.Y = this.Y;
            imageDoc.Angle = this.Angle;
            imageDoc._Scale = this._Scale;
            imageDoc.Width = this._Width;
            imageDoc.Height = this._Height;

            imageDoc.OriginalBitmapBase64 = SKBitmapUtil.SKBitmapToBase64(this._OriginalBitmap);

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
            this.ZOrder = imageDoc.ZOrder;
            this._ID = imageDoc._ID;

            this.X = imageDoc.X;
            this.Y = imageDoc.Y;
            this.Angle = imageDoc.Angle;
            this._Scale = imageDoc._Scale;
            this._Width = imageDoc.Width;
            this._Height = imageDoc.Height;

            this._OriginalBitmap = SKBitmapUtil.Base64ToSKBitmap(imageDoc.OriginalBitmapBase64);

            if (this._OriginalBitmap != null)
            {
                UpdateDrawBitmap();
            }
        }
    }
}