using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    public static class ResourceCache
    {
        private static readonly Dictionary<string, Resource> Resources = new();

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
    }

}
