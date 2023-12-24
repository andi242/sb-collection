using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    public bool Execute()
    {
        CPH.LogInfo("import starting");
        string scale = args["scale"].ToString();
        string alertScene = args["alertScene"].ToString();
        CPH.ObsSendRaw("CreateScene", "{\"sceneName\":\""+alertScene+"\"}", 0);
        var mp3Dir = args["soundsDir"].ToString();
        var mp4Dir = args["videosDir"].ToString();
        // positions of scaled video files on canvas (upper left corner of video)
        var positionX = 100;
        var positionY = 600;
        // set supported media file extensions
        List<string> videoFormats = new List<string> { "*.mp4", "*.mov", "*.avi", "*.mkv" };
        List<string> audioFormats = new List<string> { "*.mp3", "*.wav", "*.aac", "*.ogg" };
        // is obs connected to SB?
        var isConnected = args["obs.isConnected"].ToString();
        if (isConnected != "True" )
        {
            CPH.LogInfo("error: OBS not connected");
            return false;
        }
        // create list of media files
        List<string> mediaFiles = new List<string>();
        // append video files
        foreach (var format in videoFormats)
        {
            mediaFiles.AddRange(Directory.GetFiles(mp4Dir, format, SearchOption.AllDirectories));
        }
        // append audio files
        foreach (var format in audioFormats)
        {
            mediaFiles.AddRange(Directory.GetFiles(mp3Dir, format, SearchOption.AllDirectories));
        }
        
        try
        {
            CPH.LogInfo( mediaFiles.Count + " files total");
            // if no files to process, exit.
            if (mediaFiles.Count == 0 )
            {
                CPH.LogInfo("error: " + mediaFiles.Count + " files found");
                return false;
            }
            // process file list
            foreach (var file in mediaFiles)
            {
                string alert = Path.GetFileNameWithoutExtension(file);
                string jsonPath = file.Replace('\\', '/');
                string state = CPH.ObsSendRaw("GetInputSettings", "{\"inputName\":\""+alert+"\"}", 0);
                // if input already exists, skip it.
                if (state.ToString() == "{}"){
                    CPH.ObsSendRaw("CreateInput", "{\"sceneItemEnabled\": false, \"inputName\": \""+alert+"\",\"inputKind\": \"ffmpeg_source\", \"sceneName\": \""+alertScene+"\",\"inputSettings\": {\"local_file\": \""+jsonPath+"\"}}");
                    CPH.ObsSendRaw("SetInputAudioMonitorType", "{\"inputName\": \""+alert+"\", \"monitorType\": \"OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT\"}");
                    string idJson = CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+alertScene+"\",\"sourceName\":\""+alert+"\",\"searchOffset\":0}", 0);
                    JToken id = JToken.Parse(idJson);
                    // the id is only returned if it is a resizable object in OBS. otherwise this would result in a crash the websocket (audio sources do not display, so no resizing on those)
                    if (id.ToString() != "{}"){
                        // resize item
                        CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\""+alertScene+"\",\"sceneItemId\":"+id["sceneItemId"]+",\"sceneItemTransform\":{\"scaleX\":"+scale+",\"scaleY\":"+scale+",\"positionX\":"+positionX+",\"positionY\":"+positionY+"}}", 0);
                    }
                    CPH.LogInfo("created: " + alert.ToString());
                } else {
                    CPH.LogInfo("skipping " + alert.ToString() + ", already present.");
                }
            }
        }
        catch (Exception ex)
        {
            CPH.LogInfo(ex.Message);
        }
        CPH.LogInfo("import finished");
        return true;
    }
}