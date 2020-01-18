using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dossamer.PanZoom
{
    [RequireComponent(typeof(PanZoomBehavior))]
    public class PanZoomTargetClickHandler : MonoBehaviour {

        PanZoomBehavior panZoom;

        private void Start()
        {
            panZoom = GetComponent<PanZoomBehavior>();
        }

        void OnEnable()
        {
            EventOnClickBehavior.handler += ReassignTarget;
        }

        private void OnDestroy()
        {
            EventOnClickBehavior.handler -= ReassignTarget;
        }

        void ReassignTarget(GameObject target)
        {
            panZoom.ReassignTarget(target);
        }
    }
}