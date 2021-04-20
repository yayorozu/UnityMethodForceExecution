using System;
using UnityEditor;

namespace Yorozu.MethodExecute.EditorTools
{
	public class EnumValue
	{
		private Type _enumType;
		private string[] _names;
		private Array _values;
		private int _index;

		internal EnumValue(Type type)
		{
			_names = Enum.GetNames(type);
			_values = Enum.GetValues(type);
			_index = 0;
		}

		internal EnumValue OnGUI()
		{
			_index = EditorGUILayout.Popup("", _index, _names);
			return this;
		}

		internal Enum GetValue()
		{
			return (Enum) _values.GetValue(_index);
		}
	}
}
