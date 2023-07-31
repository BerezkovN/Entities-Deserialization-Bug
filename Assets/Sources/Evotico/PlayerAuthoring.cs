using System.Diagnostics;
using Enlighten.dotSprites;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace Enlighten.Evotico
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAuthoring : SpriteAuthoring
    {

    }
    
    public struct PlayerTag : IComponentData
    {

    }

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            SpriteRenderer spriteRenderer = authoring.GetComponent<SpriteRenderer>();
            Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            SpriteAuthoringUtil.AddSpriteComponents(this, authoring, spriteRenderer);
            AddComponent<PlayerTag>(playerEntity);
            AddComponent<CreatureMovementComponent>(playerEntity);
        }
    }

}
