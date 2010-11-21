﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatorShare {
  /// <summary>
  /// The exception that is thrown when the service GatorShare depends on is not
  /// functioning correctly.
  /// </summary>
  /// <remarks>It usually means an unrecoverable error which will cause 
  /// operation to fail.</remarks>
  public class DependencyException : Exception {
    public DependencyException() : base() { }
    public DependencyException(string message) : base(message) { }
    public DependencyException(string message, Exception innerException) :
      base(message, innerException) { }
  }
}
