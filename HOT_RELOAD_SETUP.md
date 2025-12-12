# Hot Reload Configuration Guide

This solution has been configured to support Hot Reload and automatic browser refresh when files are edited from external editors (Cursor, VS Code, etc.).

## Configuration Summary

### 1. launchSettings.json
- ✅ Added `hotReloadEnabled: true` to all profiles (http, https, IIS Express)
- ✅ Added `hotReloadProfile: "aspnetcore"` to all profiles

### 2. GMS.WebUI.csproj
- ✅ Added `HotReloadOnFileChange` property set to `true`
- ✅ Added `WatchInclude` pattern to watch: `**/*.cs`, `**/*.cshtml`, `**/*.razor`, `**/*.js`, `**/*.css`, `**/*.json`

### 3. Program.cs
- ✅ Hot Reload is enabled by default in .NET 8 Development environment
- ✅ No additional Browser Link configuration needed (deprecated in .NET 6+)

## How to Use

### Option 1: Visual Studio
1. Open the solution in Visual Studio
2. Ensure "Detect when a file is changed outside the environment" is enabled:
   - Tools → Options → Projects and Solutions → General
   - Check "Detect when a file is changed outside the environment"
   - Check "Reload automatically"
3. Run the project (F5 or Ctrl+F5)
4. Edit files in Cursor/VS Code and save
5. Visual Studio will detect changes and apply Hot Reload automatically

### Option 2: dotnet watch (Recommended for External Editors)
1. Open terminal in the `GMS/src/GMS.WebUI` directory
2. Run: `dotnet watch run`
3. The application will:
   - Watch for file changes in C#, Razor, JS, CSS, JSON files
   - Automatically rebuild when changes are detected
   - Apply Hot Reload to update the running application
   - Refresh the browser automatically (if configured)

### Option 3: dotnet watch with specific profile
```bash
cd GMS/src/GMS.WebUI
dotnet watch run --launch-profile https
```

## File Types Watched
- `**/*.cs` - C# source files
- `**/*.cshtml` - Razor views
- `**/*.razor` - Razor components
- `**/*.js` - JavaScript files
- `**/*.css` - CSS stylesheets
- `**/*.json` - JSON configuration files

## Troubleshooting

### Hot Reload Not Working?
1. Ensure you're running in Development environment
2. Check that `ASPNETCORE_ENVIRONMENT=Development` in launchSettings.json
3. Verify `HotReloadOnFileChange` is set to `true` in .csproj
4. Try restarting the application

### External File Changes Not Detected?
1. In Visual Studio: Tools → Options → Projects and Solutions → General
   - Enable "Detect when a file is changed outside the environment"
   - Set to "Reload automatically"
2. Use `dotnet watch run` instead of regular `dotnet run`

### Browser Not Refreshing?
- Hot Reload updates the application automatically
- For static files (JS/CSS), you may need to hard refresh (Ctrl+F5)
- Consider using browser extensions that auto-refresh on file changes

## Notes
- Hot Reload works best with C# and Razor files
- Some changes (like Program.cs modifications) may require a full restart
- Static files (JS/CSS) are served directly and may need browser refresh
- Hot Reload is only available in Development environment

