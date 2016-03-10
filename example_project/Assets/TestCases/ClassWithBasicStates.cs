using MonsterLove.StateMachine;
using UnityEngine;
using System.Collections;

public class ClassWithBasicStates : MonoBehaviour
{
	public ClassWithBasicStatesTestHelper oneStats = new ClassWithBasicStatesTestHelper();
	public ClassWithBasicStatesTestHelper twoStats = new ClassWithBasicStatesTestHelper();
	public ClassWithBasicStatesTestHelper threeStats = new ClassWithBasicStatesTestHelper();

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

	void One_Finally()
	{
		oneStats.finallyCount++;
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

	void Two_Finally()
	{
		twoStats.finallyCount++;
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

	void Three_Finally()
	{
		threeStats.finallyCount++;
	}
}

[System.Serializable]
public class ClassWithBasicStatesTestHelper
{
	public int enterCount;
	public int updateCount;
	public int lateUpdateCount;
	public int exitCount;
	public int finallyCount;
}
