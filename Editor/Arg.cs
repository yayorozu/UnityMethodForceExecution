using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Yorozu.MethodExecute.EditorTools
{
	internal interface IResetIndex
	{
		void ResetIndex();
	}

	/// <summary>
	/// もう少し汎用化したい
	/// </summary>
	internal abstract class Arg<T>
	{
		private int _index;
		private readonly List<string> names = new List<string>(0);
		private readonly List<T> values = new List<T>(0);
		internal T this[int index]
		{
			get => values[index];
			set => values[index] = value;
		}
		internal int Length => values.Count;

		internal virtual bool IsTarget(Type type)
		{
			return type == typeof(T);
		}

		internal void Add(string name, T instance = default)
		{
			values.Add(instance);
			names.Add(name);
		}

		internal void Remove(int index)
		{
			values.RemoveAt(index);
		}

		internal void ResetIndex()
		{
			foreach (var value in values)
			{
				var i = value as IResetIndex;
				if (i != null)
					i.ResetIndex();
			}

			_index = 0;
		}

		internal T Get()
		{
			if (_index >= values.Count)
				return default;

			return values[_index++];
		}

		internal void OnGUI()
		{
			for (var i = 0; i < values.Count; i++)
			{
				values[i] = OnGUIElement(i, names[i], values[i]);
			}
		}

		protected abstract T OnGUIElement(int index, string name, T value);
	}

	internal class ArgInt : Arg<int>
	{
		internal override bool IsTarget(Type type)
		{
			return type == typeof(int) ||
			       type == typeof(uint);
		}

		protected override int OnGUIElement(int index, string name, int value)
		{
			return EditorGUILayout.IntField(name, value);
		}
	}

	internal class ArgBool : Arg<bool>
	{
		protected override bool OnGUIElement(int index, string name, bool value)
		{
			return EditorGUILayout.Toggle(name, value);
		}
	}

	internal class ArgFloat : Arg<float>
	{
		internal override bool IsTarget(Type type)
		{
			return type == typeof(float) ||
			       type == typeof(long);
		}

		protected override float OnGUIElement(int index, string name, float value)
		{
			return EditorGUILayout.FloatField(name, value);
		}
	}

	internal class ArgString : Arg<string>
	{
		protected override string OnGUIElement(int index, string name, string value)
		{
			return EditorGUILayout.TextField(name, value);
		}
	}

	internal class ArgVector2 : Arg<Vector2>
	{
		protected override Vector2 OnGUIElement(int index, string name, Vector2 value)
		{
			return EditorGUILayout.Vector2Field(name, value);
		}
	}

	internal class ArgVector3 : Arg<Vector3>
	{
		protected override Vector3 OnGUIElement(int index, string name, Vector3 value)
		{
			return EditorGUILayout.Vector3Field(name, value);
		}
	}

	internal class ArgVector2Int : Arg<Vector2Int>
	{
		protected override Vector2Int OnGUIElement(int index, string name, Vector2Int value)
		{
			return EditorGUILayout.Vector2IntField(name, value);
		}
	}

	internal class ArgVector3Int : Arg<Vector3Int>
	{
		protected override Vector3Int OnGUIElement(int index, string name, Vector3Int value)
		{
			return EditorGUILayout.Vector3IntField(name, value);
		}
	}

	internal class ArgColor : Arg<Color>
	{
		protected override Color OnGUIElement(int index, string name, Color value)
		{
			return EditorGUILayout.ColorField(name, value);
		}
	}

	internal class ArgValue : Arg<Value>
	{
		protected override Value OnGUIElement(int index, string name, Value value)
		{
			return value.OnGUI();
		}
	}

	internal class ArgArray : Arg<ArrayList>
	{
		internal override bool IsTarget(Type type)
		{
			return type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
		}

		internal void Add(string label, Type type)
		{
			Add("", new ArrayList(label, type));
		}

		protected override ArrayList OnGUIElement(int index, string name, ArrayList value)
		{
			return value.OnGUI();
		}
	}

	internal class ArgEnum : Arg<EnumValue>
	{
		internal override bool IsTarget(Type type)
		{
			return type.IsEnum;
		}

		internal void Add(Type type)
		{
			Add("", new EnumValue(type));
		}

		protected override EnumValue OnGUIElement(int index, string name, EnumValue value)
		{
			return value.OnGUI();
		}
	}
}
