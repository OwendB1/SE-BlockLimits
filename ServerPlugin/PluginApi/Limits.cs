using System.Collections.Generic;
using Sandbox.Game.Entities.Cube;

namespace BlockLimits.PluginApi;

public static class Limits
{
    public static bool Enabled => ServerPlugin.Plugin.Instance?.Limits?.Enabled == true;

    public static bool IsEnabled()
        => Enabled;

    public static bool CanAdd(List<MySlimBlock> blocks, long id, out List<MySlimBlock> nonAllowedBlocks)
    {
        if (ServerPlugin.Plugin.Instance?.Limits == null)
        {
            nonAllowedBlocks = new List<MySlimBlock>();
            return true;
        }

        return ServerPlugin.Plugin.Instance.Limits.CanAdd(blocks, id, out nonAllowedBlocks);
    }
}
