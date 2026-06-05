# BlockLimits

BlockLimits is a Space Engineers dedicated server plugin for Magnetar. It is a PluginSDK rewrite of the old Torch BlockLimiter plugin.

This port provides:

- Magnetar/PluginSDK server configuration
- Block limit rules by player, faction, and grid
- Optional vanilla block type limit import
- Grid size limits
- `!blocklimit` server commands
- Cross-plugin API: `BlockLimits.PluginApi.Limits.CanAdd(...)`

Nexus synchronization from the Torch plugin is intentionally not included.

## Commands

- `!blocklimit enable [true|false]`
- `!blocklimit update`
- `!blocklimit mylimit`
- `!blocklimit limits`
- `!blocklimit pairnames [blockType]`

## Build

Build with:

```bash
dotnet build BlockLimits.sln -c Release
```
