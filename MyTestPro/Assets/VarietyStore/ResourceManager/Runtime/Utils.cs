using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Framework.Resource
{
    public static class Utils
    {
        public static string GetPathWithoutExtension(string full_name)
        {
            int last_idx = full_name.LastIndexOfAny(".".ToCharArray());
            if (last_idx < 0)
                return full_name;

            return full_name.Substring(0, last_idx);
        }

        public static string AbsoluteToRelativePath(string root_path, string absolute_path)
        {
            absolute_path = absolute_path.Replace('\\', '/');
            int last_idx = absolute_path.LastIndexOf(root_path);
            if (last_idx < 0)
                return absolute_path;

            int start = last_idx + root_path.Length;
            if (absolute_path[start] == '/')
                start += 1;

            int length = absolute_path.Length - start;
            return absolute_path.Substring(start, length);
        }
    }
}