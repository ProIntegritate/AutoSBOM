// Written in 2024 by Glenn Larsson.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

using System;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoSBOM_CS
{
    // Usage: string Results = AutoSBOM_CS.fGetSBOMFromAssembies();
    public class AutoSBOM
    {

        public static string vbCRLF = "\r\n"; // yeah, i don't like checking escaped sequences.

        // Embedded resource
        // NOTE: This is your job to include this file (project.assets.json), it is often found in the obj folder, but ONLY  after adding a nuget package.
        public static string sSBOMEmbededResourceFile = "project.assets.json"; 

        // Auto init with Project name
        public static string sSBOMProjectName = Assembly.GetCallingAssembly().FullName.Split(",")[0];

        public static string fGetSBOMFromAssembies() {

            string sResults = ""; // Results to return

            // Get assemblies for project
            Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();

            // Build initial internal Reflection.Assembly list of items:
            foreach (Assembly aItem in asm) {
                sResults += "Package: " + aItem.GetName().Name + ", ";
                sResults += "Version: " + aItem.GetName().Version + vbCRLF;
            }

            // ---- Below we get the embedded project.assets.json references ----

            // Embedded file name. Unlike in VB .NET, ".obj." is added after projectname in C# for some reason.
            string sEmbeddedFile = sSBOMProjectName + ".obj." + sSBOMEmbededResourceFile;
            System.IO.Stream oStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sEmbeddedFile); ;

            System.IO.StreamReader strReader;
            string sEmbeddedProjectAssetsFile = "";

            try {
                // Get Reference stream from assembly -> string
                strReader = new StreamReader(oStream);
                sEmbeddedProjectAssetsFile = strReader.ReadToEnd();
            }
            catch (Exception e)
            {
                // If this fails, just return the reflection assemblies. Can't check if a Stream == Nothing in C#, and NULL is not the same.
                return fSortData(sResults);
            }

            // Read the resource into string[] array
            string[] sLines = sEmbeddedProjectAssetsFile.Split(vbCRLF);

            string sTmp = "";
            string sTmpLine = "";
            foreach (string foo in sLines)
            {
                // copy to tmp as iterators are static in C#...
                sTmp = foo;

                //
                // A proper line we need looks like
                // "System.Text.Json/8.0.1": {
                // Check Regexp below
                //

                if (System.Text.RegularExpressions.Regex.IsMatch(sTmp, "[A-Za-z\\./]{1,}\\/[0-9\\.]{3,}\": {") == true) {

                    // Remove unwanted characters
                    sTmp = System.Text.RegularExpressions.Regex.Replace(sTmp, "\"", ""); 
                    sTmp = System.Text.RegularExpressions.Regex.Replace(sTmp, ":", "");
                    sTmp = System.Text.RegularExpressions.Regex.Replace(sTmp, "{", "");
                    sTmp = System.Text.RegularExpressions.Regex.Replace(sTmp, "/", ",");
                    sTmp = sTmp.Trim(); // Trim spaces

                    // Build sTmpLine
                    sTmpLine = "Package: " + sTmp.Split(",")[0];
                    sTmpLine = sTmpLine + "Version: " + sTmp.Split(",")[1];

                    // Add sTmpLine if not found
                    if (sResults.IndexOf(sTmpLine) == -1 )
                    {
                        sResults = sResults + sTmpLine + vbCRLF;
                    }
                }
            }
            return fSortData(sResults);
        }

        // Convert and sort string results using Array.Sort()
        public static string fSortData(string sData) {

            string[] stufftosort = sData.Split(vbCRLF);
            Array.Sort(stufftosort);

            string sResult = "";

            foreach (string s in stufftosort)
            {
                if (s != "")
                    sResult = sResult + s + vbCRLF;
            }

            return sResult;
        }


    }
}

