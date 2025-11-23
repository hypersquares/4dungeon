# 4Dungeon Developer Guide

## Overview

Unity project (6000.2.4f1) for 4D geometry visualization. The system renders 4D objects by slicing them with 3D hyperplanes and displaying the resulting cross-sections.

## Project Structure

```
Assets/
├── 4d try/          # Core 4D geometry system
├── Editor/          # Custom inspectors and tools
├── Scripts/         # Game logic and player controls
├── Tests/Editor/    # Unit tests
├── Scenes/          # Unity scenes
└── Prefab/          # Reusable prefabs
```

## Core Systems

### 4D Geometry (`Assets/4d try/`)

| Class | Purpose |
|-------|---------|
| **Mesh4D** | ScriptableObject storing 4D vertices and edges |
| **Transform4D** | Applies 4D rotation, position, and scale |
| **MeshRenderer4D** | Slices 4D mesh with hyperplane, outputs 3D mesh |
| **Plane4D** | Defines slicing hyperplane (normal + offset) |
| **MeshCompositor4D** | Creates 4D mesh from two 3D meshes stacked along W |
| **Cube4DInitializer** | Factory for tesseracts, spheres, and morphs |

### Data Flow

```
Mesh4D → Transform4D → MeshRenderer4D → MeshFilter → MeshRenderer
              ↑              ↑
         (4D transforms)  (Plane4D slice)
```

### Game Logic (`Assets/Scripts/`)

- **GameManager** - Singleton managing W coordinate (Q/E keys)
- **ChangeW / ListenToW** - Scale objects based on W value
- **PlayerMovement / PlayerCamera** - First-person controls

## Quick Start

### Create a 4D Object

**Menu:** `GameObject > 4D > Composite Object > Create New`

Or manually:
1. Add `Transform4D` component
2. Assign a `Mesh4D` asset
3. Add `MeshRenderer4D`, `MeshFilter`, `MeshRenderer`
4. Wire references

### 4D Rotation

Transform4D uses 6 rotation planes via `Euler4`:
- XY, YZ, XZ (standard 3D)
- XW, YW, ZW (4D rotations)

## Code Style

- **Private fields:** `m_PascalCase`
- **Public properties:** `camelCase`
- **Use `[SerializeField]`** for fields you would like to edit in the inspector, but keep private.
- See `CLAUDE.md` for full guidelines

## Key Files

| File | Location |
|------|----------|
| Transform4D.cs | `Assets/4d try/` |
| MeshRenderer4D.cs | `Assets/4d try/` |
| Plane4D.cs | `Assets/4d try/` |
| GameManager.cs | `Assets/Scripts/` |
| Object4DSetup.cs | `Assets/Editor/` |
