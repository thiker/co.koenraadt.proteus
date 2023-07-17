using co.koenraadt.proteus.Runtime.ViewModels;
using System.ComponentModel;

/// <summary>
/// Example of a simple Digital Twin Component
/// </summary>
public class SampleCarPartDigiTwinComponent : GODigiTwinComponent
{
    private void Start()
    {
        base.Start(); // always call the parents start as well.
    }

    override protected void OnStateDataChanged(PTState obj, PropertyChangedEventArgs e)
    {
        // Implement custom behavior when states data changes here...
    }

    override protected void Update()
    {
        base.Update(); // always call the parents update as well.
    }
}
