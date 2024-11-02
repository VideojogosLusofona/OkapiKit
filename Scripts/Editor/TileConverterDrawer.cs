using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OkapiKit.Editor
{
    [CustomPropertyDrawer(typeof(TileConverterRule))]
    public class TileConverterDrawer : PropertyDrawer
    {
        const int TilePreviewSize = 64;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(new Rect(position.x, position.y, position.width, position.height * 2), label, property);

            // Draw label - NO LABEL!
            //label.text = label.text.Replace("Element", "Condition");
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Check if we need both selectors
            var propSource = property.FindPropertyRelative("source");
            var propDest = property.FindPropertyRelative("dest");

            var elemHeight = 20;

            float arrowElemWidth = position.width * 0.1f;
            float elementWidth = (position.width - arrowElemWidth) * 0.5f;

            var sourceRect = new Rect(position.x, position.y, elementWidth, elemHeight);
            var arrowRect = new Rect(sourceRect.xMax, position.y, arrowElemWidth, elemHeight);
            var destRect = new Rect(arrowRect.xMax, position.y, elementWidth, elemHeight);
            
            EditorGUI.PropertyField(sourceRect, propSource, new GUIContent("", "Source Tile"));
            GUI.DrawTexture(arrowRect, GUIUtils.GetTexture("ArrowRight"), ScaleMode.ScaleToFit, true, 1.0f);
            EditorGUI.PropertyField(destRect, propDest, new GUIContent("", "Dest Tile"));

            if (IsDisplayable(propSource) || IsDisplayable(propDest))
            {
                var previewSrcRect = new Rect(position.x + (elementWidth - TilePreviewSize) * 0.5f, position.y + elemHeight + 4, Mathf.Min(TilePreviewSize, elementWidth) - 8, TilePreviewSize - 8);
                DrawTilePreview(propSource, previewSrcRect);

                var previewDestRect = new Rect(destRect.xMin + (elementWidth - TilePreviewSize) * 0.5f, position.y + elemHeight + 4, Mathf.Min(TilePreviewSize, elementWidth) - 8, TilePreviewSize - 8);
                DrawTilePreview(propDest, previewDestRect);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseHeight = base.GetPropertyHeight(property, label) + 2;

            var propSource = property.FindPropertyRelative("source");
            var propDest = property.FindPropertyRelative("dest");

            var height = baseHeight;
            if (IsDisplayable(propSource) || IsDisplayable(propDest))
            {
                height += TilePreviewSize;
            }

            return height;
        }

        bool IsDisplayable(SerializedProperty source)
        {
            if (source == null) return false;
            if (source.objectReferenceValue == null) return false;
            if (source.objectReferenceValue is Tile)
            {
                var tile = source.objectReferenceValue as Tile;

                return (tile.sprite != null);
            }
            return false;
        }

        private void DrawTilePreview(SerializedProperty source, Rect rect)
        {
            if ((source == null) || (source.objectReferenceValue == null))
            {
                // Draw to delete
                GUI.DrawTexture(rect, GUIUtils.GetTexture("Trash"), ScaleMode.ScaleAndCrop);
                return;
            }

            var tile = source.objectReferenceValue as TileBase;

            // Cast TileBase to Tile to access the sprite
            if (tile is Tile tileObj && tileObj.sprite != null)
            {
                // Get the texture from the sprite
                Texture2D tileTexture = tileObj.sprite.texture; 

                // Calculate UV rect to match the sprite's texture region
                Rect spriteRect = tileObj.sprite.textureRect;
                Rect uv = new Rect(
                    spriteRect.x / tileTexture.width,
                    spriteRect.y / tileTexture.height,
                    spriteRect.width / tileTexture.width,
                    spriteRect.height / tileTexture.height
                );

                // Draw the texture with alpha support
                GUI.DrawTexture(rect, GUIUtils.GetTexture("Checkerboard"), ScaleMode.ScaleAndCrop);
                GUI.DrawTextureWithTexCoords(rect, tileTexture, uv, true);
            }
        }
    }
}