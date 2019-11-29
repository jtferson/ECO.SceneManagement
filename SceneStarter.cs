#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace ECO.SceneManagement
{
    public class SceneStarter : MonoBehaviour
    {
        public bool IsKernel;
        public bool HasSetup;
#if ODIN_INSPECTOR
        [ShowIf("HasSetup")]
#endif
        public SceneSetup SceneSetup;

        public List<SceneReference> SceneDependsOn;
        protected void Awake()
        {
            if(IsKernel)
            {
                ProcessorScene.Initialize();
                ProcessorScene.PrepareKernel(this);
            }
            ProcessorScene.AddToLoading(SceneDependsOn, this);
        }
    }
}
