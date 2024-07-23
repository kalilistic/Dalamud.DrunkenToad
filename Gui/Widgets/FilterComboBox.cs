namespace Dalamud.DrunkenToad.Gui.Widgets;

using System.Collections.Generic;
using System.Linq;
using Core;
using Dalamud.Interface.Utility;
using Loc.ImGui;
using ImGuiNET;

/// <summary>
/// Filterable combo box.
/// </summary>
public class FilterComboBox
{
    private readonly IReadOnlyList<string> items;
    private readonly string filterTextHint;
    private readonly string noMatchFoundText;
    private List<string> filteredItems;
    private string filter;
    private string selectedItem;
    private ImGuiListClipperPtr clipper;
    private bool comboOpened;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterComboBox"/> class.
    /// </summary>
    /// <param name="items">Items to display.</param>
    /// <param name="filterTextHint">Filter text hint.</param>
    /// <param name="noMatchFoundText">No match found text.</param>
    public FilterComboBox(IReadOnlyList<string> items, string filterTextHint, string noMatchFoundText)
    {
        this.filterTextHint = filterTextHint;
        this.noMatchFoundText = noMatchFoundText;
        this.items = items;
        this.filteredItems = items.ToList();
        this.selectedItem = string.Empty;
        this.filter = string.Empty;
        unsafe
        {
            this.clipper = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
        }
    }

    /// <summary>
    /// Draw the combo box.
    /// </summary>
    /// <param name="label">Label.</param>
    /// <param name="width">Width.</param>
    /// <param name="itemHeight">Item height.</param>
    /// <param name="flags">Flags.</param>
    /// <returns>Selected index.</returns>
    public int? Draw(string label, float width = 100f, float itemHeight = 30f, ImGuiComboFlags flags = ImGuiComboFlags.None)
    {
        ImGui.SetNextItemWidth(width);

        var previewValue = !string.IsNullOrEmpty(this.selectedItem) ? this.selectedItem : string.Empty;
        int? selectedIndex = null;

        var localizedLabel = DalamudContext.LocManager.GetString(label);
        if (ImGui.BeginCombo(localizedLabel, previewValue, flags | ImGuiComboFlags.HeightLargest))
        {
            this.comboOpened = true;
            this.DrawFilter();
            ImGui.Separator();
            selectedIndex = this.DrawItems(itemHeight);
            if (selectedIndex.HasValue)
            {
                this.selectedItem = this.filteredItems[selectedIndex.Value];
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndCombo();
        }
        else if (this.comboOpened)
        {
            this.comboOpened = false;
            this.filter = string.Empty;
            this.UpdateFilter();
        }

        return selectedIndex.HasValue ? this.items.ToList().FindIndex(item => item == this.selectedItem) : null;
    }

    private void DrawFilter()
    {
        if (LocGui.InputTextWithHint("#FilterText", this.filterTextHint, ref this.filter, 256))
        {
            this.UpdateFilter();
        }
    }

    private int? DrawItems(float itemHeight)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 0f);
        var totalItemsHeight = this.filteredItems.Count * itemHeight;
        var maxHeight = 10 * itemHeight;
        var minHeight = itemHeight * 1.2f;
        var childHeight = System.Math.Max(totalItemsHeight, minHeight);
        childHeight = System.Math.Min(childHeight, maxHeight);

        ImGui.BeginChild("##ItemList", ImGuiHelpers.ScaledVector2(0, childHeight), true, totalItemsHeight >= maxHeight ? ImGuiWindowFlags.AlwaysVerticalScrollbar : ImGuiWindowFlags.None);

        int? selectedIndex = null;

        if (this.filteredItems.Count == 0)
        {
            LocGui.Text(this.noMatchFoundText);
        }
        else
        {
            this.clipper.Begin(this.filteredItems.Count);

            while (this.clipper.Step())
            {
                for (var i = this.clipper.DisplayStart; i < this.clipper.DisplayEnd; i++)
                {
                    var value = this.filteredItems[i];
                    var isSelected = value == this.selectedItem;
                    if (ImGui.Selectable(value, isSelected))
                    {
                        selectedIndex = i;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
            }

            this.clipper.End();
        }

        ImGui.EndChild();
        ImGui.PopStyleVar();

        return selectedIndex;
    }

    private void UpdateFilter()
    {
        this.filteredItems = string.IsNullOrEmpty(this.filter)
            ? this.items.ToList()
            : this.items.Where(item => item.Contains(this.filter, System.StringComparison.CurrentCultureIgnoreCase)).ToList();

        if (!this.filteredItems.Contains(this.selectedItem))
        {
            this.selectedItem = string.Empty;
        }
    }
}
