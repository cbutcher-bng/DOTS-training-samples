﻿using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct FoodLocation : IComponentData
{
    public float2 value;
}