using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using co.koenraadt.proteus.Runtime.ViewModels;

/// <summary>
/// Example of a custom digital twin component for a car that listens to changes of the engine's state and changes the car's offset accordingly / emits exhaust particles.
/// </summary>
public class SampleCarDigiTwinComponent : GODigiTwinComponent
{
    /// <summary>
    /// The position that the car should be offset when the engine starts running.
    /// </summary>
    public Vector3 RunningOffset = new Vector3(0,0,0);

    /// <summary>
    /// The speed that the car should move to the new offset.
    /// </summary>
    public float Speed = .01f;

    /// <summary>
    /// Reference to the particle system that should play when the engine starts running.
    /// </summary>
    public ParticleSystem ExhaustParticleSystem;

    private Vector3 _startPos;
    private bool _isRunning = false;

    override protected void  Start()
    {
        base.Start();
        _startPos = transform.position;
    }

    override protected void OnStateDataChanged(PTState obj, PropertyChangedEventArgs e)
    {
        Dictionary<string, object> values = obj.Values;
        object inStateObj;
        values.TryGetValue("InState", out inStateObj);
        string inState = inStateObj.ToString(); 
        
        if (inState == "Running")
        {
            ExhaustParticleSystem.Play();
            _isRunning = true;
        } else
        {
            ExhaustParticleSystem.Stop();
            _isRunning = false;
        }
    }


    override protected void Update()
    {
        base.Update();

        if (_isRunning)
        {
            Vector3 endPos = _startPos + RunningOffset;
            Vector3 difference = endPos - transform.position;
            transform.position += Speed * difference;
        } else
        {
            Vector3 endPos = _startPos;
            Vector3 difference = endPos - transform.position;
            transform.position += Speed * difference;
        }
    }
}
