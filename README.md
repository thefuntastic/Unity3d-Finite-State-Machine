# Unity3D - Simple Finite State Machine (C#)

State machines are a very effective way to manage game state, either on your main game play object (Game Over, Restart, Continue etc) or on individual actors and NPCs (AI behaviours, Animations, etc). The following is a simple state machine that should work well within any unity context. 

## Designed with simplicity in mind

Most state machines come from the world of C# enterprise, and are wonderfully complicated or require a lot of boilerplate code. State Machines however are an incredibly useful pattern in game development, administrative overhead should never be a burden that discourages you from writing good code. 

* Simple use of Enums as state definition. 
* Minimal initialization - one line of code. 
* Incredibly easy to add/remove states
* Uses reflection to avoid boiler plate code - only write the methods you actually need. 
* Compatible with Coroutines.
* Tested on iOS and Android

## Usage

An example project included (Unity 4.6) to show the State Machine in action.

To use the state machine you need a few simple steps

##### Your monobehivour should inherit from StateMachineBeviour

```C#
using MonsterLove.StateMachine; //Remember the using statement before the class declaration

public class MyStateMachine : StateMachineBehaviour
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
##### Initialize the State Machine 

```C#
Initialize<States>(this);

```
This can be done at any time, but generally you would do it on start up in your Awake function. 

##### You are now ready to manage state by simply calling `stateMachine.ChangeState()`
```C#
ChangeStates(States.Init);
```

##### State callbacks defined by underscore convention ( `StateName_Method` )

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

It should be easy enough to extend the source to include other Unity Methods such as OnTriggerEnter, OnMouseDown etc

These methods can be private or public. The methods themselves are all optional, so you only need to provide the ones you actually intend on using. 

Couroutines are supported on Enter and Exit, simply return `IEnumerator`. This can be great way to accommodate animations.

##### Dependencies

There are no dependencies, but if you're working with the source files, the tests rely on the UnityTestTools package which can be downloaded from the Asset Store. These are non-essential, only work in the editor, and can be deleted if you so choose. 

## Implementation and Shortcomings

This implementation uses reflection to automatically bind the state methods callbacks for each state. This saves you having to write endless boilerplate and generally makes life a lot more pleasant. But of course reflection is slow, so we try minimize this by only doing it once during the call to `Initialize`. 

For most objects this won't be a problem, but note that if you are spawning many objects during game play it might pay to make use of an object pool, and initialize objects on start up instead. (This is generally good practice anyway). 

##### Memory Allocation Free?
This is designed to target mobile, as such should be memory allocation free. Right now I think we are failing on that front, primarily due to IEnumerator's generating garbage. This remains a work in progress. 

##### Callable States, Cancelling Transitions
Right now the StateMachine is quite naive. There is no way to set a sub state (e.g. setting a child state without exiting the parent state). In my experience this mostly adds unnecessary convolution to the library. 

Also, there is currently no way to cancel a coroutine once a state transition has started. Indeed, this is a shortcoming, because actors often need to go to a different state before the transition is complete. It's uncertain when, if ever, there will be support for this.  

##### Test Coverage
There is some minimal test coverage, and is likely to remain this way until I get a better grasp of the UnityTestTools.

## Update - November 2014 - v2.0

Over the past couple of years I've been getting a steady stream of traffic to this project, in spite of my neglect and recommendation that people should instead use the State Machine found on the Unity Gems blog.

The truth is that most of the time I ended up using a very rudimentary state machine above the Unity Gems effort. This was primarily because I kept on running into hitches with iOS. But it was also an intimidating project to use, sifting through an elaborate tutorial to find all the moving parts (which as of writing appears to be offline anyway).

Finally I found the time to take my favourite parts of the Unity Gems effort and my ghetto state machine to create something awesome. Adhering to the principle that a library should do one thing really well, what we have now I consider to be the easiest and most straight forward State Machine in existence for Unity.

**NB** The previous versions of the state machine featured here are not compatible with the current implementation, as it's been rewritten from the ground up to prefer a single class based approach over multiple classes per state. The 2.0 version now resides in the `MonsterLove` namespace, so there should be no conflicts if you want to upgrade and use both in your project.

##License
MIT License

## Notes

This is state machine is used extensively on the [Made With Monster Love](http://www.madewithmonsterlove.com) game [Cadence](http://www.playcadence.com), a puzzle game about making music.  

It is heavily inspired by the state machine found at http://unitygems.com/ (available via http://archive.org/web/ as of Nov 2014)

- [State Machines Part 1](http://unitygems.com/fsm1/)
- [State Machines Part 2](http://unitygems.com/fsm2/)
- [State Machines Part 3](http://unitygems.com/finite-state-machines-3-final-state-machine-framework/)

For feedback and suggestions:

http://www.thefuntastic.com/2012/04/simple-finite-state-machine/

http://www.twitter.com/thefuntastic
