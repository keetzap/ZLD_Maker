# Selection History Plus

Selection History Plus is a Unity Editor extension designed to enhance your workflow by keeping track of your valid object selections. It provides a robust history interface with pinning, filtering, and quick access capabilities.

## Features

### 1. Smart Selection Tracking
- Automatically records objects selected in the Project or Hierarchy windows.
- Toggle recording on/off to pause history tracking (`ICO_GUI_RecordingON`, `ICO_GUI_RecordingOFF`).

### 2. History Management
- **Pinning**: Keep important objects permanently in the list (`Pin`, `Unpin`). Supported icons: `ICO_GUI_Pinned`, `ICO_GUI_Unpinned`.
- **Locking**: Prevent the history list from updating or clearing accidentally (`ICO_GUI_Locked`, `ICO_GUI_Unlocked`).
- **Deletion**: Remove specific items or clear the entire unpinned history (`ICO_GUI_Trash`, `Delete from history`).

### 3. Quick Actions
- **Show in Inspector**: Ping and highlight the object in its respective window without selecting it.
- **Open Prefab**: Quickly open the prefab asset associated with a history item.
- **Drag & Drop**: Drag items from the history list directly into scene slots or other inspector fields.

### 4. Organization & Filtering
- **Type Filtering**: Filter history items by their type (GameObject, Asset, specific Component, etc.) using `SelectionHistoryPlusTypeFilter`.
- **Sorting**: Reorder history items for better visibility (`ICO_GUI_Sorting`).
- **Pin on Top**: Option to keep pinned items at the top of the list (`ICO_GUI_PinnedOnTop`).
