using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using Unity.Mathematics;
using static UnityEngine.Tilemaps.Tile;

//[CustomEditor(typeof(GridSelection))]
public class GridSelectionEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        BoundsInt selection = GridSelection.position;
        Tilemap tilemap = GridSelection.target.GetComponent<Tilemap>();

        var root = new VisualElement();

        var foldout = new Foldout()
        {
            viewDataKey = "GridSelectionEditorTilesFoldout",
            text = "Tiles"
        };

        foreach (var p in selection.allPositionsWithin)
        {
            var tile = tilemap.GetTile(p) as Tile;

            if (tile == null) continue;

            var tileFoldout = new Foldout
            {
                viewDataKey = $"GridSelectionTileFoldout_{p.x}_{p.y}_{p.z}",
                text = $"Tile ({p.x},{p.y},{p.z})",
                value = false
            };

            Debug.Log($"Position - {p}");
            Debug.Log($"Tilemap - {tilemap}");
            Debug.Log($"Tile - {tile}");

            tileFoldout.Add(new ObjectField
            {
                label = "Tile",
                objectType = tile.GetType(),
                value = tile
            });

            tileFoldout.Add(new ObjectField
            {
                label = "Sprite",
                objectType = typeof(Sprite),
                value = tilemap.GetSprite(p)
            });

            tileFoldout.Add(new ColorField
            {
                label = "Color",
                value = tilemap.GetColor(p)
            });

            var colliderTypeField = new EnumField
            {
                label = "Collider Type",
                value = tilemap.GetColliderType(p)
            };
            colliderTypeField.Init(ColliderType.None);
            tileFoldout.Add(colliderTypeField);

            var positionField = new Vector3Field
            {
                label = "Position",
                value = p
            };
            Debug.Log($"Tile Flags: {tile.flags} -> {TileFlags.InstantiateGameObjectRuntimeOnly}");
            Debug.Log($"Tile GO: | {tile.gameObject?.GetType()} | -> | {tile.GameObject()?.GetType()} |");
            //Debug.Log($"Tile Transform: {tile.gameObject?.transform}");
            /*
            var soPosition = new SerializedObject(p);
            positionField.TrackPropertyValue(
                ,
                (prop) => {
            });
            */
            tileFoldout.Add(positionField);

            var matrix = tilemap.GetTransformMatrix(p);
            var translation = new Vector3(matrix.m03, matrix.m13, matrix.m23);

            var mRotScale = new float3x3(
                matrix.m00, matrix.m01, matrix.m02,
                matrix.m10, matrix.m11, matrix.m12,
                matrix.m20, matrix.m21, matrix.m22
                );
            var lenC0 = math.length(mRotScale.c0);
            var lenC1 = math.length(mRotScale.c1);
            var lenC2 = math.length(mRotScale.c2);

            var scale = new Vector3(lenC0, lenC1, lenC2);

            var r3x3 = new float3x3(mRotScale.c0 / lenC0, mRotScale.c1 / lenC1, mRotScale.c2 / lenC2);

            if (math.dot(math.cross(r3x3.c0, r3x3.c1), r3x3.c2) < 0f)
            {
                r3x3 *= -1f;
                scale *= -1f;
            }
            r3x3.c0 = math.normalize(r3x3.c0);
            r3x3.c1 = math.normalize(r3x3.c1);
            r3x3.c2 = math.normalize(r3x3.c2);

            var qRot = new quaternion(r3x3).value;
            var rotation = new Vector3(qRot.x, qRot.y, qRot.z);

            tileFoldout.Add(new Toggle("Lock Transform") {
                value = (tile.flags & TileFlags.LockTransform) == TileFlags.LockTransform
            });
            var transformFoldout = new Foldout
            {
                viewDataKey = "TransformFoldout",
                text = "Transform"
            };

            transformFoldout.Add(new Vector3Field
            {
                label = "Offset",
                value = translation
            });

            transformFoldout.Add(new Vector3Field
            {
                label = "Rotation",
                value = rotation
            });

            transformFoldout.Add(new Vector3Field
            {
                label = "Scale",
                value = scale
            });

            tileFoldout.Add(transformFoldout);
            foldout.Add(tileFoldout);
        }

        root.Add(foldout);

        return root;
    }
}
