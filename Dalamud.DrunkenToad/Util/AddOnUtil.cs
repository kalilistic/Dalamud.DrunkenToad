using Dalamud.DrunkenToad.Core;
using Dalamud.DrunkenToad.Extension;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Dalamud.DrunkenToad.Util;

/// <summary>
/// More easily work with GUI AddOns.
/// </summary>
public unsafe class AddOnHelper
{
    /// <summary>
    /// Check if addon is ready.
    /// </summary>
    /// <param name="addon">addon.</param>
    /// <returns>indicator if addon is ready.</returns>
    public static bool IsAddonReady(AtkComponentNode* addon)
    {
        return addon != null && addon->AtkResNode.IsVisible && addon->Component->UldManager.LoadedState == AtkLoadState.Loaded;
    }

    /// <summary>
    /// Check if addon is ready.
    /// </summary>
    /// <param name="addon">addon.</param>
    /// <returns>indicator if addon is ready.</returns>
    public static bool IsAddonReady(AtkTextNode* addon)
    {
        return addon != null && addon->AtkResNode.IsVisible;
    }

    /// <summary>
    /// Check if unit base is ready.
    /// </summary>
    /// <param name="unitBase">unit base.</param>
    /// <returns>indicator if unit base is ready.</returns>
    public static bool IsUnitBaseReady(AtkUnitBase* unitBase)
    {
        return unitBase != null && unitBase->IsVisible && unitBase->UldManager.LoadedState == AtkLoadState.Loaded;
    }

    /// <summary>
    /// Extract text from component node.
    /// </summary>
    /// <param name="unitBase">unit base.</param>
    /// <param name="textNodeId">text node id.</param>
    /// <param name="componentNodeId">component node id.</param>
    /// <returns>node text.</returns>
    public static string GetNodeTextFromComponentNode(AtkUnitBase* unitBase, uint textNodeId, uint componentNodeId)
    {
        var componentNode = (AtkComponentNode*)unitBase->UldManager.SearchNodeById(componentNodeId);
        if (!IsAddonReady(componentNode)) return string.Empty;
        var textNode = (AtkTextNode*)componentNode->Component->UldManager.SearchNodeById(textNodeId);
        if (!IsAddonReady(textNode)) return string.Empty;
        return DalamudContext.PluginInterface.Sanitize(textNode->NodeText.ToString());
    }

    /// <summary>
    /// Extract text from component node.
    /// </summary>
    /// <param name="unitBase">unit base.</param>
    /// <param name="nodeId">node id.</param>
    /// <returns>node text.</returns>
    public static string GetNodeText(AtkUnitBase* unitBase, uint nodeId)
    {
        var textNode = (AtkTextNode*)unitBase->UldManager.SearchNodeById(nodeId);
        if (!IsAddonReady(textNode)) return string.Empty;
        return DalamudContext.PluginInterface.Sanitize(textNode->NodeText.ToString());
    }
}
