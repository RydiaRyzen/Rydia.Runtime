using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using Rydia.Engine.Serialization;

namespace Rydia.Engine
{

    /// <summary>
    /// リソースの基底クラスを表します。
    /// </summary>
    public abstract class Resource : DisposableBase
    {

        /// <summary>
        /// リソースファイルの既定の拡張子を表します。
        /// </summary>
        internal static readonly string FileExt = ".res";

        /// <summary>
        /// リソースファイルのパスを保持します。
        /// </summary>
        [DontSerialize]
        protected string path = null;

        /// <summary>
        /// このリソースが実行時に生成されたリソースであるかどうかを取得します。
        /// </summary>
        /// <value>
        /// リソースファイルのパスが設定されていない場合は <see langword="true"/>、
        /// 設定されている場合は <see langword="false"/>。
        /// </value>
        public bool IsRuntimeResource
        {
            get { return string.IsNullOrEmpty(this.path); }
        }

        /// <summary>
        /// リソースの名前を取得します。
        /// </summary>
        /// <value>
        /// 実行時リソースの場合はハッシュコードを文字列に変換した値、
        /// ファイルリソースの場合はファイル名から拡張子を除いた名前。
        /// </value>
        public string Name
        {
            get
            {
                if (IsRuntimeResource)
                    return GetHashCode().ToString(CultureInfo.InvariantCulture);
                else
                    return Resource.GetNameFromPath(this.path);
            }
        }

        /// <summary>
        /// リソースの階層を含む完全な名前を取得します。
        /// </summary>
        /// <value>
        /// 実行時リソースの場合はハッシュコードを文字列に変換した値、
        /// ファイルリソースの場合はファイルパスから生成した完全な名前。
        /// </value>
        public string FullName
        {
            get
            {
                if (IsRuntimeResource)
                    return GetHashCode().ToString(CultureInfo.InvariantCulture);
                else
                    return Resource.GetFullNameFromPath(this.path);
            }
        }

        /// <summary>
        /// リソースファイルのパスを取得または設定します。
        /// </summary>
        /// <value>リソースファイルのパス。</value>
        public string Path
        {
            get { return this.path; }
            internal set { this.path = value; }
        }

        /// <summary>
        /// 指定されたパスからリソースの名前を取得します。
        /// </summary>
        /// <param name="resPath">リソースファイルのパス。</param>
        /// <returns>
        /// ファイル名から拡張子を除いた名前。
        /// パスが <see langword="null"/> または空文字列の場合は
        /// <c>"null"</c>。
        /// </returns>
        public static string GetNameFromPath(string resPath)
        {
            if (string.IsNullOrEmpty(resPath))
                return "null";
            return System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(resPath));
        }

        /// <summary>
        /// 指定されたパスから階層を含むリソースの完全な名前を取得します。
        /// </summary>
        /// <param name="resPath">リソースファイルのパス。</param>
        /// <returns>
        /// ディレクトリパスと、拡張子を除いたリソース名を結合した文字列。
        /// パスが <see langword="null"/> または空文字列の場合は
        /// <c>"null"</c>。
        /// </returns>
        public static string GetFullNameFromPath(string resPath)
        {
            if (string.IsNullOrEmpty(resPath)) return "null";
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(resPath), GetNameFromPath(resPath));
        }

        /// <summary>
        /// このリソースを、設定されているパスのファイルに保存します。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 保存先のパスが設定されていない場合にスローされます。
        /// </exception>
        public void Save()
        {
            if (string.IsNullOrWhiteSpace(Path))
            {
                throw new InvalidOperationException(
                    "リソースの保存先パスが設定されていません。");
            }
            Save(Path);
        }

        /// <summary>
        /// このリソースを指定されたファイルに保存します。
        /// </summary>
        /// <param name="path">保存先ファイルのパス。</param>
        /// <exception cref="ArgumentException">
        /// 保存先のパスが <see langword="null"/>、空文字列、
        /// または空白文字のみの場合にスローされます。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// シリアライザーを生成できない場合にスローされます。
        /// </exception>
        /// <exception cref="MissingMethodException">
        /// 対応する <c>Serialize</c> メソッドが見つからない場合に
        /// スローされます。
        /// </exception>
        public void Save(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(
                    "保存先パスを指定してください。",
                    nameof(path));
            }

            var serializerType = typeof(XmlResourceSerializer<>)
                .MakeGenericType(GetType());

            var serializer = Activator.CreateInstance(serializerType)
                ?? throw new InvalidOperationException(
                    $"シリアライザーを生成できません: {GetType().FullName}");

            var method = serializerType.GetMethod(
                "Serialize",
                new[] { GetType(), typeof(string) })
                ?? throw new MissingMethodException(
                    serializerType.FullName,
                    "Serialize");

            method.Invoke(serializer, new object[] { this, path });

            Path = path;
        }

        /// <summary>
        /// このリソースに設定されているパスから、指定された型のリソースを読み込みます。
        /// </summary>
        /// <typeparam name="TResource">
        /// 読み込むリソースの型。
        /// </typeparam>
        /// <returns>読み込まれたリソース。</returns>
        /// <exception cref="InvalidOperationException">
        /// リソースの保存先パスが設定されていない場合にスローされます。
        /// </exception>
        public TResource Load<TResource>()
            where TResource : Resource
        {
            if (string.IsNullOrWhiteSpace(Path))
            {
                throw new InvalidOperationException(
                    "リソースの保存先パスが設定されていません。");
            }
            return Load<TResource>(Path);
        }

        /// <summary>
        /// 指定されたパスから、指定された型のリソースを読み込みます。
        /// </summary>
        /// <typeparam name="TResource">
        /// 読み込むリソースの型。
        /// </typeparam>
        /// <param name="path">読み込むリソースファイルのパス。</param>
        /// <returns>読み込まれたリソース。</returns>
        /// <exception cref="InvalidOperationException">
        /// リソースの保存先パスが設定されていない場合、
        /// またはリソースを読み込めなかった場合にスローされます。
        /// </exception>
        public TResource Load<TResource>(string path)
            where TResource : Resource
        {
            TResource resource = (TResource)ResourceCache.Load(path, typeof(TResource));
            
            if (resource == null)
            {
                throw new InvalidOperationException(
                    $"リソースを読み込めませんでした: {path}");
            }

            resource.Path = path;
            return resource;
        }

        /// <summary>
        /// リソースの破棄処理を実行します。
        /// </summary>
        /// <param name="disposing">
        /// マネージドリソースも破棄する場合は <see langword="true"/>。
        /// </param>
        protected override void Disposing(bool disposing)
        {
            
        }

        /// <summary>
        /// リソースの破棄完了後に実行される処理です。
        /// リソースキャッシュからこのリソースを削除します。
        /// </summary>
        /// <param name="disposing">
        /// マネージドリソースも破棄した場合は <see langword="true"/>。
        /// </param>
        protected override sealed void Disposed(bool disposing)
        {
            ResourceCache.Remove(this);
            base.Disposed(disposing);
        }

    }

}
