using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The <c>Core</c> class is responsible for managing and initializing the core components
/// attached to the GameObject. It works closely with the <c>LayoutManager</c> to manage
/// various components that are essential to the functionality of the GameObject.
/// </summary>
public class Core : MonoBehaviour
{
    /// <summary>
    /// A list of <c>CoreComponent</c> objects managed by this Core. This list holds all
    /// the components that are part of the core system.
    /// </summary>
    private readonly List<CoreComponent> CoreComponents = new List<CoreComponent>();

    /// <summary>
    /// The <c>LayoutManager</c> that is the parent of this Core. It manages the layout
    /// of the components within the GameObject.
    /// </summary>
    public LayoutManager Parent { get; private set; }

    /// <summary>
    /// Called by Unity when the script instance is being loaded. This method initializes
    /// the parent <c>LayoutManager</c> component.
    /// </summary>
    private void Start()
    {
        Parent = GetComponentInParent<LayoutManager>();
    }

    /// <summary>
    /// Initializes all <c>CoreComponent</c> instances managed by this Core. Each component
    /// that has been added to the <c>CoreComponents</c> list will have its <c>Initialize</c>
    /// method called.
    /// </summary>
    public void Initialize()
    {
        foreach (CoreComponent component in CoreComponents)
        {
            component.Initialize();
        }
    }

    /// <summary>
    /// Adds a new <c>CoreComponent</c> to the list of managed components if it isn't
    /// already in the list.
    /// </summary>
    /// <param name="component">The component to add to the core management list.</param>
    public void AddComponent(CoreComponent component)
    {
        if (!CoreComponents.Contains(component))
        {
            CoreComponents.Add(component);
        }
    }

    /// <summary>
    /// Retrieves a component of the specified type from the list of managed core components.
    /// </summary>
    /// <typeparam name="T">The type of the core component to retrieve.</typeparam>
    /// <returns>The component of type <typeparamref name="T"/> if found; otherwise, null.</returns>
    public T GetCoreComponent<T>() where T : CoreComponent
    {
        var comp = CoreComponents.OfType<T>().FirstOrDefault();

        if (comp == null)
        {
            Debug.LogWarning($"{typeof(T)} not found on {transform.parent.name}");
            return null;
        }

        return comp;
    }

    /// <summary>
    /// Retrieves a component of the specified type and assigns it to the provided reference.
    /// </summary>
    /// <typeparam name="T">The type of the core component to retrieve.</typeparam>
    /// <param name="value">A reference to assign the found component to.</param>
    /// <returns>The component of type <typeparamref name="T"/> if found; otherwise, null.</returns>
    public T GetCoreComponent<T>(ref T value) where T : CoreComponent
    {
        value = GetCoreComponent<T>();
        return value;
    }
}
