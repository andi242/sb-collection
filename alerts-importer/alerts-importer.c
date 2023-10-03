using System;
using System.IO;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        CPH.LogInfo("import startet");
        string alertScene = args["alertScene"].ToString();
        var mp3Dir = args["soundsDir"].ToString();
        var mp4Dir = args["videosDir"].ToString();
        List<string> videoFormats = new List<string> { "*.mp4", "*.mov", "*.avi", "*.mkv" };
        List<string> audioFormats = new List<string> { "*.mp3", "*.wav", "*.aac", "*.ogg" };
        var isConnected = args["obs.isConnected"].ToString();
        if (isConnected != "True" )
        {
            CPH.LogInfo("error: OBS nicht verbunden");
            return false;
        }
        List<string> mediaFiles = new List<string>();
        // video file formats
        foreach (var format in videoFormats)
        {
            mediaFiles.AddRange(Directory.GetFiles(mp4Dir, format, SearchOption.AllDirectories));
        }
        // sound file formats
        foreach (var format in audioFormats)
        {
            mediaFiles.AddRange(Directory.GetFiles(mp3Dir, format, SearchOption.AllDirectories));
        }
        
        try
        {
            CPH.LogInfo("insgesamt " + mediaFiles.Count + " Dateien");
            if (mediaFiles.Count == 0 )
            {
                CPH.LogInfo("error: " + mediaFiles.Count + " Dateien");
                return false;
            }
            foreach (var file in mediaFiles)
            {
                string alert = Path.GetFileNameWithoutExtension(file);
                string jsonPath = file.Replace('\\', '/');
                CPH.ObsSendRaw("CreateInput", "{\"sceneItemEnabled\": false, \"inputName\": \""+alert+"\",\"inputKind\": \"ffmpeg_source\", \"sceneName\": \""+alertScene+"\",\"inputSettings\": {\"local_file\": \""+jsonPath+"\"}}");
                CPH.ObsSendRaw("SetInputAudioMonitorType", "{\"inputName\": \""+alert+"\", \"monitorType\": \"OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT\"}");
                CPH.LogInfo("verarbeitet: " + alert.ToString());
            }
        }
        catch (Exception ex)
        {
            CPH.LogInfo(ex.Message);
            CPH.LogInfo("err");
        }
        CPH.LogInfo("import abgeschlossen");
        return true;
    }
}