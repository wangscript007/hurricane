﻿// Copyright (c) 2011 XU, Jiang Yan <me@jxu.me>, University of Florida
//
// This software may be used and distributed according to the terms of the
// MIT license: http://www.opensource.org/licenses/mit-license.php

namespace GSeries.ProvisionSupport {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The class that represents an entry in the <see cref="ChunkDbService"/> 
    /// that has the information
    /// </summary>
    public class DataChunk {
        public const int ChunkSize = (1 << 14); // 16kB
        public const int HashSize = 20;

        public virtual int Id { get; private set; }
        public virtual byte[] Hash { get; set; }
        public virtual ManagedFile File { get; set; }
        /// <summary>
        /// Gets or sets the index of the file.
        /// </summary>
        /// <value>
        /// The index of the chunk in the file. Can be negative.
        /// </value>
        /// <remarks>A negative value means it's the last chunk is the file.
        /// </remarks>
        public virtual int ChunkIndex { get; set; }
        public virtual int Count { get; set; }
    }
}
