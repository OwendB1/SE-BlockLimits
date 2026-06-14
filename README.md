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

The plugin version lives in `Version.Build.props` (committed, imported by `Directory.Build.props`).
Bump the version there.

`Directory.Build.props.template` is a template for `Directory.Build.props`, a local, **not
committed** config file you can use to override the reference folder paths (`Bin64`, `Pulsar`,
`Magnetar`, `Dedicated64`). Running `setup.py` copies the template to `Directory.Build.props`
if it does not exist yet and fills in the auto-detected paths. Leaving a path empty falls back
to the platform-specific auto-detection (Windows and Linux) further down in the file.

Functionality is inspired by and reimplements the original Torch plugin
BlockLimiter by N1Ran: https://github.com/N1Ran/BlockLimiter
