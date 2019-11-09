using UnityEngine;

namespace ECO.SceneManagement
{
    public abstract class SceneSetup : MonoBehaviour
    {
        protected SceneKernel Kernel => _kernel;
        private SceneKernel _kernel;
        public void Prepare(SceneKernel kernel)
        {
            _kernel = kernel;
        }

        public abstract void RegisterDependencies();

        public virtual void ExecuteSetup() { }
    }
}
