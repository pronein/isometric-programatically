using System;
using UnityEngine;

[Serializable]
public class EntityMaterial
{
    [SerializeField]
    private Material m_Material;
    [SerializeField]
    private float m_MaterialAmount;

    public Material Material { get { return m_Material; } set { m_Material = value; } }
    public float Amount { get { return m_MaterialAmount; } set { m_MaterialAmount = value; } }
}
