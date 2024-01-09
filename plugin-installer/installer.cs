using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;

public class CPHInline
{
	public bool Execute()
	{
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string obsRoot = args["obsRootDir"].ToString();
        string dlFolder = obsRoot + @"\plugin-downloads\";
        string logFolder = @"plugin-install-logs\";
        string pluginList = obsRoot + @"\plugins.txt";
        // it's `;` because url parameters have a = in them -.-
        char pluginListDelimiter = ';';
        if (!Directory.Exists(dlFolder))
        {
            CPH.LogInfo($"dir not present!");
            Directory.CreateDirectory(dlFolder+logFolder);
        } else
        {
            CPH.LogInfo($"{dlFolder} present, deleting!");
            Directory.Delete(dlFolder, true);

            CPH.LogInfo($"creating {dlFolder}.");
            Directory.CreateDirectory(dlFolder+logFolder);
        }
        
        Process[] obsProc = Process.GetProcessesByName("obs64");
        if (obsProc.Length > 0)
        {
            CPH.LogWarn("killing obs64 process.");
            //kill proc
            foreach (Process item in obsProc)
            {
                item.CloseMainWindow();
                CPH.Wait(1000);
                item.Close();
            }
        }

        Dictionary<string, string> plugins = new Dictionary<string, string>();
        Dictionary<string, string> uninstallPlugins = new Dictionary<string, string>();
        var lines = File.ReadAllLines(pluginList);
        foreach (var line in lines) {
            string[] substrings = line.Split(pluginListDelimiter);
            string name = substrings[0].Trim();
            string url = substrings[1].Trim();
            if (name.StartsWith("#")){
                CPH.LogInfo($"uninstall {name.Replace("#", string.Empty)}");
                uninstallPlugins.Add(name.Replace("#", string.Empty),url);
            } else {
                CPH.LogInfo($"add {name} with {url}");
                plugins.Add(name,url);
            }
        }

        List<string> zipContent = new List<string>();
        List<string> excludes = new List<string>{"obs-plugins/", "data/", "data/obs-plugins/", "obs-plugins/64bit/", "obs-plugins/32bit/"};
        foreach (KeyValuePair<string, string> item in plugins)
        {
            string logFile = dlFolder + logFolder + item.Key + ".txt";
            string zipFile = dlFolder + item.Key + ".zip";
            using (var client = new WebClient()) {
                CPH.LogInfo($"downloading {item.Key}");
                client.DownloadFile(item.Value, zipFile);
            }
            // get zipfile content and write the install.log
            using (ZipArchive archive = ZipFile.OpenRead(zipFile)){
                string topFolder = "";
                foreach (ZipArchiveEntry entry in archive.Entries){
                    string pathEntry = entry.FullName;
                    if ( !(pathEntry.StartsWith("obs-plugins/")) && !(pathEntry.StartsWith("data/")) ){
                        int lastLocation = pathEntry.IndexOf( "/" );
                        topFolder = pathEntry.Substring( 0, lastLocation +1 );
                        CPH.LogInfo($"{ pathEntry } is wrong by {pathEntry.Substring( 0, lastLocation +1 )}");
                        pathEntry = pathEntry.Substring( lastLocation + 1 );

                    }
                    if (!String.IsNullOrEmpty(pathEntry)) {
                        zipContent.Add(pathEntry);
                    }
                }
                // remove the directories, for logging and later uninstalling we do not want to ruin obs entirely
                excludes.ForEach(s => zipContent.Remove(s));
                CPH.LogInfo($"write logfile for {item.Key} to {logFile}");
                // write logfile
                File.WriteAllLines(logFile, zipContent);

                CPH.LogInfo($"extracting {item.Key} from { zipFile }");
                archive.ExtractToDirectory(dlFolder);

                // move rogues
                if (!string.IsNullOrEmpty(topFolder)){
                    CPH.LogInfo($"{topFolder} content needs to be moved.");
                }
            }

            File.Delete(zipFile);
            zipContent.Clear();
            // if a plugin already exists, wipe it from obs so we can have a fresh installation
            CPH.LogInfo("checking: " + obsRoot + @"\plugin-install-logs\" + item.Key + ".txt");
            if (File.Exists(obsRoot + @"\plugin-install-logs\" + item.Key + ".txt")){
                Dictionary<string, string> existingPlugin = new Dictionary<string, string>();
                existingPlugin.Add(item.Key, "-");
                uninstall(obsRoot+ @"\", existingPlugin);
                existingPlugin.Clear();
            } else {
                CPH.LogInfo($"no cleanup for {item.Key}. This is probably a new plugin.");
            }
        }
        
        // uninstall all marked plugins
        uninstall(obsRoot+ @"\", uninstallPlugins);
        Process.Start(dlFolder);
        Process.Start(obsRoot);
        CPH.LogInfo($"we're done downloading and extracting. check {dlFolder} content and copy it to {obsRoot} manually.");
		return true;
	}

    public bool uninstall(string dlFolder, Dictionary<string, string> plugins){
        CPH.LogInfo($"uninstall existing versions of plugins.");
        foreach (KeyValuePair<string, string> item in plugins)
        {
            CPH.LogInfo($"uninstall {item.Key}");
            try {
                string logFile = dlFolder + @"plugin-install-logs\" + item.Key + ".txt";
                List<string> lines = new List<string>(File.ReadLines(logFile));

                lines.ForEach(l => {
                    // CPH.LogInfo($"delete {dlFolder + l}");
                    try {File.Delete(dlFolder + l);}
                    catch (System.Exception e) {}
                    try {Directory.Delete(dlFolder + l, true);}
                    catch (System.Exception e) {}
                });
                File.Delete(logFile);
            }
            catch (FileNotFoundException) {
                CPH.LogError($"error uninstalling, logFile not found. Plugin probably already uninstalled.");
            }
            catch (System.Exception e) {
                CPH.LogError(e.ToString());
            }
        }
		return true;
    }
}
