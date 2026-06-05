using System.Collections.Generic;
using System.ComponentModel;

namespace Shared.Config;

public interface IPluginConfig : INotifyPropertyChanged
{
    bool Enabled { get; set; }
    bool DetectCodeChanges { get; set; }
    bool UseVanillaLimits { get; set; }
    int MaxBlocksSmallGrid { get; set; }
    int MaxBlocksLargeGrid { get; set; }
    int MaxBlockSizeShips { get; set; }
    int MaxBlockSizeStations { get; set; }
    int MaxSmallGridsPerPlayer { get; set; }
    int MaxLargeGridsPerPlayer { get; set; }
    string DenyMessage { get; set; }
    List<LimitRule> Limits { get; set; }
}
