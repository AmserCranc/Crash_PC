using System.IO;

unsafe public class LevelFiles
{
    public string displayname;
    public string filename;

    public NSD directory;
    public NSF levelFile;

    public LevelFiles(string streamPath, string fileID)
    {
        filename    = $"s00000{fileID}";
        displayname = NameLookup.Find(fileID);
        directory   = new($"{streamPath}/{fileID}");
        levelFile   = new($"{streamPath}/{fileID}");


    }   
}