using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicbox2d.その他
{



    /// <summary>
    /// スナップモードの各ステップで使用するデータをまとめたクラス
    /// </summary>
    internal class SnapModeDataPack
    {
        /// <summary>
        /// 現在のスナップモードステップ
        /// </summary>
        public eMode_Snap CurrentStep = eMode_Snap.Step1;

        /// <summary>
        /// STEP1のデータセット
        /// </summary>
        internal SnapDataSet_Step1 Step1 = new SnapDataSet_Step1();

        /// <summary>
        /// STEP2のデータセット
        /// </summary>
        internal SnapDataSet_Step2 Step2 = new SnapDataSet_Step2();

        /// <summary>
        /// STEP3のデータセット
        /// </summary>
        internal SnapDataSet_Step3 Step3 = new SnapDataSet_Step3();
    }



    /// <summary>
    /// スナップモードステップ1のデータセットクラス
    /// </summary>
    internal class SnapDataSet_Step1
    {
    }

    /// <summary>
    /// スナップモードステップ2のデータセットクラス
    /// </summary>
    internal class SnapDataSet_Step2
    {
        /// <summary>
        /// スナップ実行中のオブジェクト
        /// </summary>
        internal Object2D SnappingObject = null;

        /// <summary>
        /// マウスヒット中のスナップポイント（グリッド座標系）
        /// </summary>
        internal PointF? HitSnapPoint = null;
    }

    /// <summary>
    /// スナップモードステップ3のデータセットクラス
    /// </summary>
    internal class SnapDataSet_Step3
    {
        /// <summary>
        /// スナップ実行中のオブジェクト
        /// </summary>
        internal Object2D SnappingObject = null;

        /// <summary>
        /// STEP2で選択されたスナップポイント（グリッド座標系）
        /// </summary>
        internal PointF? SelectSnapPoint = null;

        /// <summary>
        /// マウスから最も近いグリッドポイントまたはスナップポイント（グリッド座標系）
        internal PointF? NearSnapPoint = null;
    }
}
