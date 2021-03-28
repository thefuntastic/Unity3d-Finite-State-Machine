# Simple Finite State Machine for Unity (C#)

State machines are a very effective way to manage game state, either on your main game play object (Game Over, Restart, Continue etc) or UI (buttonHover, buttonPress etc) or on individual actors and NPCs (AI behaviours, Animations, etc). The following is a simple state machine that should work well within any Unity context. 

## Designed for simplicity

The textbook state machine implementation, and by extension other C# state machine libraries, have a tendency towards complicated configuration or excessive boilerplate. StateMachines are incredibly useful though - administrative overhead should never prevent us from improving readability, fixing bugs, and otherwise writing good code.

* **Create states at the speed of thought:** Just add Enum fields!
* **Effectively reason about your code:** Everything is in one place directly inside your MonoBehaviour.
* **Use what you know about Unity:** Doing things the "Unity" way avoids unexpected weirdness and side effects.
* **Only write the methods you're going to use:** Clever under-the-hood reflection saves you from writing tedious boilerplate.

However, working programmers still need to ship production code. Correctness and performance should not be sacrificed in the name of convenience.  

* Extensive unit test coverage
* Garbage allocation free after initialization
* Battle hardened and shipped in production code 
* Suports iOS/Android/IL2CPP


## Usage

The included example project (for Unity 2019.4) shows the State Machine in action. However, the following is everything you need to get going immediately:

```C#
using MonsterLove.StateMachine; //1. Remember the using statement

public class MyGameplayScript : MonoBehaviour
{
    public enum States
    {
        Init, 
        Play, 
        Win, 
        Lose
    }
    
    StateMachine<States> fsm;
    
    void Awake(){
        fsm = new StateMachine<States>(this); //2. The main bit of "magic". 

        fsm.ChangeState(States.Init); //3. Easily trigger state transitions
    }

    void Init_Enter()
    {
        Debug.Log("Ready");
    }

    void Play_Enter()
    {      
        Debug.Log("Spawning Player");    
    }

    void Play_FixedUpdate()
    {
        Debug.Log("Doing Physics stuff");
    }

    void Play_Update()
    {
        if(player.health <= 0)
        {
            fsm.ChangeState(States.Lose); //3. Easily trigger state transitions
        }
    }

    void Play_Exit()
    {
        Debug.Log("Despawning Player");    
    }

    void Win_Enter()
    {
        Debug.Log("Game Over - you won!");
    }

    void Lose_Enter()
    {
        Debug.Log("Game Over - you lost!");
    }

}
```

### State Methods are defined by underscore convention ( `StateName_Method` )

Like MonoBehavior methods (`Awake`, `Updates`, etc), state methods are defined by convention. Declare a method in the format `StateName_Method`, and this will be associated with any matching names in the provided enum.

```C#
void enum States
{
    Play, 
}


//Coroutines are supported, simply return IEnumerator
IEnumerator Play_Enter()
{
    yield return new WaitForSeconds(1);
    
    Debug.Log("Start");    
}


IEnumerator Play_Exit()
{
    yield return new WaitForSeconds(1);
}

void Play_Finally()
{
    Debug.Log("GameOver");
}
```
These built-in methods are always available, triggered automatically by `ChangeState(States newState)` calls: 
- `Enter`
- `Exit`
- `Finally`

Both `Enter` and `Exit` support co-routines, simply return `IEnumerator`. However, return `void`, and they will be called immediately with no overhead. `Finally` is always called after `Exit` and provides an opportunity to perform clean-up and hygiene in special cases where the `Exit` routine might be interrupted before completing (see the Transitions heading).

## Data-Driven State Events

To define additional events, we need to specify a `Driver`.

```C#
public class Driver
{
    StateEvent Update;
    StateEvent<Collision> OnCollisionEnter; 
    StateEvent<int> OnHealthPickup;
}
```

This is a very simple class. It doesn't have to be called `Driver`; the only constraint is that it must contain `StateEvent` fields. When we pass this to our state machine definition, it will take care of everything needed to set up new State event hooks.

```C#
StateMachine<States, Driver> fsm;
    
void Awake(){
    fsm = new StateMachine<States, Driver>(this); 
}

void Play_Enter()
{
    Debug.Log("Started");
}

void Play_Update()
{
    Debug.Log("Ticked");
}

void Play_OnHealthPickup(int health)
{
    //Add to player health
}

```

As these are custom events, the final step is to tell the state machine when these should be fired.

```C#
void Update()
{
    fsm.Driver.Update.Invoke();
}

void OnCollisionEnter(Collision collision)
{
    fsm.Driver.OnCollisionEnter.Invoke(collision);
}

void OnHealthPickup(int health)
{
    fsm.Driver.OnHealthPickup.Invoke();
}
```

##### Driver Deep-Dive

Compared to the rest of the StateMachine, the `Driver` might elicit a reaction of: *"Hey! You said there wasn't going to be any funny business here!"*

Indeed, there aren't many analogues in either C# or Unity. Before `v4.0`, the state machine would dynamically assign a `StateMachineRunner` component that would call `FixedUpdate`,`Update` & `LateUpate` hooks. (For backwards compatibility this is still the default behaviour when omitting a `Driver`). This worked, but additional hooks meant forking the `StateMachineRunner` class. Also, as a separate MonoBehaviour, it has it's own script execution order which could sometimes lead to oddities.

But with the user responsible for invoking events - eg `fsm.Drive.Update.Invoke()`, it becomes much easier to reason about the lifecycle of the fsm. No more having to guess whether the StateMachine will update before or after the rest of the class, because the trigger is right there. It can be moved to right spot in the main `Update()` call. 

```C#
void Update()
{
    //Do Stuff

    fsm.Driver.Update.Invoke();

    //Do Other Stuff
}

void Play_Update()
{
    //No guessing when this happens
}
```

The real power shines when we consider another anti-pattern. Calling a state change from outside the state machine can lead to unintended side-effects. Imagine the following scenario where a global call causes a state transition. However without 

```C#
public void EndGame()
{
    fsm.ChangeState(States.GameOver);
}

void Idle_Update()
{
    //Changing to GameOver would cause unintended things to happen
}

void Play_Update()
{
    //GameOver is legal
}
```

Some libraries deal with this by defining transitons tables. However, it's possible to achieve a similar outcome using state events:  
```C#
public class Driver()
{
    public StateEvent OnEndGame;
}

public void EndGame()
{
    fsm.Driver.OnEndGame.Invoke();
}

void Idle_Update()
{
    //Changing to GameOver would cause unintended things to happen
}

void Play_Update()
{
    //GameOver is legal
}

void Play_OnEndGame()
{
    fsm.ChangeState(State.GameOver);
}
```
Now the `Play` state is only state that can respond to EndGame calls. This creates an implicit transition table as sort of "free" side-effect.


## Async Transitions

There is simple support for managing asynchronous state changes with long enter or exit coroutines.

```C#
fsm.ChangeState(States.MyNextState, StateTransition.Safe);
```

The default is `StateTransition.Safe`. This will always allows the current state to finish both its enter and exit functions before transitioning to any new states.

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

## Upgrading from v3 and above - April 2020

Version `4.0` brings substantial innovation, however the API strives for backwards compatibility which means all the code you've already written does not need to change. However, the layout of the files inside the package has changed. To avoid errors it is recommended you delete the existing `MonsterLove` folder containing `StateMachine.cs` and related files, then reimport the new package.  

## Performance & Limitations

##### Design Philosophy

The state machine is designed to maximise simplicity for the end-user. To achieve this, under the hood lies some intricate reflection "magic". Reflection is a controversial choice because it is slow - and that's no exception here. However, we seek to balance the trade-off by limiting all the reflection to a single call when the state machine is initialised. This does degrade instantiation performance, however, instantiation is already slow. It's expected that strategies such as object pooling (recycling objects spawned on startup instead of at runtime) are already in effect, which moves this cost to a time when the user is unlikely to notice it.

Once the initialisation cost has been swallowed, the State Machine aims to be a good citizen at runtime, avoiding allocations that cause garbage pressure and aiming for respectable performance. Ensuring correctness does mean that calling StateEvents' `Invoke()` is slower than naked method calls. Over tens of thousands of instances this can add up to a significant overhead. In these use cases (multiple 1000's of objects) it is recommended to replace the state machine with something hand-tuned. 

However, for most general use cases, eg manager classes, or other items with low instance counts (10's or 100's) - the difference in performance should absolutely not be something you need to think about. 

##### Memory Allocation Free?
This is designed to target mobile, as such should be memory allocation free. However the same rules apply as with the rest of Unity in regards to using `IEnumerator` and Coroutines.  

##### Windows Store Platforms
Due to differences in the Windows Store flavour of .Net and WinRT, this platform is currently incompatible. More details available in this [issue](https://github.com/thefuntastic/Unity3d-Finite-State-Machine/issues/4).

## License
MIT License

## Notes

This is state machine is used extensively on the [Made With Monster Love](http://www.madewithmonsterlove.com) title [Cadence](http://store.steampowered.com/app/362800/Cadence/), a puzzle game about making music.  

This library owes its origins to the state machine found at http://unitygems.com/ (available via [The Internet Archive](http://web.archive.org/web/20140902150909/http://unitygems.com/fsm1/) as of Nov 2014). The original project, however, had many short comings that made usage difficult. Adhering to the principle that a library should do one thing really well, what we have now is perhaps the easiest and most straight forward State Machine in existence for Unity.

##### Feedback and suggestions:
- http://www.twitter.com/thefuntastic
