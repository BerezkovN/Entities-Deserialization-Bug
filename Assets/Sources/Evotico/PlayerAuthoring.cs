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

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            SpriteRenderer spriteRenderer = authoring.GetComponent<SpriteRenderer>();
            
            SpriteAuthoringUtil.AddSpriteComponents(this, authoring, spriteRenderer);
            AddComponent<PlayerTag>(GetEntity(TransformUsageFlags.Dynamic));

            var scene = SceneManager.GetSceneAt(0);
            var gameObjects = scene.GetRootGameObjects();

            var playerGameObject = gameObjects.First(el => el.name == "Player");

            if (playerGameObject == null)
            {
                Debug.LogError("[Evotico] Player object is not found in the main scene");
                return;
            }

            // playerGameObject.GetComponent<PlayerBehaviour>().PlayerEntity = GetEntity(TransformUsageFlags.Dynamic);
        }
    }

    public struct PlayerTag : IComponentData
    {

    }

}
