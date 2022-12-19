using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Summit.UGame;
using UnityEngine.UIElements;
using Summit.UGame.Terrain;
using UnityEditor.UIElements;

namespace Summit.UGame
{
    [CustomEditor(typeof(TestComponent))]
    public class TestComponentEditor : Editor
    {
        public override VisualElement CreateInspectorGUI() {

            var container = new VisualElement();

            var vecField = new Vector3Field();
            vecField.bindingPath = nameof(TestComponent.vec_);
            container.Add(vecField);

            var blField = new BlkEdgeField();
            blField.bindingPath = nameof(TestComponent.edge_);
            container.Add(blField);

            container.Bind(serializedObject);

            return container;
        }
    }
}
