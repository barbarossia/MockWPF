// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationType.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MockWPF.Common
{
    /// <summary>
    /// The local location type of a referenced assembly.
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// The new. User provided it just now.
        /// </summary>
        New,

        /// <summary>
        /// The cached. Assembly is alread in local caching folder.
        /// </summary>
        Cached,

        /// <summary>
        /// The none. Assembly has no location yet.
        /// </summary>
        None,
    }
}