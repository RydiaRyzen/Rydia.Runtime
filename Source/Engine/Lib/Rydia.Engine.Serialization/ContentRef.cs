using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// コンテンツ参照を表すクラスです
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ContentRef<T> : IContentRef
        where T : Resource
    {

        private string _path;

        public ContentRef(string path)
        {
            this._path = path;
        }

        private ContentRef(T t)
        {
            Res = t;
            this._path = t.Path;
        }

        public string Path
        {
            get
            {
                return IsLoaded ? Res.Path : this._path;
            }
        }

        public bool IsLoaded
        {
            get;
        }

        public T Res
        {
            get;
            private set;
        }

        Resource IContentRef.Res
        {
            get
            {
                return Res;
            }
        }

        public static implicit operator T(ContentRef<T> contentRef)
        {
            return contentRef.Res;
        }

        public static implicit operator ContentRef<T>(T @ref)
        {
            return new ContentRef<T>(@ref);
        }
    }
}
