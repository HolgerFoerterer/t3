﻿using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using T3.Core.Operator;
using T3.Core.Operator.Slots;
using T3.Core.Utils;
using T3.Editor.Gui.ChildUi.WidgetUi;
using T3.Editor.Gui.Interaction;
using T3.Editor.Gui.Styling;
using T3.Editor.Gui.UiHelpers;
using T3.Operators.Types.Id_5d7d61ae_0a41_4ffa_a51d_93bab665e7fe;

namespace T3.Editor.Gui.ChildUi
{
    public static class ValueUi
    {
        public static SymbolChildUi.CustomUiResult DrawChildUi(Instance instance, ImDrawListPtr drawList, ImRect area)
        {
            if (!(instance is Value valueInstance))
                return SymbolChildUi.CustomUiResult.None;

            var symbolChild = valueInstance.Parent.Symbol.Children.Single(c => c.Id == valueInstance.SymbolChildId);
            ImGui.PushClipRect(area.Min, area.Max, true);
            
            var value = (double)valueInstance.Float.TypedInputValue.Value;
            
            // Draw slider
            var rangeMin = valueInstance.SliderMin.TypedInputValue.Value;
            var rangeMax = valueInstance.SliderMax.TypedInputValue.Value;
            if (MathF.Abs(rangeMax - rangeMin) > 0.0001f)
            {
                var f = MathUtils.NormalizeAndClamp((float)value, rangeMin, rangeMax);
                var w = (int)area.GetWidth() * f;
                drawList.AddRectFilled(area.Min, 
                                       new Vector2(area.Min.X + w, area.Max.Y),
                                       T3Style.Colors.WidgetSlider);
                
                drawList.AddRectFilled(new Vector2(area.Min.X + w, area.Min.Y), 
                                       new Vector2(area.Min.X + w + 1, area.Max.Y),
                                       T3Style.Colors.GraphActiveLine);
            }
            
            // Slider Range
            if (rangeMin == 0 && rangeMax != 0)
            {
                ValueLabel.Draw(drawList, area, new Vector2(1, 1), valueInstance.SliderMax);
            }

            // Interaction
            {
                var editingUnlocked = ImGui.GetIO().KeyCtrl || _activeJogDialInputSlot != null;
                var inputSlot = valueInstance.Float;
                if (editingUnlocked)
                {
                    ImGui.SetCursorScreenPos(area.Min);
                    ImGui.InvisibleButton("button", area.GetSize());
                    
                    if (ImGui.IsItemActivated() && ImGui.GetIO().KeyCtrl)
                    {
                        _jogDialCenter = ImGui.GetIO().MousePos;
                        _activeJogDialInputSlot = inputSlot;
                        drawList.AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), Color.White);
                    }
                    
                    if (_activeJogDialInputSlot == inputSlot)
                    {
                        if (ImGui.IsItemActive())
                        {
                            var modified = JogDialOverlay.Draw(ref value, ImGui.IsItemActivated(), _jogDialCenter, double.NegativeInfinity, double.PositiveInfinity,
                                                           0.01f);
                            if (modified)
                            {
                                if (valueInstance.ClampSlider.TypedInputValue.Value)
                                {
                                    value = value.Clamp(rangeMin, rangeMax);
                                }
                                inputSlot.TypedInputValue.Value = (float)value;
                                inputSlot.Input.IsDefault = false;
                                inputSlot.DirtyFlag.Invalidate();
                            }
                        }
                        else
                        {
                            _activeJogDialInputSlot = null;
                        }
                    }
                }
            }
            
            // Label if instance has title
            if (!string.IsNullOrEmpty(symbolChild.Name))
            {
                WidgetElements.DrawTitle(drawList, area, symbolChild.Name);
            }

            WidgetElements.DrawPrimaryValue(drawList, area, $"{value:0.000}");
            
            ImGui.PopClipRect();
            return SymbolChildUi.CustomUiResult.Rendered | SymbolChildUi.CustomUiResult.PreventInputLabels;
        }

        private static Vector2 _jogDialCenter;
        private static InputSlot<float> _activeJogDialInputSlot;
    }
}