# Video Editor Downloads

Download the latest executables and installers for all Video Editor applications.

## C# Video Editor (WPF)

A professional video editing application built with C# and WPF, featuring a modern timeline interface, theme support, and comprehensive video editing capabilities.

### Executable
- **VideoEditor.exe** - Standalone executable (requires .NET 8.0 Runtime)
  - Location: `VideoEditor-CSharp/bin/Release/net8.0-windows/VideoEditor.exe`
  - **Description**: Direct executable for the C# Video Editor. Run this file to launch the application. Requires .NET 8.0 Desktop Runtime to be installed on your system.

### Installer
- **VideoEditor-CSharp-Setup.exe** - Windows installer with setup wizard
  - Location: `VideoEditor-CSharp/VideoEditorCS/Output/VideoEditor-CSharp-Setup.exe`
  - **Description**: Full-featured Windows installer for the C# Video Editor. Includes automatic .NET Runtime installation prompts, Start Menu shortcuts, and standard Windows installation options. Publisher: Robert Foster.

---

## Python Video Editor

A cross-platform video editing application built with Python, PyQt6, and OpenCV, offering similar functionality to the C# version with Python's flexibility.

### Executable
- **VideoEditor.exe** - Standalone executable (no dependencies)
  - Location: `VideoEditor-Python/dist/VideoEditor.exe`
  - **Description**: Self-contained executable for the Python Video Editor. Includes all dependencies (PyQt6, OpenCV, etc.) bundled into a single file. No additional installation required - just download and run.

### Installer
- **VideoEditor-Python-Setup.exe** - Windows installer
  - Location: `VideoEditor-Python/Output/VideoEditor-Python-Setup.exe`
  - **Description**: Windows installer for the Python Video Editor. Creates Start Menu shortcuts and provides a standard installation experience. All Python dependencies are included in the executable, so no Python installation is required.

---

## System Requirements

### C# Version
- **Operating System**: Windows 10/11 (64-bit)
- **Runtime**: .NET 8.0 Desktop Runtime (will be prompted to install if missing)
- **FFmpeg**: Recommended for video preview and export (see application for installation instructions)

### Python Version
- **Operating System**: Windows 10/11 (64-bit)
- **Dependencies**: None (all included in executable)
- **FFmpeg**: Recommended for video preview and export (see application for installation instructions)

---

## Installation Instructions

### Using Installers (Recommended)
1. Download the appropriate installer (.exe file)
2. Double-click the installer
3. Follow the installation wizard
4. Launch from Start Menu or desktop shortcut

### Using Executables
1. Download the executable file
2. Double-click to run (C# version requires .NET 8.0 Runtime)
3. For C# version: If .NET Runtime is missing, you'll be prompted to download it

---

## Features

Both versions include:
- **Timeline-based video editing** - Visual timeline with drag-and-drop support
- **Video preview** - Real-time preview of edits
- **Cut and trim** - Two-point cutting system for precise edits
- **Properties panel** - Adjust brightness, contrast, saturation, volume, and playback speed
- **Theme support** - Multiple color themes (Light, Dark, Ocean, Forest, Sunset, Midnight)
- **Media library** - Import and manage multiple video files
- **Export functionality** - Export edited videos (requires FFmpeg)

---

## Getting Help

- **Tooltips**: Hover over buttons and controls for detailed help
- **Info icons**: Look for ℹ️ icons next to section headers for contextual guidance
- **Status bar**: Check the bottom status bar for operation feedback

---

## Notes

- **FFmpeg**: While the applications will run without FFmpeg, video preview and export features require FFmpeg to be installed. The application will provide instructions if FFmpeg is not detected.
- **.NET Runtime**: The C# version requires .NET 8.0 Desktop Runtime. The installer will help you install it if needed.
- **Python**: The Python version does NOT require Python to be installed - everything is bundled.

---

*Last updated: $(Get-Date -Format "yyyy-MM-dd")*

