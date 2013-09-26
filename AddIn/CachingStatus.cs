using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    public enum CachingStatus
    {
        /// <summary>
        /// Assembly has no assigned location (e.g. when it's new and it's being imported).
        /// </summary>
        None = 0,

        /// <summary>
        /// Assembly is cached locally, and is the current version
        /// </summary>
        Latest = 1,

        /// <summary>
        /// Assembly is cached locally, but it's an old version, a newer exists in the server.
        /// </summary>
        CachedOld = 2,

        /// <summary>
        /// Assembly is only in the asset store (server), not cached locally.
        /// </summary>
        Server = 3
    }
}
