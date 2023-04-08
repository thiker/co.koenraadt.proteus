using System.ComponentModel;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;

public class GODigiTwinController : MonoBehaviour
{
    public Material XrayMaterial;
    private PTGlobals _globalsData;

    // Start is called before the first frame update
    void Start()
    {
        _globalsData = Repository.Instance.Proteus.GetGlobals();
        _globalsData.PropertyChanged += OnGlobalsDataChanged;
    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        _globalsData.PropertyChanged -= OnGlobalsDataChanged;
    }
    

    private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "SelectedNodes":
                {
                    break;
                }
        }
    }
}
