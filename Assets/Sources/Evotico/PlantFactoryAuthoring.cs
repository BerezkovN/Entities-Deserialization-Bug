using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Enlighten.dotSprites
{
    public class PlantFactoryAuthoring : MonoBehaviour
    {
        public SpriteMaterial SpriteMaterial;
        public int Count;
        public bool RemoveChildren;
        public float2 Size;

        public List<PlantLayer> Layers;

        [ContextMenu("Generate plants")]
        public void GeneratePlants()
        {

            foreach (Transform child in transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }

            Random random = new Random(69);

            for (int ind = 0; ind < Count; ind++)
            {
                var pos = random.NextFloat2();
                float x = random.NextFloat(-Size.x, Size.x);
                float y = random.NextFloat(-Size.y, Size.y);

                var plant = new GameObject("Plant_" + ind);
                plant.transform.position = new Vector3(x, y, 0);
                plant.transform.SetParent(transform, false);

                float currentHeight = 0;
                foreach (var layer in Layers)
                {
                    currentHeight -= layer.heightDifference;

                    var layerObject = new GameObject("Layer_" + layer.sprite.name);
                    layerObject.transform.position = new Vector3(0, 0, currentHeight + random.NextFloat(-0.1f, .1Df));
                    
                    var spriteRenderer = layerObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = layer.sprite;
                    
                    SpriteAuthoring layerSprite = layerObject.AddComponent<SpriteAuthoring>();
                    //layerSprite.Sprite = layer.sprite;
                    layerSprite.SpriteMaterial = SpriteMaterial;

                    layerObject.transform.localScale = new Vector3(layer.size, layer.size, layer.size);
                    layerObject.transform.SetParent(plant.transform, false);
                }
            }
        }
    }

    [Serializable]
    public struct PlantLayer
    {
        public Sprite sprite;
        public float size;
        public float heightDifference;
    }
}
