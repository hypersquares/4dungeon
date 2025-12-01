# Unity C# Code Style Guide

This skill provides Unity C# code style guidelines for this project.

## Naming Conventions

### Classes & Interfaces
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

### Methods
- **PascalCase** for all method names (public and private)

### Fields
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

## Unity-Specific Conventions

### Inspector Fields
- Always use `[SerializeField]` for inspector fields, **never public fields**
- Add `[Tooltip]` for designer clarity

```csharp
[SerializeField]
[Tooltip("The Input Action that will be used to resize the sphere")]
private InputActionProperty m_ChangeRadius;
```

### Component Dependencies
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

### Class Organization
Order class members as follows:

1. **Serialized fields** (with attributes)
2. **Private fields**
3. **Public properties**
4. **Unity lifecycle methods** (Awake, Start, OnEnable, OnDisable, etc.)
5. **Public methods**
6. **Private methods**

### Component Setup Pattern
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

## Application
Apply these conventions to all C# scripts in the Unity project. Ensure consistency across the codebase.
