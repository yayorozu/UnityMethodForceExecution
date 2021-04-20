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
	public abstract class Arg<T>
	{
		internal T this[int index]
		{
			get => values[index];
			set => values[index] = value;
		}

		private List<T> values = new List<T>(0);
		private List<string> names = new List<string>(0);

		private int _index;
		internal int Length => values.Count;

		public virtual bool IsTarget(Type type)
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
				{
					i.ResetIndex();
				}
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
				values[i] = OnGUIElement(names[i], values[i]);
			}
		}

		protected abstract T OnGUIElement(string name, T value);
	}

	public class ArgInt : Arg<int>
    {
        public override bool IsTarget(Type type)
        {
            return type == typeof(int) ||
                   type == typeof(uint) ||
                   type.IsEnum;
        }

        protected override int OnGUIElement(string name, int value)
        {
            return EditorGUILayout.IntField(name, value);
        }
    }

    public class ArgBool : Arg<bool>
    {
        protected override bool OnGUIElement(string name, bool value)
        {
            return EditorGUILayout.Toggle(name, value);
        }
    }

    public class ArgFloat : Arg<float>
    {
        public override bool IsTarget(Type type)
        {
            return type == typeof(float) ||
                type == typeof(long);
        }

        protected override float OnGUIElement(string name, float value)
        {
            return EditorGUILayout.FloatField(name, value);
        }
    }

    public class ArgString : Arg<string>
    {
        protected override string OnGUIElement(string name, string value)
        {
            return EditorGUILayout.TextField(name, value);
        }
    }

    public class ArgVector2 : Arg<Vector2>
    {
        protected override Vector2 OnGUIElement(string name, Vector2 value)
        {
            return EditorGUILayout.Vector2Field(name, value);
        }
    }

    public class ArgVector3 : Arg<Vector3>
    {
        protected override Vector3 OnGUIElement(string name, Vector3 value)
        {
            return EditorGUILayout.Vector3Field(name, value);
        }
    }

    public class ArgVector2Int : Arg<Vector2Int>
    {
        protected override Vector2Int OnGUIElement(string name, Vector2Int value)
        {
            return EditorGUILayout.Vector2IntField(name, value);
        }
    }

    public class ArgVector3Int : Arg<Vector3Int>
    {
        protected override Vector3Int OnGUIElement(string name, Vector3Int value)
        {
            return EditorGUILayout.Vector3IntField(name, value);
        }
    }

    public class ArgColor : Arg<Color>
    {
        protected override Color OnGUIElement(string name, Color value)
        {
            return EditorGUILayout.ColorField(name, value);
        }
    }

    public class ArgValue : Arg<Value>
    {
	    protected override Value OnGUIElement(string name, Value value)
	    {
		    return value.OnGUI();
	    }
    }

    public class ArgArray : Arg<ArrayList>
    {
	    public override bool IsTarget(Type type)
	    {
		    return type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
	    }

	    internal void Add(string label, Type type)
	    {
		    var elementType = type.IsArray ?
			    type.GetElementType() :
			    type.GetGenericArguments()[0];

		    Add("", new ArrayList(label + " (" + elementType.Name + ")", elementType));
	    }

	    protected override ArrayList OnGUIElement(string name, ArrayList value)
	    {
		    return value.OnGUI();
	    }
    }

    /// <summary>
    /// Array もしくは List のキャッシュ用
    /// </summary>
    [Serializable]
    public class ArrayList : IResetIndex
    {
	    private string _label;
	    private Type _elementType;

	    public ArrayList(string label, Type type)
	    {
		    _label = label;
		    _elementType = type;
	    }

	    /// <summary>
	    /// Arrayの中身
	    /// </summary>
	    private ArgValue Values = new ArgValue();

	    public ArrayList OnGUI()
	    {
		    // Header
		    using (new GUILayout.HorizontalScope())
		    {
			    EditorGUILayout.LabelField(string.Format("{0} [{1}]", _label, Values.Length));
			    GUILayout.FlexibleSpace();
			    // 要素の追加
			    if (GUILayout.Button(EditorGUIUtility.TrIconContent("Toolbar Plus"), "RL FooterButton", GUILayout.Width(16)))
			    {
				    var v = new Value();
				    v.Set(_elementType, "");
				    Values.Add("", v);
			    }
		    }
		    for (var i = 0; i < Values.Length; i++)
		    {
			    using (new EditorGUILayout.HorizontalScope())
			    {
				    Values[i].OnGUI();
				    if (GUILayout.Button(EditorGUIUtility.TrIconContent("Toolbar Minus"), "RL FooterButton", GUILayout.Width(16)))
				    {
					    Values.Remove(i);
					    EditorGUIUtility.ExitGUI();
				    }
			    }
		    }
		    return this;
	    }

	    public void ResetIndex()
	    {
		    Values.ResetIndex();
	    }

	    public object GetValue(Type type)
	    {

		    if (type.IsArray)
		    {
			    var objects = new object[Values.Length];

			    var array = (Array) Activator.CreateInstance(type, new object[] {Values.Length});
			    for (var i = 0; i < Values.Length; i++)
			    {
					array.SetValue(Values.Get().GetValue(_elementType), i);
			    }
			    return array;
		    }

		    type = typeof(List<>).MakeGenericType(type);
		    IList list = (IList)Activator.CreateInstance(type);
		    for (var i = 0; i < Values.Length; i++)
		    {
			    list.Add(Values.Get().GetValue(_elementType));
		    }

		    return list;
	    }
    }
}
