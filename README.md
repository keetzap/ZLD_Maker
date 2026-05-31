# 🎮 ZLD Maker – Unity Zelda‑style Level Designer

## 📖 What the project is
`ZLD Maker` is a **Unity editor extension** that lets designers create Zelda‑style levels quickly. It provides:
- **Auto‑snap on prefab drop** (default 0.5 grid, configurable via the *Scene Utilities* window).
- **Transform shortcut inspector** that adds one‑click actions (`Round Position`, `Rotate ± 90°`, `Add Random Transformation`).
- A **modular gameplay framework** built on interfaces (`ICollectable`, `IDraggable`, `IDropable`, `IHitable`, `IInteractable`, `IListener`).
- **Trigger & Interactable system** that fires events when the player enters zones, touches objects, etc.
- Centralised **FeedbackSystem** for audio/visual cues.
- An **event‑driven architecture** that keeps runtime logic decoupled from concrete implementations.

---

## 📂 Repository Layout (root)
```
ZLD_Maker/
├─ Assets/
│   ├─ AssetStore/Keetzap/ZLDMaker/
│   │   └─ Scripts/                     # All source code
│   │       ├─ Canvas/                  # UI canvases (MainMenu, Pause, GameOver, etc.)
│   │       ├─ Characters/            # Player / NPC prefabs
│   │       ├─ Core/                  # Core utilities (Decorators, BaseEditorWindow, etc.)
│   │       ├─ Editor/                # Editor‑only tools
│   │       │   ├─ PrefabDropSnapper.cs          # Auto‑snap on drop
│   │       │   ├─ SceneUtilities.cs            # Window with buttons + snap config
│   │       │   ├─ TransformCustomInspector.cs # Transform shortcuts
│   │       │   └─ … (other editor scripts)
│   │       ├─ Feedback/              # FeedbackSystem and data assets
│   │       ├─ GameData/               # ScriptableObjects for items, enemies, etc.
│   │       ├─ GameFlow/               # Scene‑transition manager
│   │       ├─ GamePlay/               # **Gameplay core**
│   │       │   ├─ Interfaces/         # ICollectable, IDraggable, …
│   │       │   ├─ Listeners/          # Event listeners (completed)
│   │       │   ├─ Triggers/           # Base Trigger, TriggerByProximity, …
│   │       │   │   └─ Interactables/   # Chest, Fountain, Sign, Timeline, etc.
│   │       │   └─ Other/              # Collectable, DraggableByPath/Tile, Hitable, SimpleDropper
│   │       ├─ Core/                  # Shared runtime utilities (Enumerators, Layers, …)
│   │       └─ … (other modules)
│   └─ ProjectSettings/                # Unity project settings
├─ README.md                         # ← THIS FILE (project overview)
└─ .gitignore
```

---

## 🛠️ Core Systems
| System | Responsibility |
|--------|-----------------|
| **Editor Utilities** | `SceneUtilities` window + `TransformCustomInspector` – expose common actions and store preferences (`snapStep`, `enableNegativeScales`). |
| **Auto‑Snap Prefab Dropper** | Listens to `ObjectChangeEvents` and automatically aligns newly instantiated prefabs to the nearest multiple of `snapStep` (default 0.5). |
| **Gameplay Interfaces** | Defined in `GamePlay/Interfaces`. All runtime behaviours implement the appropriate interface, guaranteeing loose coupling. |
| **Triggers & Interactables** | Base `Trigger` class plus concrete implementations (`TriggerByProximity`, etc.) that raise typed events. Interactable components (Chest, Fountain, Sign, Timeline, Trigger) inherit `IInteractable` and act on those events. |
| **Feedback System** | Centralised `FeedbackSystem` (audio, particle, animation) used by collectables, hitables, etc. |
| **Event‑Driven Architecture** | Objects emit events (`CollectableCollected`, `HitableDamaged`, `TriggerEntered`). Listeners subscribe and react, keeping runtime logic decoupled from concrete classes. |

---

## 🚀 Extensibility Guidelines
1. **Add a new gameplay object**
   - Create a `ScriptableObject` for its data if needed (e.g., `GD_Collectable`).
   - Implement the appropriate interface (`ICollectable`, `IDraggable`, …) in a `MonoBehaviour`.
   - Register any custom events it needs to fire.
2. **Expose a new editor action**
   - Add a button to `SceneUtilities` *or* `TransformCustomInspector`.
   - Put the core logic in a static method (e.g., `GameplayUtils.RotateY90(Selection.gameObjects)`) so it can be reused from both UI locations.
3. **Create a new trigger**
   - Inherit from `Trigger` or an existing concrete trigger class.
   - Implement `OnTriggerEnter`/`OnTriggerExit` logic and raise a typed event.
   - If player interaction is required, add an `Interactable` component that implements `IInteractable`.
4. **Maintain decoupling**
   - **Never** call `GameManager` directly from gameplay scripts. Use events instead.
   - Keep editor code separate from runtime (`Editor` folder vs. `Runtime` folder). 
5. **Testing**
   - Write unit tests that target interface contracts and event flow.
   - Use the Unity Test Framework to verify that a `Trigger` fires the correct event and that a listener reacts appropriately.

---

## 🎯 Quick Start for Designers
1. **Open the editor utilities** – `Keetzap → Scene Utilities`.
2. Adjust **Snap Step** (default 0.5) to change rounding granularity.
3. Drag any prefab into the scene – it will snap automatically.
4. Select a GameObject and use the **Transform** inspector shortcuts:
   - `Round Position` (uses current Snap Step)
   - `Rotate +90°` / `Rotate –90°` on the Y‑axis
   - `Add Random Transformation` (random Y‑rotation ± 90°, optional negative scale).
5. Add a **Trigger** component to any GameObject to fire events when the player enters that zone.
6. Add an **Interactable** (Chest, Fountain, Sign, etc.) to define what happens when the trigger fires.
7. Play the scene – feedback will be shown automatically via `FeedbackSystem`.

---

## 📚 Further Reading
- `Assets/Keetzap/Scripts/Editor/README.md` – detailed description of each editor tool.
- `Assets/Keetzap/Scripts/GamePlay/README.md` – gameplay architecture deep‑dive (interfaces, event flow, trigger hierarchy).
- `Assets/Keetzap/Documentation/FeedbackSystem.md` – how to configure audio/visual feedback.

---

*This README lives at the repository root and provides a concise yet comprehensive snapshot of the project. Future contributors should read it first to understand the purpose, structure, and extension points.*
