using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.GameStudio.Assets
{

    public interface IResourceImporter<TResource>
    {

        bool TryImport(byte[] data, out TResource resource);

    }

}
