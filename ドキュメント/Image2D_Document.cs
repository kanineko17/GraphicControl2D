using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d
{
    /// <summary>
    /// 画像図形ドキュメントクラス
    /// </summary>
    public class Image2D_Document : Object2D_Document
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
        public float _Scale { get; set; } = 1.0f;

        /// <summary>
        /// ビットマップの幅
        /// </summary>
        public float Width { get; set; } = 0;

        /// <summary>
        /// ビットマップの高さ
        /// </summary>
        public float Height { get; set; } = 0;


        /// <summary>
        /// オリジナルビットマップのBase64エンコードされたPNG画像データ
        /// </summary>
        public string OriginalBitmapBase64 { get; set; } = string.Empty;
    }
}
