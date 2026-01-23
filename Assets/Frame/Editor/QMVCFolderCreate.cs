using System.IO;
using UnityEditor;

public class QMVCFolderCreate
{
	static string[] folders =
	{
		"Scripts/System",
		"Scripts/Utility",
		"Scripts/Controller",
		"Scripts/View",
		"Scripts/Model",
		"Scripts/Entity",
		"Scripts/Qurey",
		"Scripts/Command",
		"Scripts/Event"
	};

	[MenuItem("Assets/Create/QMVC/Folders", false, 0)]
	private static void CreateFolder() {
		string rootPath = Path.Combine(GetSelectedPath(), "Folder");
		Directory.CreateDirectory(rootPath);

		AssetDatabase.Refresh();

		foreach (var folder in folders)
		{
			string path = Path.Combine(rootPath, folder);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		AssetDatabase.Refresh();
	}

	private static string GetSelectedPath()
	{
		string path = "Assets";

		if (Selection.activeObject != null)
		{
			path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (File.Exists(path))
			{
				path = Path.GetDirectoryName(path);
			}
		}

		return path;
	}
}
