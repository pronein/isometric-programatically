using Assets.Entity_Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Assets.Editor
{
    [CustomPropertyDrawer(typeof(TileEntity))]
    public class TileEntityEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            //return base.CreatePropertyGUI(property);

            var root = new VisualElement();

            root.Add(new PropertyField(property.FindPropertyRelative("EntityName")));



            return root;
        }
    }
}
