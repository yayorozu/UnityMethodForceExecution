using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Yorozu.MethodExecute.EditorTools
{
    public class Value : IResetIndex
	{
		private const BindingFlags Flags = BindingFlags.Instance |
		                                   BindingFlags.Public |
		                                   BindingFlags.NonPublic;

		private readonly ArgBool _bools = new ArgBool();
		private readonly ArgColor _colors = new ArgColor();
		private readonly ArgFloat _floats = new ArgFloat();
		private readonly ArgInt _ints = new ArgInt();
		private readonly ArgString _strings = new ArgString();
		private readonly ArgEnum _enums = new ArgEnum();
		private readonly ArgVector2Int _vector2Ints = new ArgVector2Int();
		private readonly ArgVector2 _vector2s = new ArgVector2();
		private readonly ArgVector3Int _vector3Ints = new ArgVector3Int();
		private readonly ArgVector3 _vector3s = new ArgVector3();

        private readonly ArgValue _values = new ArgValue();
        private readonly ArgArray _arrayes = new ArgArray();

		private readonly string _label;

		public Value(string label = "")
		{
			_label = label;
		}

		private static bool IsClassOrStruct(Type type)
		{
			return type.IsClass || type.IsValueType && !type.IsPrimitive;
		}

		internal void Set(Type type, string name)
		{
			if (_ints.IsTarget(type))
			{
				_ints.Add(name);
			}
			else if (_floats.IsTarget(type))
			{
				_floats.Add(name);
			}
			else if (_strings.IsTarget(type))
			{
				_strings.Add(name, "");
			}
			else if (_bools.IsTarget(type))
			{
				_bools.Add(name);
			}
			else if (_enums.IsTarget(type))
			{
				_enums.Add(type);
			}
			else if (_vector2s.IsTarget(type))
			{
				_vector2s.Add(name);
			}
			else if (_vector3s.IsTarget(type))
			{
				_vector3s.Add(name);
			}
			else if (_vector2Ints.IsTarget(type))
			{
				_vector2Ints.Add(name);
			}
			else if (_vector3Ints.IsTarget(type))
			{
				_vector3Ints.Add(name);
			}
			else if (_colors.IsTarget(type))
			{
				_colors.Add(name);
			}
			else if (_arrayes.IsTarget(type))
			{
				_arrayes.Add(name, type);
			}

			// 上記でヒットしなかった
			else if (IsClassOrStruct(type))
			{
				var v = new Value(name);
				// フィールドから要素を構築
				foreach (var field in type.GetFields(Flags))
					v.Set(field.FieldType, field.Name);

				_values.Add(name, v);
			}
			else
			{
				Debug.LogError("Invalid Type: " + type.Name);
			}
		}

		public void ResetIndex()
		{
			_bools.ResetIndex();
			_floats.ResetIndex();
			_ints.ResetIndex();
			_strings.ResetIndex();
			_enums.ResetIndex();
			_vector2s.ResetIndex();
			_vector3s.ResetIndex();
			_vector2Ints.ResetIndex();
			_vector3Ints.ResetIndex();
			_colors.ResetIndex();
			_values.ResetIndex();
			_arrayes.ResetIndex();
		}

		internal object GetValue(Type type)
		{
			var val = GetTargetArg(type);

			if (val != null)
				return val;

			// Nullable
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var v = _values.Get();
				// GetHasValue
				var hasValue = (bool) v.GetValue(typeof(bool));
				if (!hasValue)
					return null;

				var baseType = Nullable.GetUnderlyingType(type);
				var genericType = typeof(Nullable<>).MakeGenericType(baseType);
				return Activator.CreateInstance(genericType, v.GetValue(baseType));
			}

			if (IsClassOrStruct(type))
			{
				var instance = Activator.CreateInstance(type);
				var v = _values.Get();
				foreach (var field in type.GetFields(Flags))
					field.SetValue(instance, v.GetValue(field.FieldType));

				return instance;
			}

			return null;
		}

		private object GetTargetArg(Type type)
		{
			if (_ints.IsTarget(type))
				return _ints.Get();

			if (_floats.IsTarget(type))
				return _floats.Get();

			if (_strings.IsTarget(type))
				return _strings.Get();

			if (_bools.IsTarget(type))
				return _bools.Get();

			if (_enums.IsTarget(type))
				return _enums.Get().GetValue();

			if (_vector2s.IsTarget(type))
				return _vector2s.Get();

			if (_vector3s.IsTarget(type))
				return _vector3s.Get();

			if (_vector2Ints.IsTarget(type))
				return _vector2Ints.Get();

			if (_vector3Ints.IsTarget(type))
				return _vector3Ints.Get();

			if (_colors.IsTarget(type))
				return _colors.Get();

			if (_arrayes.IsTarget(type))
				return _arrayes.Get().GetValue(type);

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
				_floats.OnGUI();
				_ints.OnGUI();
				_strings.OnGUI();
				_enums.OnGUI();
				_vector2s.OnGUI();
				_vector3s.OnGUI();
				_vector2Ints.OnGUI();
				_vector3Ints.OnGUI();
				_colors.OnGUI();
				_values.OnGUI();
				_arrayes.OnGUI();
			}

			return this;
		}
	}
}
