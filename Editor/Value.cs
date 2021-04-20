using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Yorozu.MethodExecute.EditorTools
{
    /// <summary>
    ///     1ParamType
    /// </summary>
    public class Value : IResetIndex
	{
		private const BindingFlags Flags = BindingFlags.Instance |
		                                   BindingFlags.Public |
		                                   BindingFlags.NonPublic;
		private readonly ArgBool _bools = new ArgBool();
		private readonly ArgColor Colors = new ArgColor();
		private readonly ArgFloat Floats = new ArgFloat();
		private readonly ArgInt Ints = new ArgInt();
		private readonly ArgString Strings = new ArgString();
		private readonly ArgVector2Int Vector2Ints = new ArgVector2Int();
		private readonly ArgVector2 Vector2s = new ArgVector2();
		private readonly ArgVector3Int Vector3Ints = new ArgVector3Int();
		private readonly ArgVector3 Vector3s = new ArgVector3();
        /// <summary>
        /// リストやクラスがあった場合はこれ
        /// </summary>
        private ArgValue Values = new ArgValue();
        private ArgArray Array = new ArgArray();

		private string _label;

		public Value(string label = "")
		{
			_label = label;
		}

		private static bool IsClassOrStruct(Type type)
		{
			return type.IsClass || type.IsValueType && !type.IsPrimitive;
		}

		internal bool Set(Type type, string name)
		{
			if (Ints.IsTarget(type))
			{
				Ints.Add(name);
			}
			else if (Floats.IsTarget(type))
			{
				Floats.Add(name);
			}
			else if (Strings.IsTarget(type))
			{
				Strings.Add(name, "");
			}
			else if (_bools.IsTarget(type))
			{
				_bools.Add(name);
			}
			else if (Vector2s.IsTarget(type))
			{
				Vector2s.Add(name);
			}
			else if (Vector3s.IsTarget(type))
			{
				Vector3s.Add(name);
			}
			else if (Vector2Ints.IsTarget(type))
			{
				Vector2Ints.Add(name);
			}
			else if (Vector3Ints.IsTarget(type))
			{
				Vector3Ints.Add(name);
			}
			else if (Colors.IsTarget(type))
			{
				Colors.Add(name);
			}
			else if (Array.IsTarget(type))
			{
				Array.Add(name, type);
			}

			// 上記でヒットしなかった
			else if (IsClassOrStruct(type))
			{
				var v = new Value(name + " (" + type.Name + ")");
				// フィールドから要素を構築
				foreach (var field in type.GetFields(Flags))
					v.Set(field.FieldType, field.Name);

				Values.Add(name, v);
			}
			else
			{
				Debug.LogError("Invalid Type: " + type.Name);

				return false;
			}

			return true;
		}

		public void ResetIndex()
		{
			_bools.ResetIndex();
			Floats.ResetIndex();
			Ints.ResetIndex();
			Strings.ResetIndex();
			Vector2s.ResetIndex();
			Vector3s.ResetIndex();
			Vector2Ints.ResetIndex();
			Vector3Ints.ResetIndex();
			Colors.ResetIndex();
			Values.ResetIndex();
			Array.ResetIndex();
		}

		internal object GetValue(Type type)
		{
			var val = GetTargetArg(type);

			if (val != null)
				return val;

			if (IsClassOrStruct(type))
			{
				var instance = Activator.CreateInstance(type);
				var v = Values.Get();
				foreach (var field in type.GetFields(Flags))
					field.SetValue(instance, v.GetValue(field.FieldType));

				return instance;
			}

			return null;
		}

		private object GetTargetArg(Type type)
		{
			if (Ints.IsTarget(type))
				return Ints.Get();

			if (Floats.IsTarget(type))
				return Floats.Get();

			if (Strings.IsTarget(type))
				return Strings.Get();

			if (_bools.IsTarget(type))
				return _bools.Get();

			if (Vector2s.IsTarget(type))
				return Vector2s.Get();

			if (Vector3s.IsTarget(type))
				return Vector3s.Get();

			if (Vector2Ints.IsTarget(type))
				return Vector2Ints.Get();

			if (Vector3Ints.IsTarget(type))
				return Vector3Ints.Get();

			if (Colors.IsTarget(type))
				return Colors.Get();

			if (Array.IsTarget(type))
				return Array.Get().GetValue(type);

			return null;
		}

		internal Value OnGUI()
		{
			if (!string.IsNullOrEmpty(_label))
			{
				EditorGUILayout.LabelField(_label);
			}

			using (new EditorGUI.IndentLevelScope())
			{
				_bools.OnGUI();
				Floats.OnGUI();
				Ints.OnGUI();
				Strings.OnGUI();
				Vector2s.OnGUI();
				Vector3s.OnGUI();
				Vector2Ints.OnGUI();
				Vector3Ints.OnGUI();
				Colors.OnGUI();
				Values.OnGUI();
				Array.OnGUI();
			}

			return this;
		}
	}
}
