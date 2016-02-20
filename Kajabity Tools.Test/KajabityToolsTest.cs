/*
 * Copyright 2009-15 Williams Technologies Limited.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Kajbity is a trademark of Williams Technologies Limited.
 *
 * http://www.kajabity.com
 */

using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kajabity.Tools.Test
{
    [SetUpFixture]
    public class KajabityToolsTest
    {


        public static string ToString(string[] strings)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            if (strings != null)
            {
                for (int i = 0; i < strings.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append("\"");
                    builder.Append(NoNull(strings[i]));
                    builder.Append("\"");
                }
            }

            builder.Append("}");
            return builder.ToString();
        }

        public static string NoNull(string s)
        {
            if (s == null)
            {
                return "";
            }

            return s;
        }

        /// <summary>
        /// Create the named file and store the given content.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void CreateTestFile(string name, string content)
        {
            //TODO: write CreateTestFile
        }

        /// <summary>
        /// Verify that a file exists and contains the specified content exactly.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void CheckFileContent(string name, string content)
        {
            //TODO write CheckFileContent
        }

        /// <summary>
        /// Returns true if two string arrays contain the same values.
        /// </summary>
        /// <param name="a">first string array to compare</param>
        /// <param name="b">second string array to compare</param>
        /// <returns></returns>
        public bool CompareStringArray(string[] a, string[] b)
        {
            if (a == null)
            {
                return b == null;
            }

            if (b == null || a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
