using strange.extensions.signal.impl;
using UnityEngine;

namespace onur.pool.signals
{
    public class StartAppSignal : Signal {}
    public class MoveSignal : Signal <float, float> { }
    public class ShotSignal : Signal<float> { }
    public class ShotPower : Signal<float> { }
    public class GameModeSignal : Signal<bool> { }
}