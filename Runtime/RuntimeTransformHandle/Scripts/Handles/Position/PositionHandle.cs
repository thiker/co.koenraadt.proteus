using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class PositionHandle : MonoBehaviour
    {
        //PROTEUS: ROOT FOR HANDLE
        private GameObject _gizmoRoot;

        protected RuntimeTransformHandle _parentTransformHandle;
        protected List<PositionAxis> _axes;
        protected List<PositionPlane> _planes;

        public PositionHandle Initialize(RuntimeTransformHandle p_runtimeHandle)
        {
            _parentTransformHandle = p_runtimeHandle;
            _gizmoRoot = _parentTransformHandle.transform.Find("GizmoRoot").gameObject;
            transform.SetParent(_gizmoRoot.transform, false);

            _axes = new List<PositionAxis>();

            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ) {
                GameObject o = new GameObject();
                _axes.Add(o.AddComponent<PositionAxis>().Initialize(_parentTransformHandle, Vector3.right, Color.red));
                o.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                o.transform.SetParent(_gizmoRoot.transform);
            }
            
            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ) {
                GameObject o = new GameObject();
                _axes.Add(o.AddComponent<PositionAxis>().Initialize(_parentTransformHandle, Vector3.up, Color.green));
                o.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                o.transform.SetParent(_gizmoRoot.transform);
            }

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ) {
                GameObject o = new GameObject();
                _axes.Add(o.AddComponent<PositionAxis>().Initialize(_parentTransformHandle, Vector3.forward, Color.blue));
                o.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                o.transform.SetParent(_gizmoRoot.transform);
            }

            _planes = new List<PositionPlane>();
            
            if (_parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XYZ) {
                GameObject o = new GameObject();
                _planes.Add(o.AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.right, Vector3.up, -Vector3.forward, new Color(0,0,1,.2f)));
                o.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                o.transform.SetParent(_gizmoRoot.transform);
            }

            if (_parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ) {
                GameObject o = new GameObject();
                _planes.Add(o.AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.up, Vector3.forward, Vector3.right, new Color(1, 0, 0, .2f)));
                o.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                o.transform.SetParent(_gizmoRoot.transform);
            }
            if (_parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ) {
                GameObject o = new GameObject();
                _planes.Add(o.AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.right, Vector3.forward, Vector3.up, new Color(0, 1, 0, .2f)));
                o.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                o.transform.SetParent(_gizmoRoot.transform);
            }

            return this;
        }

        public void Destroy()
        {
            foreach (PositionAxis axis in _axes)
                Destroy(axis.gameObject);
            
            foreach (PositionPlane plane in _planes)
                Destroy(plane.gameObject);
            
            Destroy(this);
        }
    }
}