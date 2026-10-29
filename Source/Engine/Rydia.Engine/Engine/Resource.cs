using System;
using System.Collections.Generic;
using System.Globalization;
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

        public void Save()
        {

        }

        public void Load()
        {

        }

        protected override void Disposing(bool disposing)
        {

        }

        protected override void Disposed(bool disposing)
        {
            base.Disposed(disposing);
        }

    }

}
