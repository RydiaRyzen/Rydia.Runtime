using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.SolutionBuilder
{

    /// <summary>
    /// 外部プロジェクトへの参照情報を表します。
    /// </summary>
    public class ProjectReferenceDefinition
    {

        /// <summary>
        /// 指定されたアセンブリ名とプロジェクトファイルへのパスを使用して、
        /// <see cref="ProjectReferenceDefinition"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="asmName">参照先のアセンブリ名。</param>
        /// <param name="path">参照先のプロジェクトファイルへのパス。</param>
        public ProjectReferenceDefinition(string asmName, string path)
        {
            AsmName = asmName;
            Path = path;
        }

        /// <summary>
        /// 参照先のアセンブリ名を取得します。
        /// </summary>
        public string AsmName { get; }

        /// <summary>
        /// 参照先のプロジェクトファイルへのパスを取得します。
        /// </summary>
        public string Path { get; }

    }
}
