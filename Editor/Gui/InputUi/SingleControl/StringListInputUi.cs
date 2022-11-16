﻿using System.Collections.Generic;
using ImGuiNET;
using T3.Editor.Gui.InputUi;

namespace Editor.Gui.InputUi.SingleControl
{
    public class StringListInputUi : SingleControlInputUi<List<string>>
    {
        public override IInputUi Clone()
        {
            return new StringListInputUi
                   {
                       InputDefinition = InputDefinition,
                       Parent = Parent,
                       PosOnCanvas = PosOnCanvas,
                       Relevancy = Relevancy
                   };
        }

        protected override bool DrawSingleEditControl(string name, ref List<string> list)
        {
            var outputString = string.Join(", ", list);
            ImGui.TextUnformatted($"{outputString}");
            return false;
        }

        protected override void DrawReadOnlyControl(string name, ref List<string> list)
        {
            var outputString = list != null ? string.Join(", ", list) : "<null>";
            ImGui.TextUnformatted(outputString);
        }
    }
}