using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;

#nullable enable


namespace Summit.UGame.Terrain
{
    public class BlkEdgeField : BaseField<BlkEdge>
    {
        private const int layerButtonWidth = 0x8;
        private const int layerButtonMargin = 0x1;

        private const int lsMargin = 0x2;

        private const int layerButtonTotalWidth = layerButtonWidth + layerButtonMargin * 2;
        private const int layerTotalWidth = layerButtonTotalWidth * 3;

        private enum SelState
        {
            UnSelected = 0x0,
            Selected = 0x1,
            SelectedOtherLayer = 0x2,
            Invalid = 0x3,
        }

        private static readonly Color unselectedColor = new(0.3f, 0.3f, 0.3f);
        private static readonly Color selectedColor = new(0.8f, 0.6f, 0.3f);
        private static readonly Color selectedOtherColor = new(0.6f, 0.6f, 0.3f);
        private static readonly Color midSectionColor = new(0.1f, 0.1f, 0.1f);



        private int viewLayer_ = 0;


        private SectionSelector sections_;
        private LayerSelector layers_;


        private class SectionSelector : VisualElement
        {
            private readonly BlkEdgeField parent_;

            private El[] els_;

            private class El : VisualElement
            {
                private readonly SectionSelector parent_;
                private int s_, x_, z_;

                internal El(SectionSelector parent, int s, int x, int z) {
                    parent_ = parent;
                    s_ = s;
                    x_ = x;
                    z_ = z;

                    this.style.position = Position.Absolute;
                    this.style.width = layerButtonWidth;
                    this.style.height = layerButtonWidth;

                    this.style.translate = new Translate(layerButtonMargin + layerButtonTotalWidth * x,
                        layerButtonMargin + layerButtonTotalWidth * z, 0);

                    this.RegisterCallback<MouseDownEvent>(OnClick);
                }

                void OnClick(MouseDownEvent ev) {
                    parent_.parent_.SelectSection(s_);
                }

                internal void SetSelState(SelState state) {
                    this.style.backgroundColor = state switch {
                        SelState.Selected => selectedColor,
                        SelState.SelectedOtherLayer => selectedOtherColor,
                        SelState.Invalid => midSectionColor,
                        _ => unselectedColor,
                    };
                }
            }


            internal SectionSelector(BlkEdgeField parent) {
                parent_ = parent;

                this.style.width = layerTotalWidth;
                this.style.height = layerTotalWidth;

                els_ = new El[9];
                AddEl_(0, 0, 2); AddEl_(1, 0, 1); AddEl_(2, 0, 8);
                AddEl_(0, 1, 3); AddEl_(1, 1, 0); AddEl_(2, 1, 7);
                AddEl_(0, 2, 4); AddEl_(1, 2, 5); AddEl_(2, 2, 6);

                void AddEl_(int x, int z, int s) {
                    var e = els_[s] = new(this, s, x, z);
                    this.Add(e);
                }
            }


            internal void UpdateSelection() {
                var v = parent_.value;

                var sIdx = v.section;
                var curLayer = v.layer + 1 == parent_.viewLayer_;
                var midLayer = parent_.viewLayer_ == 1;

                for (int i = 0; i < 9; i++) {
                    var state = i == sIdx
                        ? curLayer
                            ? SelState.Selected
                            : SelState.SelectedOtherLayer
                        : midLayer && i == 0
                            ? SelState.Invalid
                            : SelState.UnSelected;

                    els_[i].SetSelState(state);
                }
            }
        }

        private class LayerSelector : VisualElement
        {
            private readonly BlkEdgeField parent_;

            private El[] els_;

            private class El : VisualElement
            {
                private readonly LayerSelector parent_;
                private int l_, i_;

                internal El(LayerSelector parent, int l, int i) {
                    parent_ = parent;
                    l_ = l;
                    i_ = i;

                    this.style.position = Position.Absolute;
                    this.style.width = layerButtonWidth;
                    this.style.height = layerButtonWidth;

                    this.style.translate = new Translate(0,
                        layerButtonMargin + layerButtonTotalWidth * i, 0);

                    this.RegisterCallback<MouseDownEvent>(OnClick);
                }

                void OnClick(MouseDownEvent ev) {
                    parent_.parent_.SelectLayer(i_);
                }

                internal void SetSelState(SelState state) {
                    this.style.backgroundColor = state switch {
                        SelState.Selected => selectedColor,
                        SelState.SelectedOtherLayer => selectedOtherColor,
                        _ => unselectedColor,
                    };
                }
            }


            internal LayerSelector(BlkEdgeField parent) {
                parent_ = parent;

                this.style.width = layerButtonTotalWidth;
                this.style.height = layerTotalWidth;
                this.style.marginLeft = lsMargin;

                els_ = new El[9];
                AddEl_(0, -1);
                AddEl_(1, 0);
                AddEl_(2, 1);

                void AddEl_(int i, int l) {
                    var e = els_[i] = new(this, l, i);
                    this.Add(e);
                }
            }

            internal void UpdateSelection() {
                var v = parent_.value;

                var lIdx = v.layer + 1;

                for (int i = 0; i < 3; i++) {
                    var state =  i == lIdx
                        ? SelState.Selected
                        : i == parent_.viewLayer_
                            ? SelState.SelectedOtherLayer
                            : SelState.UnSelected;

                    els_[i].SetSelState(state);
                }
            }
        }



        public BlkEdgeField(string? label = null)
            : base(label, null) {

            this.style.minHeight = layerTotalWidth;
            this.style.minWidth = layerTotalWidth + layerButtonTotalWidth;

            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            this.Add(container);

            sections_ = new(this);
            container.Add(sections_);

            layers_ = new(this);
            container.Add(layers_);

            UpdateSelection_();
        }

        #region Callbacks

        void SelectSection(int sectionIdx) {
            value = new(viewLayer_ - 1, sectionIdx);

            UpdateSelection_();
        }

        void SelectLayer(int layerIdx) {
            viewLayer_ = layerIdx;
            UpdateSelection_();
        }

        #endregion Callbacks

        #region Internal

        private void UpdateSelection_() {
            layers_.UpdateSelection();
            sections_.UpdateSelection();
        }

        #endregion Internal
    }



    [CustomPropertyDrawer(typeof(BlkEdge))]
    public class BlkEdgeEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) { // TODO not writing value to disk
            var f = new BlkEdgeField(property.name) { name = property.propertyPath, bindingPath = property.propertyPath };

            return f;
        }

    }
}