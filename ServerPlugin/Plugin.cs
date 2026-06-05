using System;
using System.IO;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using PluginSdk.Commands;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using ServerPlugin.Commands;
using Shared.Config;
using Shared.Logging;
using Shared.Patches;
using Shared.Plugin;
using VRage.FileSystem;
using VRage.Game;
using VRage.Plugins;

namespace ServerPlugin;

// ReSharper disable once UnusedType.Global
public class Plugin : IPlugin, ICommonPlugin
{
    public const string Name = "BlockLimits";
    public static Plugin Instance { get; private set; }

    public long Tick { get; private set; }
    public BlockLimitService Limits { get; private set; }
    private static bool failed;

    public IPluginLogger Log => Logger;
    private static readonly IPluginLogger Logger = new PluginLogger(Name);

    public IPluginConfig Config => config?.Data;
    private PersistentConfig<PluginConfig> config;
    private static readonly string ConfigFileName = $"{Name}.cfg";

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public void Init(object gameInstance)
    {
#if DEBUG
        // Allow the debugger some time to connect once the plugin assembly is loaded
        Thread.Sleep(100);
#endif

        Instance = this;

        Log.Info("Loading");

        var configPath = Path.Combine(MyFileSystem.UserDataPath, ConfigFileName);
        config = PersistentConfig<PluginConfig>.Load(Log, configPath);

        var gameVersion = MyFinalBuildConstants.APP_VERSION_STRING.ToString();
        Common.SetPlugin(this, gameVersion, MyFileSystem.UserDataPath);

        Limits = new BlockLimitService();
        ServerCommands.Register(Assembly.GetExecutingAssembly(), typeof(BlockLimitCommands));
        MyCubeGrids.BlockBuilt += OnBlockBuilt;

        if (!PatchHelpers.HarmonyPatchAll(Log, new Harmony(Name)))
        {
            failed = true;
            return;
        }

        Log.Debug("Successfully loaded");
    }

    public void Dispose()
    {
        try
        {
            // TODO: Save state and close resources here, called when the game exists (not guaranteed!)
            // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.
            MyCubeGrids.BlockBuilt -= OnBlockBuilt;
        }
        catch (Exception ex)
        {
            Log.Critical(ex, "Dispose failed");
        }

        Limits = null;
        Instance = null;
    }

    public void Update()
    {
        if (failed)
            return;
        
#if DEBUG
        CustomUpdate();
        Tick++;
#else        
        try
        {
            CustomUpdate();
            Tick++;
        }
        catch (Exception e)
        {
            Log.Critical(e, "Update failed");
            failed = true;
        }
#endif       
    }

    private void CustomUpdate()
    {
        PatchHelpers.PatchUpdates();

        if (Tick % 600 == 0 && Config?.Enabled == true)
            Limits?.Recalculate();
    }

    private void OnBlockBuilt(MyCubeGrid grid, MySlimBlock block)
    {
        if (Config?.Enabled != true || grid == null || block == null)
            return;

        long identityId = block.BuiltBy != 0 ? block.BuiltBy : block.OwnerId;
        if (!Limits.CanAddBlock(grid, block.BlockDefinition, identityId, 1, out string limitName))
        {
            Log.Info("Removing block over limit '{0}': {1} on {2}", limitName, block.BlockDefinition.BlockPairName, grid.DisplayName);
            grid.RemoveBlock(block);
        }

        Limits.Recalculate();
    }
}
