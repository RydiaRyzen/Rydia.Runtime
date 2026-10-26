using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rydia.GameStudio
{
    public sealed class SceneObject
    {

        /// <summary>
        /// 名前を取得または設定します
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 位置を取得または設定します
        /// </summary>
        public float[] Position { get; } = { 0, 0, 0 };

        /// <summary>
        /// 回転を取得または設定します
        /// </summary>
        public float[] Rotation { get; } = { 0, 0, 0 };

        /// <summary>
        /// 拡大・縮小値を取得または設定します
        /// </summary>
        public float[] Scale { get; } = { 1, 1, 1 };

        public SceneObject(string name)
        {
            Name = name;
        }
    }

}
