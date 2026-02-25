using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace VFX
{
    [ExecuteAlways]
    public class TargetPosition : MonoBehaviour
    {
        public Transform target;

        private int shaderPropertyID_Current;
        private int shaderPropertyID_Past;

        private Queue<(float time, Vector3 position)> positionHistory = new Queue<(float, Vector3)>();

        void Start()
        {
            shaderPropertyID_Current = Shader.PropertyToID("_TargetTurbulencePose");
            shaderPropertyID_Past = Shader.PropertyToID("_TargetTurbulencePose2");

#if UNITY_EDITOR
            EditorApplication.update += UpdateInEditor;
#endif
        }

        void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= UpdateInEditor;
#endif
        }

        void FixedUpdate()
        {
            if (Application.isPlaying)
            {
                // Use Time.realtimeSinceStartup at runtime instead of Application.timeSinceStartup
                UpdateShader(Time.realtimeSinceStartup);
            }
        }

#if UNITY_EDITOR
        void UpdateInEditor()
        {
            if (!Application.isPlaying)
            {
                UpdateShader((float)EditorApplication.timeSinceStartup);
            }
        }
#endif

        void UpdateShader(float currentTime)
        {
            if (target == null) return;

            Vector3 currentPosition = target.position;
            positionHistory.Enqueue((currentTime, currentPosition));

            Vector3 pastPosition = currentPosition; // fallback if none found

            while (positionHistory.Count > 0)
            {
                var (time, position) = positionHistory.Peek();

                float age = currentTime - time;

                if (age > 0.2f)
                {
                    // Too old: remove from queue
                    positionHistory.Dequeue();
                }
                else
                {
                    pastPosition = position;
                    break;
                }
            }

            // Shader.SetGlobalVector expects a Vector4, pass explicit Vector4
            Shader.SetGlobalVector(shaderPropertyID_Current, new Vector4(currentPosition.x, currentPosition.y, currentPosition.z, 0f));
            Shader.SetGlobalVector(shaderPropertyID_Past, new Vector4(pastPosition.x, pastPosition.y, pastPosition.z, 0f));
        }
    }
}