using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace VRCholdstickpen
{
    public class UdonPenLine : UdonSharpBehaviour
    {
        [SerializeField] private float simplification = .005f;
        [UdonSynced] public Vector3[] points;

        [UdonSynced, FieldChangeCallback(nameof(SyncedColor))]
        private Color _syncedColor;

        public Color SyncedColor
        {
            set
            {
                _syncedColor = value;
                lineRenderer.material.color = value;
            }
            get => _syncedColor;
        }


        private LineRenderer lineRenderer;
        private MeshCollider lineCollider;
        private Mesh lineMesh;
        private bool isDown;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (!Utilities.IsValid(lineRenderer))
            {
                Debug.LogError($"UdonPenLine: LineRenderer not found for {name}");
                return;
            }

            lineCollider = GetComponent<MeshCollider>();
            if (!Utilities.IsValid(lineCollider))
            {
                Debug.LogError($"UdonPenLine: MeshCollider not found for {name}");
                return;
            }

            lineMesh = new Mesh();
        }

        public void OnUpdate()
        {
            var tempPoints = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(tempPoints);
            lineRenderer.enabled = true;
            points = tempPoints;
            RequestSerialization();
        }

        public void OnFinish()
        {
            lineRenderer.Simplify(simplification);
            OnUpdate();
            BakeMesh();
        }

        public void BakeMesh()
        {
            lineRenderer.BakeMesh(lineMesh, true);
            lineMesh.Optimize();
            lineMesh.RecalculateNormals();
            lineMesh.RecalculateBounds();
            lineCollider.sharedMesh = lineMesh;
        }

        public override void OnDeserialization(DeserializationResult result)
        {
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
            lineRenderer.enabled = true;
            BakeMesh();
        }

        public void Erase()
        {
            if (Networking.IsOwner(gameObject))
            {
                lineRenderer.positionCount = 0;
                OnUpdate();
            }
        }
    }
}
