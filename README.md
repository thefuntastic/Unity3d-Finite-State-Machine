# Unity3D - Simple Finite State Machine (C#)

State machines are a very effective way to manage game state, either on your main game play object (Game Over, Restart, Continue etc) or on individual actors and NPCs (AI behaviours, Animations, etc). The following is a simple state machine that should work well within any Unity context. 

## Designed with simplicity in mind

Most state machines come from the world of C# enterprise, and are wonderfully complicated or require a lot of boilerplate code. State Machines however are an incredibly useful pattern in game development, administrative overhead should never be a burden that discourages you from writing good code. 

* Simple use of Enums as state definition. 
* Minimal initialization - one line of code. 
* Incredibly easy to add/remove states
* Uses reflection to avoid boiler plate code - only write the methods you actually need. 
* Compatible with Coroutines.
* Tested on iOS and Android

## Usage

An example project is included (Unity 5.0) to show the State Machine in action.

To use the state machine you need a few simple steps

##### Include the StateMachine package

```C#
using MonsterLove.StateMachine; //Remember the using statement before the class declaration

public class MyManagedComponent : MonoBehaviour
{

}
```

##### Define your states using an Enum 

```C#
public enum States
{
	Init, 
    Play, 
    Win, 
    Lose
}
```
##### Create a variable to store a reference to the State Machine 

```C#
StateMachine<States> fsm;
```

##### Get a valid state machine for your MonoBehaviour

```C#
fsm = StateMachine<States>.Initialize(this);
```

This is where all of the magic in the StateMachine happens: in the background it inspects your MonoBehaviour (`this`) and looks for any methods described by the convention shown below.

You can call this at any time, but generally `Awake()` is a safe choice. 

##### You are now ready to manage state by simply calling `ChangeState()`
```C#
fsm.ChangeState(States.Init);
```

##### State callbacks are defined by underscore convention ( `StateName_Method` )

```C#
void Init_Enter()
{
	Debug.Log("We are now ready");
}

//Coroutines are supported, simply return IEnumerator
IEnumerator Play_Enter()
{
	Debug.Log("Game Starting in 3");
	yield return new WaitForSeconds(1);
    
    Debug.Log("Game Starting in 2");
	yield return new WaitForSeconds(1);
    
    Debug.Log("Game Starting in 1");
	yield return new WaitForSeconds(1);
    
    Debug.Log("Start");	
}

void Play_Update()
{
	Debug.Log("Game Playing");
}

void Play_Exit()
{
	Debug.Log("Game Over");
}
```
Currently supported methods are:

- `Enter`
- `Exit`
- `FixedUpdate`
- `Update`
- `LateUpdate`
- `Finally`

It should be easy enough to extend the source to include other Unity Methods such as OnTriggerEnter, OnMouseDown etc

These methods can be private or public. The methods themselves are all optional, so you only need to provide the ones you actually intend on using. 

Couroutines are supported on Enter and Exit, simply return `IEnumerator`. This can be great way to accommodate animations. Note: `FixedUpdate`, `Update` and `LateUpdate` calls won't execute while an Enter or Exit routine is running.

Finally is a special method guaranteed to be called after a state has exited. This is a good place to perform any hygiene operations such as removing event listeners. Note: Finally does not support coroutines.

##### Transitions

There is simple support for managing asynchronous state changes with long enter or exit coroutines.

```C#
fsm.ChangeState(States.MyNextState, StateTransition.Safe);
```

The default is `StateTransition.Safe`. This will always allows the current state to finish both it's enter and exit functions before transitioning to any new states.

```C#
fsm.ChangeState(States.MyNextState, StateTransition.Overwrite);
```

`StateMahcine.Overwrite` will cancel any current transitions, and call the next state immediately. This means any code which has yet to run in enter and exit routines will be skipped. If you need to ensure you end with a particular configuration, the finally function will always be called:

```C#
void MyCurrentState_Finally()
{
    //Reset object to desired configuration
}
```

##### Dependencies

There are no dependencies, but if you're working with the source files, the tests rely on the UnityTestTools package. These are non-essential, only work in the editor, and can be deleted if you so choose. 

## Upgrade Notes - March 2016 - v3.0

Version 3 brings with it a substantial redesign of the library to overcome limitations plaguing the previous iteration (now supports multiple states machines per component, instant Enter & Exit calls, more robust initialization, etc). As such there is a now a more semantic class organisation with `StateMachine` & `StateMachineRunner`. 

It is recommend you delete the previous package before upgrading, **but this will break your code!** 

To do a complete upgrade you will need to rewrite initialization as per above. You will also need to replace missing `StateEngine` component references with `StateMachineRunner` in the Unity editor. If you want a workaround in order to do a gradual upgrade without breaking changes, you can change the namespace of the `StateMachine` and `StateMachineRunner` and you will be able to use it alongside v2 code until you feel confident enough to do a full upgrade.  

## Implementation and Shortcomings

This implementation uses reflection to automatically bind the state methods callbacks for each state. This saves you having to write endless boilerplate and generally makes life a lot more pleasant. But of course reflection is slow, so we try minimize this by only doing it once during the call to `Initialize`. 

For most objects this won't be a problem, but note that if you are spawning many objects during game play it might pay to make use of an object pool, and initialize objects on start up instead. (This is generally good practice anyway). 

##### Manual Initialization
In performance critical situations (e.g. thousands of instances) you can optimize initialization further but manually configuring the StateMachineRunner component. You will need to manually add this to a GameObject and then call:
```C#
StateMachines<States> fsm = GetComponent<StateMachineRunner>().Initialize<States>(componentReference);
```

##### Memory Allocation Free?
This is designed to target mobile, as such should be memory allocation free. However the same rules apply as with the rest of unity in regards to using `IEnumerator` and Coroutines.  

##### Windows Store Platforms
Due to differences in the Windows Store flavour of .Net, this is currently incompatible. More details available in this [issue](https://github.com/thefuntastic/Unity3d-Finite-State-Machine/issues/4).

## License
MIT License

## Notes

This is state machine is used extensively on the [Made With Monster Love](http://www.madewithmonsterlove.com) title [Cadence](http://www.playcadence.com), a puzzle game about making music.  

This library owes its origins to the state machine found at http://unitygems.com/ (available via [The Internet Archive](http://web.archive.org/web/20140902150909/http://unitygems.com/fsm1/) as of Nov 2014). The original project, however, had many short comings that made usage difficult. Adhering to the principle that a library should do one thing really well, what we have now is perhaps the easiest and most straight forward State Machine in existence for Unity.

##### Feedback and suggestions:
- http://www.thefuntastic.com/2012/04/simple-finite-state-machine/
- http://www.twitter.com/thefuntastic
