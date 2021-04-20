using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Yorozu.MethodExecute.EditorTools
{
	/// <summary>
	/// Array もしくは List のキャッシュ用
	/// </summary>
	internal class ArrayList : IResetIndex
	{
		private Type _elementType;
		private string _label;
		/// <summary>
		///     Arrayの中身
		/// </summary>
		private ArgValue Values = new ArgValue();

		internal ArrayList(string label, Type type)
		{
			_elementType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
			_label = label + " (" + _elementType.Name + ")";
		}

		public void ResetIndex()
		{
			Values.ResetIndex();
		}

		internal ArrayList OnGUI()
		{
			// Header
			using (new GUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(string.Format("{0} [{1}]", _label, Values.Length));
				GUILayout.FlexibleSpace();

				// 要素の追加
				if (GUILayout.Button(EditorGUIUtility.TrIconContent("Toolbar Plus"), "RL FooterButton",
					GUILayout.Width(16)))
				{
					var v = new Value();
					v.Set(_elementType, "");
					Values.Add("", v);
				}
			}

			using (new EditorGUI.IndentLevelScope())
			{
				for (var i = 0; i < Values.Length; i++)
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField("Element " + i);
						if (GUILayout.Button(EditorGUIUtility.TrIconContent("Toolbar Minus"), "RL FooterButton", GUILayout.Width(16)))
						{
							Values.Remove(i);
							GUIUtility.ExitGUI();
						}
					}
					Values[i].OnGUI();
				}
			}

			return this;
		}

		internal object GetValue(Type type)
		{
			if (type.IsArray)
			{
				var array = (Array) Activator.CreateInstance(type, Values.Length);
				for (var i = 0; i < Values.Length; i++)
					array.SetValue(Values.Get().GetValue(_elementType), i);

				return array;
			}

			var genericType = typeof(List<>).MakeGenericType(_elementType);
			var list = (IList) Activator.CreateInstance(genericType);
			for (var i = 0; i < Values.Length; i++)
				list.Add(Values.Get().GetValue(_elementType));

			return list;
		}
	}
}
