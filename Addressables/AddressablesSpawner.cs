using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStatics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace AddressableFunctions
{
    public class Handler
    {
        //Spawns by Instatianting. Doesn't Load Asset First. Use it for AssetReferences that spawn individually and not
        //consolidated as a group
        public static void SpawnAssetReference(AssetReference assetReference)
        {

            if (assetReference.RuntimeKeyIsValid() == false)
            {
                Debug.Log("Invalid Key " + assetReference.RuntimeKey.ToString());
                return;
            }


            assetReference.InstantiateAsync().Completed += (asyncOperationHandle) =>
            {
                AddNotifyOnDestroy(assetReference,asyncOperationHandle.Result);
            };
        }

        
        //Applies Func to the instantiated Result object
        public static void SpawnAssetReference(AssetReference assetReference, Action<GameObject> Func)
        {

            if (assetReference.RuntimeKeyIsValid() == false)
            {
                Debug.Log("Invalid Key " + assetReference.RuntimeKey.ToString());
                return;
            }


            assetReference.InstantiateAsync().Completed += (asyncOperationHandle) =>
            {
                AddNotifyOnDestroy(assetReference,asyncOperationHandle.Result);
                Func(asyncOperationHandle.Result);
            };
        }
        public static void AddNotifyOnDestroy(AssetReference assetReference,GameObject GO)
        {
            var notify = GO.AddComponent<NotifyOnDestroy>();
            notify.Destroyed += Remove;
            notify.AssetReference = assetReference;
        }

        //------------------------

        public static async Task LoadAndAssociateResultWithKey(IList<string> keys,Addressables.MergeMode MergeMode=Addressables.MergeMode.Union)
        {
            Dictionary<string, AsyncOperationHandle<GameObject>> operationDictionary = new Dictionary<string, AsyncOperationHandle<GameObject>>();
            await LoadKeys(keys,operationDictionary,MergeMode);
            SpawnOperations(operationDictionary);
        }

        /*
            Gets Assets From a list of Keys with a specific mergemode defaulted to Union.
            Spawns them After they are loaded.
        */
        public static async Task LoadAndAssociateResultWithKey(IList<string> keys,Action<GameObject> Func,Addressables.MergeMode MergeMode=Addressables.MergeMode.Union)
        {
            Dictionary<string, AsyncOperationHandle<GameObject>> operationDictionary = new Dictionary<string, AsyncOperationHandle<GameObject>>();
            await LoadKeys(keys,operationDictionary,MergeMode);
            SpawnOperations(operationDictionary,Func);
        }

        public static async Task< Dictionary<string, AsyncOperationHandle<GameObject>> > GetLoadAndAssociateResultWithKey(IList<string> keys,Action<GameObject> Func,Addressables.MergeMode MergeMode=Addressables.MergeMode.Union)
        {
            Dictionary<string, AsyncOperationHandle<GameObject>> operationDictionary = new Dictionary<string, AsyncOperationHandle<GameObject>>();
            await LoadKeys(keys,operationDictionary,MergeMode);
            return operationDictionary;
        }
        /*
        public static async Task LoadAndAssociateResultWithKey(IList<string> keys,Action callback,Addressables.MergeMode MergeMode=Addressables.MergeMode.Union)
        {
            await LoadAndAssociateResultWithKey(keys,MergeMode);
            callback?.Invoke();
        }
        public static async Task LoadAndAssociateResultWithKey(IList<string> keys,Action callback,Action<GameObject> Func,Addressables.MergeMode MergeMode=Addressables.MergeMode.Union)
        {
            await LoadAndAssociateResultWithKey(keys,Func,MergeMode);
            callback?.Invoke();
        }
        */

        public static async Task LoadKeys(IList<string> keys,Dictionary<string, AsyncOperationHandle<GameObject>> operationDictionary,
         Addressables.MergeMode MergeMode = Addressables.MergeMode.Union)
        {

            var locations = await Addressables.LoadResourceLocationsAsync(keys, MergeMode, typeof(GameObject)).Task;

            var loadOps = new List<AsyncOperationHandle>(locations.Count);

            foreach (IResourceLocation location in locations)
            {
                AsyncOperationHandle<GameObject> handle =
                    Addressables.LoadAssetAsync<GameObject>(location);
                handle.Completed += obj => operationDictionary.Add(location.PrimaryKey, obj);
                loadOps.Add(handle);
            }

            await Addressables.ResourceManager.CreateGenericGroupOperation(loadOps, true).Task;
        }

        public static void SpawnOperations(Dictionary<string, AsyncOperationHandle<GameObject>> operationDictionary)
        {
            foreach (var item in operationDictionary)
            {
                var go = StaticFunctions.Instantiate(item.Value.Result);
                var notify = go.AddComponent<NotifyOnDestroyNonAssetReference>();
                notify.OperationHandle = item.Value;
                notify.Destroyed += AddressableFunctions.Handler.ReleaseHandle;
            }
        }

        public static void SpawnOperations(Dictionary<string, AsyncOperationHandle<GameObject>> operationDictionary,Action<GameObject> Func)
        {
            foreach (var item in operationDictionary)
            {
                var go = StaticFunctions.Instantiate(item.Value.Result);
                var notify = go.AddComponent<NotifyOnDestroyNonAssetReference>();
                notify.OperationHandle = item.Value;
                notify.Destroyed += AddressableFunctions.Handler.ReleaseHandle;
                Func(go);
            }
        }

        public static void Remove(AssetReference assetReference, NotifyOnDestroy obj)
        {
            Addressables.ReleaseInstance(obj.gameObject);
            Resources.UnloadUnusedAssets();
        }

        public static void RemoveNonAssetRef(NotifyOnDestroyNonAssetReference obj)
        {
            Addressables.ReleaseInstance(obj.gameObject);
            Resources.UnloadUnusedAssets();
        }
        public static void ReleaseHandle(NotifyOnDestroyNonAssetReference obj)
        {
            Addressables.Release(obj.OperationHandle);
            Resources.UnloadUnusedAssets();
        }
    }
}


/*
https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#loading-an-assetreference

--Instantiating objects from Addressables---

You can load an asset, such as a Prefab, and then create an instance of it with Instantiate. You can also load and create an instance of an asset with Addressables.InstantiateAsync. 
The primary difference between these two ways of instantiating objects is how the asset reference counts are affected.

When you use InstantiateAsync, the reference counts of the loaded assets are incremented each time you call the method. 
Thus if you instantiate a Prefab five times, the reference count for the Prefab asset and any of its dependencies are incremented by five.
 You can then release each instance separately as they are destroyed in the game.

When you use LoadAssetAsync and Object.Instantiate, then the asset reference counts are only incremented once, for the initial load. 
If you release the loaded asset (or its operation handle) and the reference count drops to zero, 
then the asset is unloaded and all the additional instantiated copies lose their sub-assets as well -- 
they still exist as GameObjects in the scene, but without Meshes, Materials, or other assets that they depend on.

Which scenario is better, depends on how you organize your object code. For example, 
if you have a single manager object that supplies a pool of Prefab enemies to spawn into a game level, 
it might be most convenient to release them all at the completion of the level with a single operation 
handle stored in the manager class. In other situations, you might want to instantiate and release assets individually.
*/
/*
private static readonly Dictionary<AssetReference, List<GameObject>> _spawnedParticleSystems =
            new Dictionary<AssetReference, List<GameObject>>();

        /// The Queue holds requests to spawn an instanced that were made while we are already loading the asset
        /// They are spawned once the addressable is loaded, in the order requested
        private static readonly Dictionary<AssetReference, Queue<Vector3>> _queuedSpawnRequests =
            new Dictionary<AssetReference, Queue<Vector3>>();

        private static readonly Dictionary<AssetReference, AsyncOperationHandle<GameObject>> _asyncOperationHandles =
            new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();

        public static void Spawn(AssetReference assetReference)
        {


            if (assetReference.RuntimeKeyIsValid() == false)
            {
                Debug.Log("Invalid Key " + assetReference.RuntimeKey.ToString());
                return;
            }

            if (_asyncOperationHandles.ContainsKey(assetReference))
            {
                if (_asyncOperationHandles[assetReference].IsDone)
                    SpawnParticleFromLoadedReference(assetReference, GetRandomPosition());
                else
                    EnqueueSpawnForAfterInitialization(assetReference);

                return;
            }

            LoadAndSpawn(assetReference);
        }

        private static void LoadAndSpawn(AssetReference assetReference)
        {
            var op = Addressables.LoadAssetAsync<GameObject>(assetReference);
            _asyncOperationHandles[assetReference] = op;
            op.Completed += (operation) =>
            {
                SpawnParticleFromLoadedReference(assetReference, GetRandomPosition());
                if (_queuedSpawnRequests.ContainsKey(assetReference))
                {
                    while (_queuedSpawnRequests[assetReference]?.Any() == true)
                    {
                        var position = _queuedSpawnRequests[assetReference].Dequeue();
                        SpawnParticleFromLoadedReference(assetReference, position);
                    }
                }
            };
        }

        private static void EnqueueSpawnForAfterInitialization(AssetReference assetReference)
        {
            if (_queuedSpawnRequests.ContainsKey(assetReference) == false)
                _queuedSpawnRequests[assetReference] = new Queue<Vector3>();
            _queuedSpawnRequests[assetReference].Enqueue(GetRandomPosition());
        }

        private static void SpawnParticleFromLoadedReference(AssetReference assetReference, Vector3 position)
        {
            assetReference.InstantiateAsync(position, Quaternion.identity).Completed += (asyncOperationHandle) =>
            {
                if (_spawnedParticleSystems.ContainsKey(assetReference) == false)
                {
                    _spawnedParticleSystems[assetReference] = new List<GameObject>();
                }

                _spawnedParticleSystems[assetReference].Add(asyncOperationHandle.Result);
                var notify = asyncOperationHandle.Result.AddComponent<NotifyOnDestroy>();
                notify.Destroyed += Remove;
                notify.AssetReference = assetReference;
            };
        }

        private static Vector3 GetRandomPosition()
        {
            return new Vector3(UnityEngine.Random.Range(-5, 5), 1, UnityEngine.Random.Range(-5, 5));
        }

        private static void Remove(AssetReference assetReference, NotifyOnDestroy obj)
        {
            Addressables.ReleaseInstance(obj.gameObject);

            _spawnedParticleSystems[assetReference].Remove(obj.gameObject);
            if (_spawnedParticleSystems[assetReference].Count == 0)
            {
                Debug.Log($"Removed all {assetReference.RuntimeKey.ToString()}");

                if (_asyncOperationHandles[assetReference].IsValid())
                    Addressables.Release(_asyncOperationHandles[assetReference]);

                _asyncOperationHandles.Remove(assetReference);
            }
        }
    }
    */