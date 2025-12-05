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
| **Transform4D** | Stores and applies 4D rotation, position, and scale (no mesh data) |
| **MeshRenderer4D** | Owns Mesh4D, applies transforms, slices with hyperplane, outputs 3D mesh |
| **Plane4D** | Defines slicing hyperplane (normal + offset) |
| **MeshCompositor4D** | Creates 4D mesh from two 3D meshes, assigns to MeshRenderer4D |
| **Cube4DInitializer** | Factory for tesseracts, spheres, and morphs |

### Data Flow

```
                    MeshRenderer4D
                    ├── Mesh4D (owns the 4D mesh data)
                    ├── Transform4D (reference for transforms)
                    ├── Plane4D (slicing hyperplane)
                    └── MeshFilter (3D output)
                           ↓
                      MeshRenderer (displays 3D slice)

MeshCompositor4D ──────► MeshRenderer4D.mesh4D
```

### Game Logic (`Assets/Scripts/`)

- **GameManager** - Singleton managing W coordinate and slicing plane offset (Q/E keys)
- **ChangeW / ListenToW** - Scale objects based on W value
- **FlyingPlayerMover** - Levitating first-person controller

## Quick Start

### Create a 4D Object

**Menu:** `GameObject > 4D > Composite Object > Create New`

Or manually:
1. Add `Transform4D` component (for 4D transforms)
2. Add `MeshRenderer4D` component (owns mesh data, does slicing)
3. Assign a `Mesh4D` asset to MeshRenderer4D
4. Add `MeshFilter`, `MeshRenderer` for display
5. Wire references

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
| MeshCompositor4D.cs | `Assets/4d try/` |
| Plane4D.cs | `Assets/4d try/` |
| GameManager.cs | `Assets/Scripts/` |
| PlayerMovement.cs | `Assets/Scripts/` |
| Object4DSetup.cs | `Assets/Editor/` |

## Migration Notes

### Mesh4D Ownership Change (composite-cont branch)

**Previous architecture:** `Transform4D` owned `Mesh4D`
**Current architecture:** `MeshRenderer4D` owns `Mesh4D`

Migration is automatic via `OnValidate()`:
- **MeshRenderer4D** pulls `mesh4D` from `Transform4D.deprecatedMesh4D` if present
- **MeshCompositor4D** migrates old `m_transform` reference to `m_MeshRenderer4D`

When opening old scenes, migration runs automatically. Save the scene to persist changes.
