//
// RedirectFS-FH.cs: Port of
// http://fuse.cvs.sourceforge.net/fuse/fuse/example/fusexmp_fh.c?view=log
//
// Authors:
//   Jonathan Pryor (jonpryor@vt.edu)
//
// (C) 2006 Jonathan Pryor
//
// Mono.Fuse example program
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

/**
 * Copyright (C) 2007 Jiangyan Xu <jiangyan@ufl.edu> University of Florida
 * This file has been modified from the example file in Mono.Fuse 
 * <http://www.jprl.com/Projects/mono-fuse.html>
 * to serve as an utility class for RedirectFS operations intead of listening
 * to the system events directly
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Fuse;
using Mono.Unix.Native;

namespace GSeries.Filesystem {
  public class RedirectFHFSHelper {

    private string basedir;

    public RedirectFHFSHelper(string basedir)
    {
      this.basedir = basedir;
    }

    public virtual Errno GetPathStatus (string path, out Stat buf)
    {
      int r = Syscall.lstat (basedir+path, out buf);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno GetHandleStatus (string path, OpenedPathInfo info, out Stat buf)
    {
      int r = Syscall.fstat ((int) info.Handle, out buf);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno AccessPath (string path, AccessModes mask)
    {
      int r = Syscall.access (basedir+path, mask);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno ReadSymbolicLink (string path, out string target)
    {
      target = null;
      StringBuilder buf = new StringBuilder (256);
      do {
        int r = Syscall.readlink (basedir+path, buf);
        if (r < 0) {
          return Stdlib.GetLastError ();
        }
        else if (r == buf.Capacity) {
          buf.Capacity *= 2;
        }
        else {
          target = buf.ToString (0, r);
          return 0;
        }
      } while (true);
    }

    public virtual Errno OpenDirectory (string path, OpenedPathInfo info)
    {
      IntPtr dp = Syscall.opendir (basedir+path);
      if (dp == IntPtr.Zero)
        return Stdlib.GetLastError ();

      info.Handle = dp;
      return 0;
    }

    public virtual Errno ReadDirectory (string path, OpenedPathInfo fi,
        out IEnumerable<DirectoryEntry> paths)
    {
      IntPtr dp = (IntPtr) fi.Handle;

      paths = ReadDirectory (dp);

      return 0;
    }

    private IEnumerable<DirectoryEntry> ReadDirectory (IntPtr dp)
    {
      Dirent de;
      while ((de = Syscall.readdir (dp)) != null) {
        DirectoryEntry e = new DirectoryEntry (de.d_name);
        e.Stat.st_ino  = de.d_ino;
        e.Stat.st_mode = (FilePermissions) (de.d_type << 12);
        yield return e;
      }
    }

    public virtual Errno ReleaseDirectory (string path, OpenedPathInfo info)
    {
      IntPtr dp = (IntPtr) info.Handle;
      Syscall.closedir (dp);
      return 0;
    }

    public virtual Errno CreateSpecialFile (string path, FilePermissions mode, ulong rdev)
    {
      int r;

      // On Linux, this could just be `mknod(basedir+path, mode, rdev)' but 
      // this is more portable.
      if ((mode & FilePermissions.S_IFMT) == FilePermissions.S_IFREG) {
        r = Syscall.open (basedir+path, OpenFlags.O_CREAT | OpenFlags.O_EXCL |
            OpenFlags.O_WRONLY, mode);
        if (r >= 0)
          r = Syscall.close (r);
      }
      else if ((mode & FilePermissions.S_IFMT) == FilePermissions.S_IFIFO) {
        r = Syscall.mkfifo (basedir+path, mode);
      }
      else {
        r = Syscall.mknod (basedir+path, mode, rdev);
      }

      if (r == -1)
        return Stdlib.GetLastError ();

      return 0;
    }

    public virtual Errno CreateDirectory (string path, FilePermissions mode)
    {
      int r = Syscall.mkdir (basedir+path, mode);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno RemoveFile (string path)
    {
      int r = Syscall.unlink (basedir+path);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno RemoveDirectory (string path)
    {
      int r = Syscall.rmdir (basedir+path);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno CreateSymbolicLink (string from, string to)
    {
      int r = Syscall.symlink (from, basedir+to);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno RenamePath (string from, string to)
    {
      int r = Syscall.rename (basedir+from, basedir+to);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno CreateHardLink (string from, string to)
    {
      int r = Syscall.link (basedir+from, basedir+to);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno ChangePathPermissions (string path, FilePermissions mode)
    {
      int r = Syscall.chmod (basedir+path, mode);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno ChangePathOwner (string path, long uid, long gid)
    {
      int r = Syscall.lchown (basedir+path, (uint) uid, (uint) gid);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno TruncateFile (string path, long size)
    {
      int r = Syscall.truncate (basedir+path, size);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno TruncateHandle (string path, OpenedPathInfo info, long size)
    {
      int r = Syscall.ftruncate ((int) info.Handle, size);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno ChangePathTimes (string path, ref Utimbuf buf)
    {
      int r = Syscall.utime (basedir+path, ref buf);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno CreateHandle (string path, OpenedPathInfo info, FilePermissions mode)
    {
      int fd = Syscall.open (basedir+path, info.OpenFlags, mode);
      if (fd == -1)
        return Stdlib.GetLastError ();
      info.Handle = (IntPtr) fd;
      return 0;
    }

    public virtual Errno OpenHandle (string path, OpenedPathInfo info)
    {
      int fd = Syscall.open (basedir+path, info.OpenFlags);
      if (fd == -1)
        return Stdlib.GetLastError ();
      info.Handle = (IntPtr) fd;
      return 0;
    }

    public virtual unsafe Errno ReadHandle (string path, OpenedPathInfo info, byte[] buf, 
        long offset, out int bytesRead)
    {
      int r;
      fixed (byte *pb = buf) {
        r = bytesRead = (int) Syscall.pread ((int) info.Handle, 
            pb, (ulong) buf.Length, offset);
      }
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual unsafe Errno WriteHandle (string path, OpenedPathInfo info,
        byte[] buf, long offset, out int bytesWritten)
    {
      int r;
      fixed (byte *pb = buf) {
        r = bytesWritten = (int) Syscall.pwrite ((int) info.Handle, 
            pb, (ulong) buf.Length, offset);
      }
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno GetFileSystemStatus (string path, out Statvfs stbuf)
    {
      int r = Syscall.statvfs (basedir+path, out stbuf);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno FlushHandle (string path, OpenedPathInfo info)
    {
      /* This is called from every close on an open file, so call the
         close on the underlying filesystem.  But since flush may be
         called multiple times for an open file, this must not really
         close the file.  This is important if used on a network
         filesystem like NFS which flush the data/metadata on close() */
      int r = Syscall.close (Syscall.dup ((int) info.Handle));
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno ReleaseHandle (string path, OpenedPathInfo info)
    {
      int r = Syscall.close ((int) info.Handle);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno SynchronizeHandle (string path, OpenedPathInfo info, bool onlyUserData)
    {
      int r;
      if (onlyUserData)
        r = Syscall.fdatasync ((int) info.Handle);
      else
        r = Syscall.fsync ((int) info.Handle);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno SetPathExtendedAttribute (string path, string name, byte[] value, XattrFlags flags)
    {
      int r = Syscall.lsetxattr (basedir+path, name, value, (ulong) value.Length, flags);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno GetPathExtendedAttribute (string path, string name, byte[] value, out int bytesWritten)
    {
      int r = bytesWritten = (int) Syscall.lgetxattr (basedir+path, name, value, (ulong) value.Length);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno ListPathExtendedAttributes (string path, out string[] names)
    {
      int r = (int) Syscall.llistxattr (basedir+path, out names);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }

    public virtual Errno RemovePathExtendedAttribute (string path, string name)
    {
      int r = Syscall.lremovexattr (basedir+path, name);
      if (r == -1)
        return Stdlib.GetLastError ();
      return 0;
    }
  }
}

