using System.Collections.Generic;

namespace MonoCraft;

public interface IDebugRowProvider
{
    public IEnumerable<string> GetDebugRows();
}