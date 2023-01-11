using Assets.Scripts.Core.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(EntityMatterState))]
public class EntityMatterStateDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var asset = Resources.Load<VisualTreeAsset>("entity_matter_state_drawer");
        var drawer = asset.Instantiate(property.propertyPath);

        drawer.Q<Label>().text = property.displayName;

        return drawer;
    }
}
