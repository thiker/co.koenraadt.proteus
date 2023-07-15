using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.ComponentModel;
using UnityEngine;
using TMPro;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Other;
using System.Collections.Generic;

/// <summary>
/// Component that handles the behavior of the nodes that are used in the viewer to visually represent the nodes in the 3DML formatted model.
/// </summary>
public class GONode : MonoBehaviour, IProteusInteraction
{
    private string _nodeId;
    private string _attachedViewerId;
    private PTNode _nodeData;
    private PTViewer _attachedViewerData;
    private PTGlobals _globalsData;
    private TextMeshPro _displayNameTMP;
    private GameObject _nodeGameObject;
    private GameObject _displayNameObj;
    private MaterialPropertyBlock _matPropBlock;

    /// <summary>
    /// Called to initialize the node and obtain a reference to the viewer its attached to.
    /// </summary>
    /// <param name="nodeId">The id of the node that the component is linked to.</param>
    /// <param name="attachedViewerId">The id of the viewer that the edge component is attached to.</param>
    public void Init(string nodeId, string attachedViewerId)
    {
        _nodeId = nodeId;
        _attachedViewerId = attachedViewerId;
    }

    /// <summary>
    /// Starts and initializes the node. Obtains references to the gameobjects in the node prefab that are used to visualize the node.
    /// </summary>
    void Start()
    {
        // initialize variables
        _matPropBlock = new MaterialPropertyBlock();

        // Get data
        _nodeData = Repository.Instance.Models.GetNodeById(_nodeId);
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
        _globalsData = Repository.Instance.Proteus.GetGlobals();

        // Get the text object
        _displayNameObj = transform.Find("DisplayNameText").gameObject;
        _displayNameTMP = _displayNameObj.GetComponent<TextMeshPro>();

        // Get the node object
        _nodeGameObject = transform.Find("Node").gameObject;

        // initialize the event listeners
        LinkEventListeners();

        // Set presentation
        UpdateNodePresentation();
    }

    /// <summary>
    /// Updatest the shader of the node and node's text with the attached viewer's world to local matrix so the node and text is croppped to the viewwindow.
    /// </summary>
    void Update()
    {
        //set the matrix
        if (_attachedViewerData?.ViewWindowWorldToLocal != null) {
            _matPropBlock.SetMatrix("_WorldToBox", (Matrix4x4)_attachedViewerData.ViewWindowWorldToLocal);
        }

        //Get a renderer component either of the own gameobject or of a child
        Renderer renderer = _nodeGameObject?.GetComponentInChildren<Renderer>();
        Renderer _displayNameRenderer =  _displayNameObj?.GetComponentInChildren<Renderer>();
        

        //apply propertyBlock to renderer
        renderer?.SetPropertyBlock(_matPropBlock);
        _displayNameRenderer?.SetPropertyBlock(_matPropBlock);
    }

    /// <summary>
    /// Destroys the node and clears any listeners and references to the repository that it created to obtain node data.
    /// </summary>
    void OnDestroy()
    {
        if (_nodeData != null)
        {
            _nodeData.PropertyChanged -= OnNodeDataChanged;
        }

        if (_attachedViewerData != null)
        {
            _attachedViewerData.PropertyChanged -= OnViewerDataChanged;
        }

        if (_globalsData != null)
        {
            _globalsData.PropertyChanged -= OnGlobalsDataChanged;
        }
    }

    /// <summary>
    /// Selects the node whenever it is clicked.
    /// </summary>
    /// <param name="hit">Raycast result from the interaction.</param>
    public void OnPointerDown(RaycastHit hit)
    {
        Debug.Log($"PROTEUS: Clicked on node with identifier, {_nodeData.Id} ");
        if (Repository.Instance.Proteus.IsViewerSelected(_attachedViewerData.Id))
        {
            Repository.Instance.Proteus.SelectNode(_nodeData.Id);
        }
    }

    /// <summary>
    /// Opens the behavioral nodes related to the node in a new viewer when the user alt clicks the node.
    /// </summary>
    /// <param name="hit">Raycat result from the interaction.</param>
    public void OnPointerAltClickDown(RaycastHit hit)
    {
        List<PTNode> relatedBehavioralNodes = Repository.Instance.Models.GetRelatedBehavioralNodesById(_nodeData.Id);
        List<string> rootIds = new();

        foreach (PTNode node in relatedBehavioralNodes)
        {
            rootIds.Add(node.Id);
        }

        // Get position right of current viewer
        Vector3 startPosition = (Vector3)_attachedViewerData.Position;
        startPosition += new Vector3(0, ((Vector3)(_attachedViewerData.Scale)).y, 0);

        PTViewer behaviorViewer = new() { Id = Helpers.GenerateUniqueId(), RootNodeIds = rootIds.ToArray(), Position = startPosition, Scale = _attachedViewerData.Scale };
        Repository.Instance.Viewers.CreateViewer(behaviorViewer);
    }

    /// <summary>
    /// Links the event listeners to be notified of changes to viewer, node or the global Proteus data.
    /// </summary>
    private void LinkEventListeners()
    {
        if (_nodeData != null)
        {
            _nodeData.PropertyChanged += OnNodeDataChanged;
        }

        if (_attachedViewerData != null)
        {
            _attachedViewerData.PropertyChanged += OnViewerDataChanged;
        }

        if (_globalsData != null)
        {
            _globalsData.PropertyChanged += OnGlobalsDataChanged;
        }
    }

    /// <summary>
    /// Whenever the  global Proteus data's node selection changes, the node change's its color to reflect if it is selected.
    /// </summary>
    /// <param name="obj">The object containing the globals data.</param>
    /// <param name="e">Object storing the arguments of the property changed event.</param>
    private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SelectedNodes")
        {
            // Change text color based whether node is selected
            if (_globalsData.SelectedNodes.Contains(_nodeData.Id) && _displayNameTMP != null)
            {
                _displayNameTMP.color = Color.blue;
            }
            else
            {
                _displayNameTMP.color = Color.black;
            }
        }
    }

    /// <summary>
    /// Whenever the node's data changes the presentation is updated.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnNodeDataChanged(object obj, PropertyChangedEventArgs e)
    {
        UpdateNodePresentation();
    }

    /// <summary>
    /// Whenever the viewer's properties that affect the node, such as layout, zoom or scale, change the node's presentation is updated.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnViewerDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "LayoutNodes" || e.PropertyName == "ZoomScale" || e.PropertyName == "Scale")
        {
            UpdateNodePresentation();
        }
    }

    /// <summary>
    /// Updates the visual presentation of the node.
    /// </summary>
    private void UpdateNodePresentation()
    {
        // Update the position
        Vector3 nodeViewerPosition = new Vector3(0, 0, 0);
        var layoutNodes = _attachedViewerData?.LayoutNodes;

        if (layoutNodes != null)
        {
            if ((bool)(layoutNodes?.TryGetValue(_nodeData.Id, out nodeViewerPosition)))
            {
                //Debug.Log($"Updating local position {_nodeData.Id} {nodeViewerPosition.x} {nodeViewerPosition.y}");
                transform.SetLocalPositionAndRotation(nodeViewerPosition, transform.localRotation);
            }
        }

        // Set Display name
        if (_nodeData?.DisplayName != null && _displayNameTMP != null)
        {
            _displayNameTMP.SetText(_nodeData.DisplayName);
        }

        // Set the image texture
        if (_nodeGameObject != null)
        {
            Vector3 viewerScale = new Vector3(0, 0, 0);
            Vector3 zoomScale = new Vector3(0, 0, 0);
            
        
            float triggerPercentageOfNodeInView = _globalsData.DefaultNodeInViewTriggerPercentage;

            if (_attachedViewerData?.Scale != null && _attachedViewerData?.ZoomScale != null)
            {
                viewerScale = (Vector3)_attachedViewerData.Scale;
                zoomScale = (Vector3)_attachedViewerData.ZoomScale;
            }

            // Calculate the percentage of the node that is in viewer with respect to the viewer
            if (zoomScale.y <= (viewerScale.y / _nodeData.UnitHeight) * triggerPercentageOfNodeInView)
            {
                // Remove diagram texture
                _nodeGameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", null);

                // Place label in center
                _displayNameTMP.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
                _displayNameTMP.gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, 0, _nodeData.UnitDepth), _displayNameTMP.gameObject.transform.localRotation);
            }
            else
            {
                // Place label on top
                _displayNameTMP.gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, _nodeData.UnitHeight, _nodeData.UnitDepth), _displayNameTMP.gameObject.transform.localRotation);
                _displayNameTMP.verticalAlignment = TMPro.VerticalAlignmentOptions.Bottom;

                if (_nodeData?.ImageTexture != null)
                {
                    //TODO: Fix that texture maintains aspect ratio ratio
                    _nodeGameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", _nodeData.ImageTexture);
                }
            }

            // Update the local scale
            _nodeGameObject.transform.localScale = new Vector3(_nodeData.UnitWidth, _nodeData.UnitHeight, 1);
        }

    }
}
