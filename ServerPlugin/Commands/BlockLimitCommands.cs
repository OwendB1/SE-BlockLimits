using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSdk.Commands;
using Sandbox.Definitions;
using Sandbox.Game.World;
using VRage.Game;
using VRage.Game.ModAPI;

namespace ServerPlugin.Commands;

[CommandRoot("blocklimit", "BlockLimits", "Block limit commands")]
public sealed class BlockLimitCommands : CommandModule
{
    [Command("enable", "Enable or disable BlockLimits.")]
    [Permission(MyPromoteLevel.Admin)]
    public string Enable(bool enable = true)
    {
        Plugin.Instance.Config.Enabled = enable;
        if (enable)
            Plugin.Instance.Limits.Recalculate();

        return enable ? "BlockLimits Enabled" : "BlockLimits Disabled";
    }

    [Command("update", "Recalculate all limit counters.")]
    [Permission(MyPromoteLevel.Moderator)]
    public string Update()
    {
        Plugin.Instance.Limits.Recalculate();
        return "Limits updated";
    }

    [Command("mylimit", "List current player limit status.")]
    [Permission(MyPromoteLevel.None)]
    public string MyLimit()
    {
        if (!Plugin.Instance.Config.Enabled)
            return "Plugin disabled";

        if (Context.Caller.IsConsole || Context.Caller.IdentityId == 0)
            return "Command can only be run in-game by players";

        return Plugin.Instance.Limits.BuildLimitReport(Context.Caller.IdentityId);
    }

    [Command("limits", "List configured limits.")]
    [Permission(MyPromoteLevel.None)]
    public string Limits()
    {
        if (!Plugin.Instance.Config.Enabled)
            return "Plugin disabled";

        return Plugin.Instance.Limits.BuildLimitsList();
    }

    [Command("pairnames", "List cube block pair names.")]
    [Permission(MyPromoteLevel.None)]
    public string PairNames(string blockType = null)
    {
        IEnumerable<MyDefinitionBase> definitions = MyDefinitionManager.Static.GetAllDefinitions()
            .Where(definition => definition is MyCubeBlockDefinition);

        if (!string.IsNullOrWhiteSpace(blockType))
            definitions = definitions.Where(definition => definition.Id.TypeId.ToString().Substring(16).Contains(blockType, StringComparison.OrdinalIgnoreCase));

        StringBuilder sb = new StringBuilder();
        foreach (IGrouping<MyModContext, string> group in definitions
                     .OfType<MyCubeBlockDefinition>()
                     .Where(definition => definition.Context != null)
                     .GroupBy(definition => definition.Context, definition => definition.BlockPairName))
        {
            sb.AppendLine(group.Key.IsBaseGame ? $"[{group.Distinct().Count()} Vanilla blocks]" : $"[{group.Distinct().Count()} blocks --- {group.Key.ModName} - {group.Key.ModId}]");
            foreach (string pairName in group.Distinct().OrderBy(name => name))
                sb.AppendLine(pairName);
            sb.AppendLine();
        }

        return sb.Length == 0 ? "No definitions found" : sb.ToString();
    }
}
