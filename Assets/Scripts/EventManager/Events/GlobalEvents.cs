
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using udoEventSystem;

public class TestEventWithoutParameter : AEvent
{
}

public class TestEventWithParameter : AEvent<bool>
{
}

public class LevelStarted : AEvent
{
}

public class LevelCompleted: AEvent
{
}

public class LevelFailed : AEvent
{
}
public class GatePriceCalculated : AEvent
{
}

public class AllGangMembersDied : AEvent
{
}

public class PriceColorChanged : AEvent
{
}