﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fushare.Services.BitTorrent {
  /// <summary>
  /// Defines contract of a BitTorrent Service.
  /// </summary>
  public interface IBitTorrentService {
    /// <summary>
    /// Gets a file or directory.
    /// </summary>
    /// <param name="nameSpace">namespace of the name</param>
    /// <param name="name">name</param>
    /// <returns>The full path to the file or directory already downloaded.</returns>
    DataMetaInfo Get(string nameSpace, string name);

    /// <summary>
    /// Gets the file or directory and save it to the specified path.
    /// </summary>
    /// <param name="nameSpace">The name space.</param>
    /// <param name="name">The name.</param>
    /// <param name="saveDirPath">The full path to save the file or directory.</param>
    void Get(string nameSpace, string name, string saveDirPath);

    /// <summary>
    /// Publishes a file or directory.
    /// </summary>
    /// <param name="nameSpace">The name space.</param>
    /// <param name="name">The name.</param>
    /// <remarks>Uses the name of the directory of file as the publishing name. 
    /// Used when you want to publish file/directory already in the Cache folder.
    /// </remarks>
    void Publish(string nameSpace, string name);
    /// <summary>
    /// Same as Publish except that you don't get an exception when the key is 
    /// duplicated. You overwrite the existing key.
    /// </summary>
    /// <param name="nameSpace">The name space.</param>
    /// <param name="name">The name.</param>
    void Update(string nameSpace, string name);

    /// <summary>
    /// Publishes a file or directory.
    /// </summary>
    /// <param name="path">The path to the file/directory to be published.
    /// </param>
    /// <remarks>Used when you have a specific path for the 
    /// file/directory.</remarks>
    void Publish(string path);
    void Update(string path);
  }
}