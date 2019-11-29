using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ECO.SceneManagement
{
    internal static class ProcessorScene
    {
        //public static ProcessorScene Default => _default = _default ?? new ProcessorScene();
        //private static ProcessorScene _default;

        static Queue<string> _sceneDependsOn = new Queue<string>();
        static HashSet<string> _scenesDependsOnAvailable = new HashSet<string>();
        static Queue<SceneSetup> _sceneRegisteredSetups = new Queue<SceneSetup>();
        static Queue<SceneSetup> _scenePostBuildSetups = new Queue<SceneSetup>();

        static SceneStarter _mainStarter;
        static SceneKernel _kernel;
        static Coroutine _coroutine;

        public static void Initialize()
        {
            _sceneDependsOn = new Queue<string>();
            _scenesDependsOnAvailable = new HashSet<string>();
            _sceneRegisteredSetups = new Queue<SceneSetup>();
            _scenePostBuildSetups = new Queue<SceneSetup>();

            _mainStarter = null;
            _kernel = null;
            _coroutine = null;
        }

        public static void PrepareKernel(SceneStarter mainStarter)
        {
            if (_kernel != null) throw new Exception("Kernel has already created. You can have only one Kernel per Domain");
            _kernel = new SceneKernel();
            _kernel.CreateBuilder();

            _mainStarter = mainStarter;
        }

        public static void AddToLoading(List<SceneReference> sceneDependsOn, SceneStarter starter)
        {
            if (_kernel == null) throw new Exception("There is no Kernel. You have to create Kernel before loading scenes");
            for (var i = 0; i < sceneDependsOn.Count; i++)
            {
                if (!_scenesDependsOnAvailable.Contains(sceneDependsOn[i].ScenePath))
                {
                    _sceneDependsOn.Enqueue(sceneDependsOn[i].ScenePath);
                    _scenesDependsOnAvailable.Add(sceneDependsOn[i].ScenePath);
                }
            }

            if(starter.HasSetup && starter.SceneSetup != null)
            {
                _sceneRegisteredSetups.Enqueue(starter.SceneSetup);
            }

            if (_coroutine == null)
            {
                _coroutine = _mainStarter.StartCoroutine(LoadScenes());
            }
        }

        private static IEnumerator LoadScenes()
        {
            while (_sceneDependsOn.Count > 0)
            {
                var sceneToLoad = _sceneDependsOn.Dequeue();
                var load = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
                while (!load.isDone)
                {
                    yield return 0;
                }

                yield return new WaitForSeconds(0.1f);
            }

            while(_sceneRegisteredSetups.Count > 0)
            {
                var sceneSetup = _sceneRegisteredSetups.Dequeue();
                sceneSetup.Prepare(_kernel);
                sceneSetup.RegisterDependencies();

                _scenePostBuildSetups.Enqueue(sceneSetup);
            }

            _kernel.Build();

            while (_scenePostBuildSetups.Count > 0)
            {
                var sceneSetup = _scenePostBuildSetups.Dequeue();
                sceneSetup.ExecuteSetup();
            }

            yield return null;
        }
    }
}

