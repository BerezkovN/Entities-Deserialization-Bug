using System.Collections;
using System.Collections.Generic;
using Enlighten.Evotico;
using Sources.Evotico;
using Unity.Entities;
using UnityEngine;

public class EvoticoInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        
        var world = World.DefaultGameObjectInjectionWorld;
        
        var creatureMovementSystem = world.CreateSystem<CreatureMovementSystem>();
        world.GetExistingSystemManaged<SimulationSystemGroup>().AddSystemToUpdateList(creatureMovementSystem);
        
        var playerMovementSystem = world.CreateSystem<PlayerMovementSystem>();
        world.GetExistingSystemManaged<InitializationSystemGroup>().AddSystemToUpdateList(playerMovementSystem);

    }
}
