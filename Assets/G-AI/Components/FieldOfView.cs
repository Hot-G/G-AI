using UnityEngine;

namespace Eadon.AI.Utilities
{
    public class FieldOfView : MonoBehaviour
    {
        [Tooltip("The length of the field of view cone in world units.")]
        public float length = 10f;

        [Range(1f, 180f), Tooltip("The width angle of the view cone in degrees.")]
        public float viewWidthAngle = 120f;

        [Range(1f, 180f), Tooltip("The height angle of the view cone in degrees.")]
        public float viewHeightAngle = 45f;

        private readonly Vector3[] _points = new Vector3[17];
        
        private Mesh _mesh;
        private MeshCollider _meshCollider;

        private void Awake()
        {
            _meshCollider = GetComponent<MeshCollider>();
            CreateCollider();
        }

        private void OnValidate()
        {
            _meshCollider = GetComponent<MeshCollider>();
            CreateCollider();
        }

        private void CreateCollider()
        {
            if (_meshCollider != null && _meshCollider.sharedMesh != null && _meshCollider.sharedMesh == _mesh)
            {
                DestroyImmediate(_meshCollider.sharedMesh, true);
            }
            
            _mesh = CreateMesh();

            _meshCollider.sharedMesh = _mesh;
            _mesh.name = "Field Of View";
            _meshCollider.convex = true;
            _meshCollider.isTrigger = true;
        }

        private Mesh CreateMesh()
        {
            CreatePoints();
            var triangles = new int[90];
            var m = new Mesh();

            // left side
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;
            triangles[6] = 0;
            triangles[7] = 3;
            triangles[8] = 4;

            // top side
            triangles[9] = 0;
            triangles[10] = 4;
            triangles[11] = 8;
            triangles[12] = 0;
            triangles[13] = 8;
            triangles[14] = 12;
            triangles[15] = 0;
            triangles[16] = 12;
            triangles[17] = 16;

            // right side
            triangles[18] = 0;
            triangles[19] = 16;
            triangles[20] = 15;
            triangles[21] = 0;
            triangles[22] = 15;
            triangles[23] = 14;
            triangles[24] = 0;
            triangles[25] = 14;
            triangles[26] = 13;

            // bottom side
            triangles[27] = 0;
            triangles[28] = 13;
            triangles[29] = 9;
            triangles[30] = 0;
            triangles[31] = 9;
            triangles[32] = 5;
            triangles[33] = 0;
            triangles[34] = 5;
            triangles[35] = 1;

            // front left column
            triangles[36] = 1;
            triangles[37] = 2;
            triangles[38] = 6;
                
            triangles[39] = 1;
            triangles[40] = 6;
            triangles[41] = 5;
            
            triangles[42] = 2;
            triangles[43] = 3;
            triangles[44] = 7;
                
            triangles[45] = 2;
            triangles[46] = 7;
            triangles[47] = 6;
            
            triangles[48] = 3;
            triangles[49] = 4;
            triangles[50] = 7;
                
            triangles[51] = 3;
            triangles[52] = 8;
            triangles[53] = 7;

            // center column
            triangles[54] = 5;
            triangles[55] = 6;
            triangles[56] = 10;
                
            triangles[57] = 5;
            triangles[58] = 10;
            triangles[59] = 9;
            
            triangles[60] = 6;
            triangles[61] = 7;
            triangles[62] = 11;
                
            triangles[63] = 6;
            triangles[64] = 11;
            triangles[65] = 10;
            
            triangles[66] = 7;
            triangles[67] = 8;
            triangles[68] = 12;
                
            triangles[69] = 7;
            triangles[70] = 12;
            triangles[71] = 11;

            // right column
            triangles[72] = 9;
            triangles[73] = 10;
            triangles[74] = 14;
                
            triangles[75] = 9;
            triangles[76] = 14;
            triangles[77] = 13;
            
            triangles[78] = 10;
            triangles[79] = 11;
            triangles[80] = 15;
                
            triangles[81] = 10;
            triangles[82] = 15;
            triangles[83] = 14;
            
            triangles[84] = 11;
            triangles[84] = 12;
            triangles[86] = 16;
                
            triangles[87] = 11;
            triangles[88] = 16;
            triangles[89] = 15;
            
            m.vertices = _points;
            m.triangles = triangles;

            return m;
        }

        private void CreatePoints()
        {
            _points[0] = Vector3.zero;

            var thetaSlice = viewWidthAngle / 6;
            var phiSlice = viewHeightAngle / 6;

            var startTheta = -3 * thetaSlice;
            var phiStart = -3 * phiSlice;
            
            var theta = startTheta;

            var counter = 1;
            
            for (var i = 0; i < 4; i++)
            {
                var phi = phiStart;

                for (var j = 0; j < 4; j++)
                {
                    _points[counter] = GetPoint(length, theta, phi);
                    phi += 2 * phiSlice;
                    counter++;
                }

                theta += 2 * thetaSlice;
            }
        }

        private Vector3 GetPoint(float r, float theta, float phi)
        {
            var result = Vector3.zero;

            var azimuth = theta * Mathf.Deg2Rad;
            var altitude = phi * Mathf.Deg2Rad;
            var c = Mathf.Cos(altitude);
            return new Vector3(r * Mathf.Sin(azimuth) * c, r * Mathf.Sin(altitude), r * Mathf.Cos(azimuth) * c);
        }
    }
}