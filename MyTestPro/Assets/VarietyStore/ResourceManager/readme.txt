使用文档

1. Editor
在使用Editor中的脚本打包后，除了生成对应的AB包，还会生成一个文件，叫做AssetsTable。这个文件记录了AB包与单个文件的对应关系。



2. Runtime
使用ResourceManager来加载资源，其有两个构造函数：一个带AssetBundle路径，则构造出的实例读取的是AB包内的资源，另一个不带参数，读取的是原始资源。具体使用如下：

1） 异步读取AB包内的Prefab
var assetRootPath = Application.streamingAssetsPath + "/AssetBunldes";
ResourceManager resourceManager = new ResourceManager(assetRootPath);

var assetToLoad = "Assets/Resources/Prefabs/ST_Prop.prefab";
resourceManager.LoadAsync<GameObject>(assetToLoad, (GameObject obj) =>
{
    var prefab = GameObject.Instantiate(obj);
    prefab.transform.position = Vector3.zero;
});

2）异步读取AB包内的Scene文件
var assetRootPath = Application.streamingAssetsPath + "/AssetBunldes";
ResourceManager resourceManager = new ResourceManager(assetRootPath);

var sceneToLoad = "Assets/Resources/Scenes/Demo.unity";
resourceManager.LoadSceneAsync(sceneToLoad, () =>
{
    Debug.Log("Scene Loaded!");
});

3）异步读取原始文件时，除了assetRootPath不需要传入外，其他完全相同



3. 注意事项
1）所有导出的AB包，都存在名为AssetBunldes的文件夹下，也即是Manifest的文件名为AssetBunldes
2）资源(包含Scene)的路径是从Assets文件夹开始的全路径，包含文件扩展名
