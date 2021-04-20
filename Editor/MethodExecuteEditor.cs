using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Yorozu.MethodExecute.EditorTools
{
    /// <summary>
    /// 指定したメソッドを無理やり実行してUI等を表示する
    /// アセットバンドル等static なインスタンスを利用していた場合はできない
    /// </summary>
    public class MethodExecuteEditor : EditorWindow
    {
        [MenuItem("Tools/ExecMethod")]
        private static void Show()
        {
            var window = GetWindow<MethodExecuteEditor>();
            window.titleContent = new GUIContent("ExecMethod");
        }

        internal const BindingFlags FLAGS_Declared = BindingFlags.Instance |
                                                     BindingFlags.Public |
                                                     BindingFlags.NonPublic |
                                                     // 継承禁止
                                                     BindingFlags.DeclaredOnly |
                                                     BindingFlags.Static;

        [SerializeField]
        private Component _targetComponent;
        [SerializeField]
        private string[] _methodNames;
        /// <summary>
        /// 引数
        /// </summary>
        private Args _args;
        /// <summary>
        /// 対象のメソッド名
        /// </summary>
        [SerializeField]
        private int _methodIndex;

        private void OnEnable()
        {
            SetMethodList();
        }

        private void OnDisable()
        {
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                _targetComponent = (Component) EditorGUILayout.ObjectField("Target", _targetComponent, typeof(Component), true);
                if (check.changed)
                {
                    SetMethodList();
                }
            }

            if (_targetComponent == null || _methodNames == null)
                return;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                _methodIndex = EditorGUILayout.Popup("Method", _methodIndex, _methodNames);
                if (check.changed)
                {
                    SetArgs();
                }
            }

            if (_args != null)
            {
                _args.OnGUI();
            }

            if (GUILayout.Button("実行"))
            {
                Execute();
            }
        }

        private void SetMethodList()
        {
            _methodNames = new string[0];
            if (_targetComponent == null)
                return;

            var methods = _targetComponent
                .GetType()
                .GetMethods(FLAGS_Declared);

            foreach (var method in methods)
            {
                var name = method.Name;
                var args = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name).ToArray());
                if (!string.IsNullOrEmpty(args))
                {
                    name += "(" + args + ")";
                }

                ArrayUtility.Add(ref _methodNames, name);
            }

            if (_methodIndex >= _methodNames.Length)
            {
                _methodIndex = 0;
            }

            SetArgs();
        }

        private void SetArgs()
        {
            if (_targetComponent == null)
                return;

            var findMethod = _targetComponent.GetType()
                .GetMethods(FLAGS_Declared)
                .GetValue(_methodIndex) as MethodInfo;

            if (findMethod == null)
            {
                Debug.LogError("Method Not find");
            }
            _args = new Args(findMethod);
        }

        /// <summary>
        /// 実行
        /// </summary>
        private void Execute()
        {
            if (_targetComponent == null)
                return;

            var findMethod = _targetComponent.GetType()
                .GetMethods(FLAGS_Declared)
                .GetValue(_methodIndex) as MethodInfo;

            // 実行
            findMethod.Invoke(_targetComponent, _args.GetArgs(findMethod));
        }
    }
}
