using MonsterLove.StateMachine;
using UnityEngine;
using System.Collections;

public class ClassWithBasicStates : StateBehaviour
{
	public enum States
	{
		One,
		Two,
		Three,
	}

	public ClassWithBasicStatesTestHelper oneStats = new ClassWithBasicStatesTestHelper();
	public ClassWithBasicStatesTestHelper twoStats = new ClassWithBasicStatesTestHelper();
	public ClassWithBasicStatesTestHelper threeStats = new ClassWithBasicStatesTestHelper();

	public void Init()
	{
		Initialize<States>();
	}

	void One_Enter()
	{
		oneStats.enterCount++;
	}

	void One_Update()
	{
		oneStats.updateCount++;
	}

	void One_LateUpdate()
	{
		oneStats.lateUpdateCount++;
	}

	void One_Exit()
	{
		oneStats.exitCount++;
	}

	void Two_Enter()
	{
		twoStats.enterCount++;
	}

	void Two_Update()
	{
		twoStats.updateCount++;
	}

	void Two_LateUpdate()
	{
		twoStats.lateUpdateCount++;
	}

	void Two_Exit()
	{
		twoStats.exitCount++;
	}

	void Three_Enter()
	{
		threeStats.enterCount++;
	}

	void Three_Update()
	{
		threeStats.updateCount++;
	}

	void Three_LateUpdate()
	{
		threeStats.lateUpdateCount++;
	}

	void Three_Exit()
	{
		threeStats.exitCount++;
	}
}

[System.Serializable]
public class ClassWithBasicStatesTestHelper
{
	public int enterCount;
	public int updateCount;
	public int lateUpdateCount;
	public int exitCount;
}
