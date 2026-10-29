using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    public static class ResourceCache
    {
        private static readonly Dictionary<string, Resource> Resources = new();

        /// <summary>
        /// 指定されたパスのリソースを取得します。
        /// キャッシュに存在しない場合は新しく読み込みます。
        /// </summary>
        /// <typeparam name="T">リソースの型。</typeparam>
        /// <param name="path">リソースファイルのパス。</param>
        /// <returns>読み込まれたリソース。</returns>
        public static T Load<T>(string path) where T : Resource
        {
            string fullPath = Path.GetFullPath(path);

            if (Resources.TryGetValue(fullPath, out var cached))
            {
                if (cached is T resource)
                    return resource;

                throw new InvalidOperationException(
                    $"リソースの型が一致しません: {fullPath}");
            }

            T loaded = (T)ResourceCache.Load(fullPath, typeof(T));
            return loaded;
        }

        public static Resource Load(string path, Type resourceType)
        {
            string fullPath = Path.GetFullPath(path);

            if (Resources.TryGetValue(fullPath, out var resource))
            {
                if (!resourceType.IsInstanceOfType(resource))
                {
                    throw new InvalidOperationException(
                        $"リソースの型が一致しません: {fullPath}");
                }

                return resource;
            }

            Type serializerType =
                typeof(XmlResourceSerializer<>).MakeGenericType(resourceType);

            var serializer = Activator.CreateInstance(serializerType)
                ?? throw new InvalidOperationException(
                    $"シリアライザーを生成できません: {resourceType.FullName}");

            var method = serializerType.GetMethod(
                "Deserialize",
                new[] { typeof(string) })
                ?? throw new MissingMethodException(
                    serializerType.FullName, "Deserialize");

            var loaded = method.Invoke(serializer, new object[] { fullPath })
                as Resource
                ?? throw new InvalidOperationException(
                    $"リソースを読み込めませんでした: {fullPath}");

            Resources.Add(fullPath, loaded);
            return loaded;
        }

        /// <summary>
        /// 指定されたリソースをキャッシュから削除します。
        /// </summary>
        /// <param name="resource">削除するリソース。</param>
        public static void Remove(Resource resource)
        {
            if (resource == null)
                return;

            string fullPath = Path.GetFullPath(resource.Path);

            if (Resources.TryGetValue(fullPath, out var cachedResource) &&
                ReferenceEquals(cachedResource, resource))
            {
                Resources.Remove(fullPath);
            }
        }

    }

}
