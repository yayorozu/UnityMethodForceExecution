using System.Collections.Generic;
using UnityEngine;

public class SampleMethod : MonoBehaviour
{
	private enum Types
	{
		A,
		B,
		C
	}
	private void SampleInt(int value)
	{
		Debug.Log(value);
	}

	private void SampleEnum(Types value)
	{
		Debug.Log(value);
	}

	private void SampleVector3(Vector3 value)
	{
		Debug.Log(value);
	}

	private void SampleNullable(int? value)
	{
		Debug.Log(value.HasValue);
		if (value.HasValue)
			Debug.Log(value.Value);
	}

	private void SampleClass(TestExec value)
	{
		Debug.Log(value.IntValue);
	}

	private void SampleClassInnerList(TestInnerList value)
	{
		Debug.Log(value.IntValue);
		foreach (var i in value.IntList)
		{
			Debug.Log(i);
		}
	}

	private void SampleArray(int[] values)
	{
		Debug.Log("Array Size: " + values.Length);
		for (var index = 0; index < values.Length; index++)
		{
			Debug.Log("Element " + index);
			var value = values[index];
			Debug.Log(value);
		}
	}
	private void SampleList(List<int> values)
	{
		Debug.Log("Array Size: " + values.Count);
		for (var index = 0; index < values.Count; index++)
		{
			Debug.Log("Element " + index);
			var value = values[index];
			Debug.Log(value);
		}
	}

	private void SampleArray(TestArray[] values)
	{
		Debug.Log("Array Size: " + values.Length);
		for (var index = 0; index < values.Length; index++)
		{
			Debug.Log("Element " + index);
			var value = values[index];
			Debug.Log(value.IntValue);
			Debug.Log(value.StringValue);
		}
	}

	private void SampleClass2(TestExec value, TestExec value2)
	{
		Debug.Log(value.IntValue);
		Debug.Log(value.inner.IntValue);
		Debug.Log(value2.IntValue);
		Debug.Log(value2.inner.IntValue);
	}

	private void SampleIntString(int value, string v2)
	{
		Debug.Log(value);
		Debug.Log(v2);
	}

	private void SampleIntInt(int value, int v2)
	{
		Debug.Log(value);
		Debug.Log(v2);
	}
}

public class TestArray
{
	public int IntValue;
	public string StringValue;
}

public class TestExec
{
	public int IntValue;
	public Inner inner;

	public class Inner
	{
		public int IntValue;
	}
}

public class TestInnerList
{
	public int IntValue;
	public List<int> IntList;
}
