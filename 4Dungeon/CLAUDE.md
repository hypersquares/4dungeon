# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D project (Unity 6000.2.4f1) focused on 4D geometry visualization. The primary work area is in the `Assets/8Cell/` directory.

## Development Commands

### Unity Operations
- Open project in Unity 6000.2.4f1 or compatible version
- Main scenes are located in `Assets/Scenes/`
- Primary scene: `SuperDuper8Cell.unity` (for 8-cell/tesseract visualization)

### Build and Testing
- Use Unity's built-in build system (File > Build Settings)
- Testing performed through Unity's Play mode and scene view

## Code Style Guidelines

### Naming Conventions

#### Classes & Interfaces
- **PascalCase** for all class and interface names
- File names must **exactly match** the class name

```csharp
// ✅ Correct
public class SphereSelectLogic : MonoBehaviour
public class SphereSelectInput : MonoBehaviour

// ❌ Incorrect
public class sphereSelectLogic : MonoBehaviour
public class sphere_select_logic : MonoBehaviour
```

#### Methods
- **PascalCase** for all method names (public and private)

#### Fields
- **Private fields**: `m_` prefix + **PascalCase**
- **Public properties**: **camelCase**
- **Parameters/locals**: **camelCase**

```csharp
// ✅ Correct - Private fields
[SerializeField]
private XRDirectInteractor m_Interactor;
[SerializeField]
private float m_MinRadius = 0.0175f;
private GameObject m_Sphere;

// ✅ Correct - Public properties
public XRDirectInteractor interactor
{
    get => m_Interactor;
    set => m_Interactor = value;
}

public float minRadius
{
    get => m_MinRadius;
    set => m_MinRadius = value;
}
```

### Unity-Specific Conventions

#### Inspector Fields
- Always use `[SerializeField]` for inspector fields, **never public fields**
- Add `[Tooltip]` for designer clarity

```csharp
[SerializeField]
[Tooltip("The Input Action that will be used to resize the sphere")]
private InputActionProperty m_ChangeRadius;
```

#### Component Dependencies
- Use `[RequireComponent]` to enforce dependencies

```csharp
[RequireComponent(typeof(SphereSelectLogic))]
public class SphereSelectInput : MonoBehaviour
{
    private SphereSelectLogic m_SphereSelectLogic;

    private void Awake()
    {
        m_SphereSelectLogic = GetComponent<SphereSelectLogic>();
    }
}
```

#### Class Organization
Order class members as follows:

1. **Serialized fields** (with attributes)
2. **Private fields**
3. **Public properties**
4. **Unity lifecycle methods** (Awake, Start, OnEnable, OnDisable, etc.)
5. **Public methods**
6. **Private methods**

#### Component Setup Pattern
Use defensive null checks and GetComponent in Awake:

```csharp
private void Awake()
{
    if (m_Interactor == null)
    {
        m_Interactor = GetComponent<XRDirectInteractor>();
        if (m_Interactor == null)
        {
            Debug.LogError("Interactor not found");
        }
    }
}
```

## Documentation
- Use XML documentation comments for public APIs
- Include `/// <summary>` blocks for public properties and methods

```csharp
/// <summary>
/// The minimum radius of the sphere.
/// </summary>
public float minRadius
{
    get => m_MinRadius;
    set => m_MinRadius = value;
}
```

## Working Directory
Focus development work in `Assets/Intersections/` directory. Ignore other packages and code in the broader Assets directory unless explicitly required.

## Script Organization
Search for an appropriate "Scripts" directory to place scripts. If you don't find one, make it.

## Important Warnings

### No File Backups
- **NEVER** create backup files by duplicating them and adding a number to the filename (e.g., `MyScript 1.cs`, `MyScript2.cs`)
- Unity will break if you do this, as it creates duplicate class definitions and meta file conflicts