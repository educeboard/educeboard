using UnityEngine;
using UnityEditor;

public class ExportAssetBundles
{
	[
	 MenuItem("Assets/Build AssetBundle")]
	static void ExportResource ()
	{
		// 保存ウィンドウのパネルを表示
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		if (path.Length != 0) {
			
			BuildPipeline.BuildAssetBundles
			(
				path,
				BuildAssetBundleOptions.None,
				BuildTarget.Android
			);
			
			// for iPhone
			BuildPipeline.BuildAssetBundles
			(
				path,
				BuildAssetBundleOptions.None,
				BuildTarget.iOS
			);
			
			// for WebGL
			BuildPipeline.BuildAssetBundles
			(
				path,
				BuildAssetBundleOptions.None,
				BuildTarget.WebGL
			);

			// for MacOSX
			BuildPipeline.BuildAssetBundles
			(
				path,
				BuildAssetBundleOptions.None,
				BuildTarget.StandaloneLinux
			);
		}
	}
	
//	[
//	 MenuItem("Assets/Build AssetBundle - No dependency tracking")]
//	static void ExportResourceNoTrack ()
//	{
//		// 保存ウィンドウのパネルを表示
//		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
//		if (path.Length != 0) {
//			BuildPipeline.BuildAssetBundles
//			(
//				path,
//				BuildAssetBundleOptions.None,
//				BuildTarget.NoTarget
//			);
//		}
//	}
}
