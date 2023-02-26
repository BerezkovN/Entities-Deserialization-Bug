using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DataHolderAuthoring : MonoBehaviour
{
    public Rect[] Rects;
}

public class DataHolderBaker : Baker<DataHolderAuthoring>
{
    public override void Bake(DataHolderAuthoring authoring)
    {
        AddSharedComponentManaged(new DataSharedManaged
        {
            rects = authoring.Rects
        });
    }
}

[Serializable]
public struct DataSharedManaged : ISharedComponentData, IEquatable<DataSharedManaged>
{
    public Rect[] rects;

    public bool Equals(DataSharedManaged other)
    {
        if (other.rects == null || this.rects == null)
        {
            return false;
        }

        // This is just an example so just compare by first element.
        return rects[0].Equals(other.rects[0]);
    }

    public override int GetHashCode()
    {
        return rects[0].GetHashCode();
    }
}

[Serializable]
public class Rect : IEquatable<Rect>
{
    public float2 position;
    public float2 size;

    public bool Equals(Rect other)
    {
        var res1 = (position == other.position);
        var res2 = (size == other.size);
        return res1.x && res1.y && res2.x && res2.y;
    }

    public override int GetHashCode()
    {
        return (int)(math.hash(position) + math.hash(size));
    }
}
