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
    [CustomEditor(typeof(TestCompnent))]
    public class TestComponentEditor : Editor
    {
        public override VisualElement CreateInspectorGUI() {

            var container = new VisualElement();

            var vecField = new Vector3Field();
            vecField.bindingPath = nameof(TestCompnent.vec_);
            container.Add(vecField);

            var blField = new BlkEdgeField();
            blField.bindingPath = nameof(TestCompnent.edge_);
            container.Add(blField);

            //vecField.Bind(serializedObject);
            container.Bind(serializedObject);

            return container;
        }
    }
}
