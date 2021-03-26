using MonsterLove.StateMachine;
using UnityEngine;
using System.Collections;

public class ClassWithBasicStates : MonoBehaviour
{
	public ClassWithBasicStatesTestHelper oneStats = new ClassWithBasicStatesTestHelper();
	public ClassWithBasicStatesTestHelper twoStats = new ClassWithBasicStatesTestHelper();
	public ClassWithBasicStatesTestHelper threeStats = new ClassWithBasicStatesTestHelper();

	protected void One_Enter()
	{
		oneStats.enterCount++;
	}

	protected void One_Update()
	{
		oneStats.updateCount++;
	}

	protected void One_LateUpdate()
	{
		oneStats.lateUpdateCount++;
	}

	protected void One_Exit()
	{
		oneStats.exitCount++;
	}

	protected void One_Finally()
	{
		oneStats.finallyCount++;
	}

	protected void Two_Enter()
	{
		twoStats.enterCount++;
	}

	protected void Two_Update()
	{
		twoStats.updateCount++;
	}

	protected void Two_LateUpdate()
	{
		twoStats.lateUpdateCount++;
	}

	protected void Two_Exit()
	{
		twoStats.exitCount++;
	}

	protected void Two_Finally()
	{
		twoStats.finallyCount++;
	}

	protected void Three_Enter()
	{
		threeStats.enterCount++;
	}

	protected void Three_Update()
	{
		threeStats.updateCount++;
	}

	protected void Three_LateUpdate()
	{
		threeStats.lateUpdateCount++;
	}

	protected void Three_Exit()
	{
		threeStats.exitCount++;
	}

	protected void Three_Finally()
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
