using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Solipsis/Data/New Material", order = 1)]
public class Material : ScriptableObject
{
    [SerializeField]
    private string m_Id = Guid.NewGuid().ToString().ToUpper();

    [SerializeField]
    private string m_DisplayName;

    [SerializeField]
    private string m_Description;

    [SerializeField]
    private Types m_Type;

    [SerializeField]
    private Sprite m_Icon;

    public string Id { get { return m_Id; } }
    public string Name { get { return m_DisplayName; } set { m_DisplayName = value; } }
    public string Description { get { return m_Description; } }
    public Types Type { get { return m_Type; } }
    public Sprite Icon { get { return m_Icon; } set { m_Icon = value; } }

    public enum Types
    {
        Unknown,
        Wood,
        Sedimentary
    }
}
