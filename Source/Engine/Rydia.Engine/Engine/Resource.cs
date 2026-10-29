using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using Rydia.Engine.Serialization;

namespace Rydia.Engine
{


    public abstract class Resource : DisposableBase
    {

        internal static readonly string FileExt = ".res";

        [DontSerialize]
        protected string path = null;

        public bool IsRuntimeResource
        {
            get { return string.IsNullOrEmpty(this.path); }
        }

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

        public string Path
        {
            get { return this.path; }
            internal set { this.path = value; }
        }

        /// <summary>
        /// Determines the name of a Resource based on its path.
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static string GetNameFromPath(string resPath)
        {
            if (string.IsNullOrEmpty(resPath))
                return "null";
            return System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(resPath));
        }

        /// <summary>
        /// Determines the full (hierarchical) name of a Resource based on its path.
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static string GetFullNameFromPath(string resPath)
        {
            if (string.IsNullOrEmpty(resPath)) return "null";
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(resPath), GetNameFromPath(resPath));
        }

        /// <summary>
        /// このリソースをファイルに保存します。
        /// </summary>
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



        public TResource Load<TResource>(string path)
            where TResource : Resource
        {
            var serializer = new XmlResourceSerializer<TResource>();
            TResource resource = ResourceCache.Load(path, typeof(TResource));
            
            serializer.Deserialize(path);

            if (resource == null)
            {
                throw new InvalidOperationException(
                    $"リソースを読み込めませんでした: {path}");
            }

            resource.Path = path;
            return resource;
        }

        protected override void Disposing(bool disposing)
        {
            
        }

        protected override void Disposed(bool disposing)
        {
            ResourceCache.Remove(this);
            base.Disposed(disposing);
        }

    }

}
