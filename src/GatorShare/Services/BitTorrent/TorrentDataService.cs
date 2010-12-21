﻿/*
Copyright (c) 2010 Jiangyan Xu <jiangyan@ufl.edu>, University of Florida

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GatorShare.Services.BitTorrent {
  /// <summary>
  /// Service for torrent related information.
  /// </summary>
  public class TorrentDataService {
    readonly BitTorrentCache _bittorrentCache;

    public TorrentDataService(BitTorrentCache bittorrentCache) {
      _bittorrentCache = bittorrentCache;
    }

    /// <summary>
    /// Loads the torrent file.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="name">The name.</param>
    /// <param name="fileName">Name of the file. Usually {name}.torrent</param>
    /// <returns>The file stream.</returns>
    public Stream LoadTorrentFile(string nameSpace, string name, out string fileName) {
      string path = _bittorrentCache.GetTorrentFilePath(nameSpace, name);
      fileName = Path.GetFileName(path);
      return File.OpenRead(path);
    }
  }
}