using System.Collections.Generic;

namespace LanCopyFiles.Services.StorageServices.FilePrepare;

public interface ISendingTempFolder
{
    public void Add(string sourceThingPath);

    public void AddMany(string[] sourceThingPaths);

    public void Delete(string fileName);


    public string Get(string fileName);

    public IEnumerable<string> GetMany(string[] fileNames);

    public IEnumerable<string> GetAll();
}