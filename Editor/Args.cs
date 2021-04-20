using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Yorozu.MethodExecute.EditorTools
{
	public class Args
	{
		/// <summary>
		/// フィールド一つひとつの値
		/// </summary>
		private ArgValue _args;

		/// <summary>
		/// MethodInfo からパラメータ生成
		/// </summary>
		internal Args(MethodInfo info)
		{
			var parameters = info.GetParameters();
			_args = new ArgValue();
			foreach (var t in parameters)
			{
				var name = t.Name;
				var type = t.ParameterType;
				var v = new Value();
				v.Set(type, name);
				_args.Add(name, v);
			}
		}

		/// <summary>
		/// 引数を取得
		/// </summary>
		internal object[] GetArgs(MethodInfo info)
		{
			var parameters = info.GetParameters();
			var returns = new object[parameters.Length];

			_args.ResetIndex();
			for (var i = 0; i < parameters.Length; i++)
			{
				var arg = _args.Get();
				arg.ResetIndex();
				returns[i] = arg.GetValue(parameters[i].ParameterType);
			}

			return returns;
		}

		internal void OnGUI()
		{
			EditorGUILayout.LabelField("Set Params");
			_args.OnGUI();
		}
	}
}
