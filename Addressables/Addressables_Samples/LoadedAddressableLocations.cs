using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class LoadedAddressableLocations : MonoBehaviour
{
    [SerializeField] private string _label;

    public IList<IResourceLocation> AssetLocations { get; } = new List<IResourceLocation>();
    private List<GameObject> Assets { get; } = new List<GameObject>();

    public AssetReference assetReference;

    private IEnumerator Start()
    {
        //AddressableFunctions.AddressablesHandler.SpawnAssetReference(assetReference);
        //Task task = InitAndWaitUntilLocLoaded();

        //InitAndWaitUntilLocLoaded();
        //Addressables.LoadAssetsAsync<GameObject>(_label, null).Completed += objects =>
        //{
        //    foreach (var go in objects.Result)
        //    {
        //        Debug.Log($"Addressable Loaded: {go}");
        //        GameObject.Instantiate(go);
        //        //https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html
        //        
        //    }
        //};

        //while(!task.IsCompleted)
        //{
        //    yield return null;

        //Addressables.LoadResourceLocationsAsync(_label).Completed += (asyncOperationHandle) =>
        //{
        //    Debug.Log(asyncOperationHandle.Result.Count);
        //    foreach (var location in asyncOperationHandle.Result)
        //    {
        //        //ASSET LOCATION FULLY LOADED
        //        Debug.Log(location);
        //        //Addressables.InstantiateAsync(location).Completed+=FinishedInstantiating;                
        //    }            
        //};
        AsyncOperationHandle<IList<GameObject>> loadHandle = LoadandInstiate(); 


        //This Works but prefer loading inidividually.
        //AsyncOperationHandle<IList<GameObject>> loadHandle = LoadandInstiate(); 
        yield return new WaitForSeconds(10);
        Addressables.Release(loadHandle);
        Resources.UnloadUnusedAssets();

        //When Notify on Destroyed is called, it wil release memory. The above is needed or else it won't work.
        Debug.Log("Unloaded");
    }

    private AsyncOperationHandle<IList<GameObject>> LoadandInstiate()
    {
        AsyncOperationHandle<IList<GameObject>> loadHandle;
        loadHandle = Addressables.LoadAssetsAsync<GameObject>(_label,
            addressable =>
            {
                Debug.Log("Load");
                //Gets called for every loaded asset
                var go = Instantiate<GameObject>(addressable);
                var notify = go.AddComponent<NotifyOnDestroyNonAssetReference>();
                notify.Destroyed += AddressableFunctions.Handler.RemoveNonAssetRef;

            });
        return loadHandle;
    }

    //Want to do this but cannot work
    //private void InitAndWaitUntilLocLoaded()
    //{
    //    Addressables.LoadResourceLocationsAsync(_label).Completed += (asyncOperationHandle) =>
    //    {
    //        foreach (var location in asyncOperationHandle.Result)
    //        {
    //            //ASSET LOCATION FULLY LOADED
    //            Debug.Log(location.PrimaryKey);
    //            Addressables.InstantiateAsync(location).Completed+=FinishedInstantiating;
    //            
    //        }
    //        
    //    };
    //    
    //}

    private void FinishedInstantiating(AsyncOperationHandle<GameObject> obj)
    {
        var notify = obj.Result.AddComponent<NotifyOnDestroyNonAssetReference>();
        notify.Destroyed += AddressableFunctions.Handler.RemoveNonAssetRef;
    }



    //private async Task InitAndWaitUntilLocLoaded()
    //{
    //    await AddressableLocationLoader.GetAll(_label, AssetLocations);
    //    await CreateAddressablesLoader.InitByLoadedAddress(AssetLocations, Assets);
    //
    //    foreach (var location in AssetLocations)
    //    {
    //        //ASSET LOCATION FULLY LOADED
    //        Debug.Log(location.PrimaryKey);
    //    }
    //}
}
