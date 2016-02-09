using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections;

[TestFixture]
[Category("State Machine Tests")]
internal class TestMethodNameWarnings
{
	private GameObject go;
	private ClassWithHiddenMethods behaviour;

	[SetUp]
	public void Init()
	{
		go = new GameObject("ClassWithHiddenNames");
		behaviour = go.AddComponent<ClassWithHiddenMethods>();
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void TestMisspelledFoundAndNoHiddenMehtodsFound()
	{
		//This is bad unit test in that it will always pass, but I'm can't see anyway to assert for warnings instead of exceptions
		//Have a look in the console to make sure expected warnings are thrown or not thrown. 
		behaviour.TestInit();
	}
}


