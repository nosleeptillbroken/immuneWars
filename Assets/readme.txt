Tips:

If your asset needs to be loaded via. a script, place it inside a resources folder inside one of the asset folders, then use Resources.Load("AssetName") as AssetType.

Assets placed in resources are loaded at runtime regardless of whether they're being used or not. So if they don't need to be loaded, don't place them in resources.

Scripts being used by the unity editor should be placed inside the Scripts/Editor folder so they appear in a seperate solution in Visual Studio.

Place test scenes in Scenes/Experimental, and make sure you uniquely name them to avoid merge conflicts in the future. Only scenes used in the current release build should go in Scenes/

