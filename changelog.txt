v4.0 April 2021
	*StateMachine now accepts a Driver class, a powerful convention for data driven state events that provides explicit control of the State Machine lifecycle. 
	*Added `LastState` property
	*Added `NextState` property
	*Added `reenter` flag to allow transitions back into the same state
	*Fixed AOT imcompatibility preventing release of v4.0
	*Added new example showing Driver implementation
	*Modernised unit tests for Unity Test Runner
	*Components deriving from a super class will now also search super classes for state methods
	*Upgraded project to Unity 2019.4.19

	### Upgrade Notes:
	The layout of the library has changed. To avoid issues delete the existing `MonsterLove` folder containing `StateMachine.cs` and related files, before reimporting the Unity package. The API however remains backwards compatible with prior versions. This means your client code does not need to be upgraded.

v4.0-rc1 August 2019
	*Created StateMachine Driver concept
	*Added `LastState` property to StateMachine class
	*Modernised unit tests for Unity Test Runner
	*Compenents deriving from a super class will now also search super classes for state methods
	*Upgraded project to Unity 2018.3.7

v3.1 March 2016
	*Fixing edge cases where non coroutine Exit fucntions wouldn't be called if StateTrasition.Overwrite used

v3.0 March 2016
	*Multiple state machines per game object now supported
	*Can now reference stateMachine.State without having to cast the result to an enum
	*Enter and Exit methods that aren't corouintes are now executed on the same frame they are called - solves many timing issues
	*StateMachine is assigned to an instance variable rather than inheritance. Means your MonoBehaviour can inherit from other classes and allows multiple state machines per object
	*Classes are now StateMachine and StateMachineRunner. Is more semantic and provides a gentle upgrade path that won't clash with the previous implementation
	*StateManchine<States>.Initialize(component) syntax sugar to make initialization more friendly
	*Enhanced test coverage
	*Removed Method Name warnings as this was just an un-ending source of grief 
	*Upgraded example project to Unity 5.3.2

v2.4 June 2015
	*Bugfix: StateTransition.Overwrite is now consistent with expected behviour
	*Added "Finally" state function that is always called, even if overwrite cancels the exit routine

v2.3 April 2015
	*Breaking change: Refactored StateMachineBehaviour and StateMachineEngine to StateBehaviour and StateEngine to avoid Unity5 conflicts
	*Added the concept of StateTransition. Two options: Safe and Overwrite. The default is safe which allows the current coroutine to finish before changing to the next state. 
	*Better test coverage using UnityTestTools integration tests

v2.2 January 2015
	*State Update & Late Update functions no longer run during transitions (i.e. Enter & Exit functions)
	*Bug fix: Changing state from a states Update function no longer cause states to enter twice. Note: changing from functions outside of a state can still cause this behaviour

v2.1 December 2014
	*Change initialization method and added convenience methods on StateMachineBehaviour. NotePrevious projects will need change Initialization calls.

v2.0 November 2014
	Complete rewrite of State Machine for Unity. Now has the following features

	* Simple use of Enums as state definition. 
	* Minimal initialization - one line of code. 
	* Incredibly easy to add/remove states
	* Uses reflection to avoid boiler plate code - only write the methods you actually need. 
	* Compatible with Coroutines.
	* Tested on iOS and Android

v1.0 April 2012
	A state machine for Unity