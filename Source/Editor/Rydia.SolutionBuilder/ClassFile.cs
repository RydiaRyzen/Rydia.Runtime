using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rydia.SolutionBuilder
{

    /// <summary>
    /// C# クラスのソースファイルを表します。
    /// </summary>
    public sealed class ClassFile : SourceFile
    {
        private readonly List<PropertyDefinition> _properties = new();
        private readonly List<MethodDefinition> _methods = new();
        private readonly List<string> _usings = new();
        private readonly string? _baseClass;

        /// <summary>
        /// このクラスで使用する using ディレクティブの一覧を取得します。
        /// </summary>
        public IReadOnlyList<string> Usings
        {
            get
            {
                return this._usings;
            }
        }

        /// <summary>
        /// クラスに定義されているプロパティの一覧を取得します。
        /// </summary>
        public IReadOnlyList<PropertyDefinition> Properties
        {
            get
            {
                return this._properties;
            }
        }

        /// <summary>
        /// クラスに定義されているメソッドの一覧を取得します。
        /// </summary>
        public IReadOnlyList<MethodDefinition> Methods
        {
            get
            {
                return this._methods;
            }
        }

        /// <summary>
        /// <see cref="ClassFile"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">
        /// クラス名。
        /// </param>
        public ClassFile(string name)
            : base(name, name)
        {
        }

        /// <summary>
        /// <see cref="ClassFile"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">
        /// クラス名。
        /// </param>
        /// <param name="namespace">名前空間</param>
        public ClassFile(string name, string? @namespace, string baseClass = null)
            : base(name, @namespace)
        {
            this._baseClass = baseClass;
        }

        /// <summary>
        /// 指定した名前空間を using ディレクティブとして追加します。
        /// </summary>
        /// <param name="namespaceName">名前空間名。</param>
        public void AddUsing(string namespaceName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(namespaceName);

            if (!this._usings.Contains(namespaceName))
            {
                this._usings.Add(namespaceName);
            }
        }

        /// <summary>
        /// 指定した型のプロパティをクラスに追加します。
        /// </summary>
        /// <param name="name">
        /// プロパティ名。
        /// </param>
        /// <param name="type">
        /// プロパティの型。
        /// </param>
        public void AddProperty(
            string name,
            string type)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(type);

            this._properties.Add(
                new PropertyDefinition(
                    name,
                    type));
        }

        public void AddProperty(string name, string type, string code)
        {
            this._properties.Add(
                new PropertyDefinition(
                    name,
                    type, code));
        }

        /// <summary>
        /// 指定した戻り値の型を持つメソッドをクラスに追加します。
        /// </summary>
        /// <param name="name">
        /// メソッド名。
        /// </param>
        /// <param name="returnType">
        /// メソッドの戻り値の型。
        /// </param>
        public void AddMethod(
            string name,
            string body,
            string returnType = "void")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(returnType);

            var method = new MethodDefinition(name, returnType);
            method.Body = body;
            this._methods.Add(method);
        }

        /// <summary>
        /// Load メソッドを追加します。
        /// </summary>
        /// <param name="body">メソッド本体。</param>
        public void AddOverrideMethod(string name, string body = "")
        {
            AddLifecycleMethod(name, body);
        }

        public void AddOverrideMethod(string name,
            string body,
            params (string Type, string Name)[] parameters)
        {
            AddLifecycleMethod(name, body, parameters);
        }

        /// <summary>
        /// ライフサイクルメソッドを追加します。
        /// </summary>
        /// <param name="name">メソッド名。</param>
        /// <param name="body">メソッド本体。</param>
        /// <param name="parameters">引数の型と名前。</param>
        private void AddLifecycleMethod(
            string name,
            string body,
            params (string Type, string Name)[] parameters)
        {
            if (this._methods.Any(method => method.Name == name))
            {
                throw new InvalidOperationException(
                    $"The method '{name}' has already been added.");
            }

            var method = new MethodDefinition(name, "override void")
            {
                Body = body
            };

            foreach (var parameter in parameters)
            {
                method.Parameters.Add(parameter);
            }

            this._methods.Add(method);
        }

        /// <summary>
        /// クラスの C# ソースコードを生成します。
        /// </summary>
        /// <returns>
        /// 生成された C# ソースコード。
        /// </returns>
        public override string Generate()
        {
            var sb = new StringBuilder();

            foreach (var usingNamespace in this._usings)
            {
                sb.AppendLine($"using {usingNamespace};");
            }

            if (this._usings.Count > 0)
            {
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                sb.AppendLine($"namespace {Namespace};");
                sb.AppendLine();
            }

            sb.AppendLine($"public class {Name}");
            if (!string.IsNullOrWhiteSpace(this._baseClass))
            {
                sb.Append(" : ");
                sb.Append(this._baseClass);
            }
            sb.AppendLine("{");

            foreach (var property in this._properties)
            {
                sb.AppendLine(
                    $"    public {property.Type} {property.Name} {{ {property.Code} }}");
            }

            if (this._properties.Count > 0 && this._methods.Count > 0)
            {
                sb.AppendLine();
            }

            for (int i = 0; i < this._methods.Count; i++)
            {
                var method = this._methods[i];

                string parameters = string.Join(
                    ", ",
                    method.Parameters.Select(
                        parameter => $"{parameter.Type} {parameter.Name}"));

                sb.AppendLine(
                    $"    public {method.ReturnType} {method.Name}({parameters})");
                sb.AppendLine("    {");

                if (!string.IsNullOrWhiteSpace(method.Body))
                {
                    foreach (string line in method.Body
                        .Replace("\r\n", "\n")
                        .Split('\n'))
                    {
                        sb.AppendLine($"        {line}");
                    }
                }

                sb.AppendLine("    }");

                if (i < this._methods.Count - 1)
                {
                    sb.AppendLine();
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
            /*
            var sb = new StringBuilder();

            foreach (var usingNamespace in this._usings)
            {
                sb.AppendLine($"using {usingNamespace};");
            }

            if (this._usings.Count > 0)
            {
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                sb.AppendLine($"namespace {Namespace};");
                sb.AppendLine();
            }

            sb.AppendLine($"public class {Name}");
            sb.AppendLine("{");

            foreach (var property in this._properties)
            {
                sb.AppendLine(
                    $"    public {property.Type} {property.Name} {{ {property.Code} }}");
            }

            if (this._properties.Count > 0 &&
                this._methods.Count > 0)
            {
                sb.AppendLine();
            }

            foreach (var method in this._methods)
            {
                sb.AppendLine(
                    $"    public {method.ReturnType} {method.Name}()");
                sb.AppendLine("    {");

                if (!string.IsNullOrWhiteSpace(method.Body))
                {
                    foreach (string line in method.Body.Replace("\r\n", "\n").Split('\n'))
                    {
                        sb.AppendLine($"        {line}");
                    }
                }

                sb.AppendLine("    }");
                sb.AppendLine();
            }

            sb.AppendLine("}");

            return sb.ToString();
            */
        }

    }

}
