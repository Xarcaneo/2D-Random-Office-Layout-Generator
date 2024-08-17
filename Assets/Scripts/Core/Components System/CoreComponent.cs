using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The <c>CoreComponent</c> class serves as a base class for all components that are managed by the <c>Core</c> class.
/// It implements the <c>IInitialize</c> interface, ensuring that each derived component can be initialized
/// as part of the core system.
/// </summary>
public class CoreComponent : MonoBehaviour, IInitialize
{
    /// <summary>
    /// A reference to the <c>Core</c> class that manages this component.
    /// This reference is set during the <c>Awake</c> phase when the component is first initialized.
    /// </summary>
    protected Core core;

    /// <summary>
    /// Called when the script instance is being loaded. This method ensures that the component is
    /// correctly associated with its parent <c>Core</c> and adds itself to the core's list of managed components.
    /// </summary>
    protected virtual void Awake()
    {
        core = transform.parent.GetComponent<Core>();

        if (core == null)
        {
            Debug.LogError("There is no Core on the parent");
        }
        else
        {
            core.AddComponent(this);
        }
    }

    /// <summary>
    /// The <c>Initialize</c> method is called to perform any necessary initialization logic for the component.
    /// This method is virtual, allowing derived classes to override and provide their own initialization logic.
    /// </summary>
    public virtual void Initialize()
    {
        // Custom initialization logic for derived classes should be implemented here.
    }
}
