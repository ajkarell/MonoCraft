using System.Collections.Generic;

public interface IDebugRowProvider
{
    public IEnumerable<string> GetDebugRows();
}