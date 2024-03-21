using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    public bool setupVso(){
        // get vars
        string soScene = args["Scene"].ToString();
        string vsource = args["videoSrc"].ToString();
        string tsource = args["titleSrc"].ToString();
        string usource = args["userSrc"].ToString();
        string gsource = args["imageSrc"].ToString();
        // save settings 
        CPH.SetGlobalVar("vsoScene", soScene, true);
        CPH.SetGlobalVar("vsoVideoSrc", vsource, true);
        CPH.SetGlobalVar("vsoTitleSrc", tsource, true);
        CPH.SetGlobalVar("vsoUserSrc", usource, true);
        CPH.SetGlobalVar("vsoImageSrc", gsource, true);
        CPH.LogInfo($"checking for {soScene}");
        try {
            string testId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ vsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
            CPH.LogError($"setup found in {soScene}, will not overwrite.");
            return false;
        }
        catch (System.Exception) {
            CPH.LogInfo($"{soScene} check failed, proceeding setup.");
        }

        // create scene
        CPH.LogInfo($"creating: {soScene}");
        CPH.ObsSendRaw("CreateScene", "{\"sceneName\":\"" + soScene + "\"}", 0);
        // create sources in scene
        CPH.LogInfo($"creating: {vsource}");
        CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"" + soScene + "\",\"inputName\":\"" + vsource + "\",\"inputKind\":\"ffmpeg_source\",\"inputSettings\":{\"restart_on_activate\": false, \"close_when_inactive\": true,\"is_local_file\": false},\"sceneItemEnabled\":false}", 0);
        CPH.ObsSendRaw("SetInputAudioMonitorType", "{\"inputName\": \"" + vsource + "\", \"monitorType\": \"OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT\"}");

        CPH.LogInfo($"creating: {gsource}");
        CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"" + soScene + "\",\"inputName\":\"" + gsource + "\",\"inputKind\":\"image_source\",\"inputSettings\":null,\"sceneItemEnabled\":true}", 0);
        CPH.LogInfo($"creating: {tsource}");
        CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"" + soScene + "\",\"inputName\":\"" + tsource + "\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":null,\"sceneItemEnabled\":true}", 0);
        CPH.LogInfo($"creating: {usource}");
        CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"" + soScene + "\",\"inputName\":\"" + usource + "\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":null,\"sceneItemEnabled\":true}", 0);

        // get source IDs
        string vSourceId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ vsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nTextId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ tsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nUserId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ usource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nGameImgId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ gsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();

        // format sources
        CPH.LogInfo($"formatting sources");
        CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"" + soScene + "\",\"sceneItemId\":"+vSourceId+",\"sceneItemTransform\":{\"boundsHeight\": 1080,\"boundsType\": \"OBS_BOUNDS_SCALE_OUTER\",\"boundsWidth\": 1920,\"alignment\": 5}}", 0);
        CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"" + soScene + "\",\"sceneItemId\":"+nUserId+",\"sceneItemTransform\":{\"boundsAlignment\": 1,\"boundsHeight\": 264,\"boundsType\": \"OBS_BOUNDS_MAX_ONLY\",\"boundsWidth\":  960,\"alignment\": 5,\"positionX\": 0,\"positionY\": 0}}", 0);
        CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"" + soScene + "\",\"sceneItemId\":"+nTextId+",\"sceneItemTransform\":{\"boundsAlignment\": 1,\"boundsHeight\": 264,\"boundsType\": \"OBS_BOUNDS_MAX_ONLY\",\"boundsWidth\": 1920,\"alignment\": 5,\"positionX\": 0,\"positionY\": 264}}", 0);
        CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"" + soScene + "\",\"sceneItemId\":"+nGameImgId+",\"sceneItemTransform\":{\"positionX\": 1620,\"positionY\": 680}}", 0);
        // decrease vso volume
        CPH.ObsSendRaw("SetInputVolume", "{\"inputName\":\""+vsource+"\",\"inputVolumeDb\":-20}", 0);

        CPH.ShowToastNotification("", "Streamerbot VSO Setup", $"Please setup your sources in {soScene} to your style and needs.", "","");
        return true;
    }
    
    public bool Execute()
	{
        string soScene = CPH.GetGlobalVar<string>("vsoScene", true);
        string vsource = CPH.GetGlobalVar<string>("vsoVideoSrc", true);
        string tsource = CPH.GetGlobalVar<string>("vsoTitleSrc", true);
        string usource = CPH.GetGlobalVar<string>("vsoUserSrc", true);
        string gsource = CPH.GetGlobalVar<string>("vsoImageSrc", true);
        // get nested scene item ids
        string nSourceId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ vsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nTextId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ tsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nUserId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ usource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nGameImgId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ gsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        //CPH.LogInfo($"nested scene ids: nSourceId {nSourceId}, nTextId {nTextId}, nUserId {nUserId}, nGameImgId {nGameImgId}");
        List<string> soSources = new List<string> { nGameImgId , nSourceId, nTextId, nUserId };
        
        string userName = args["rawInput"].ToString();
        List<string> clipList = CPH.GetGlobalVar<List<string>>("clipList", true);
        if (clipList == null){
            clipList = new List<string>();
        }
        
        CPH.LogInfo($"getting clips for {userName}");
        var clips = CPH.GetClipsForUser(userName, true); // only get the featured clips of user
        if (clips.Count != 0) {
            Random randomNumber = new Random();
            CPH.Wait(100);
            int clipid = randomNumber.Next(0,clips.Count);
            var clip = clips[clipid];
            // if already played, skip this. no recall, to prevent infinite loop if only one clip availables
            if (clipList.Contains(clip.Id)) {
                CPH.LogInfo($"{clip.BroadcasterName}s {clip.Id} already played.");
                return false;
            }
            string videoUrl = Regex.Replace(clip.ThumbnailUrl, "-preview-.*",".mp4");

            int delay = 1000 + (int)(clip.Duration * 1000);
            CPH.ObsSetMediaSourceFile(soScene, vsource, videoUrl, 0);
            CPH.ObsSetGdiText(soScene, usource, clip.BroadcasterName, 0);
            CPH.ObsSetGdiText(soScene, tsource, clip.Title, 0);
            string gameArt = "https://static-cdn.jtvnw.net/ttv-boxart/" + clip.GameId + "-300x400.jpg";
            CPH.ObsSetImageSourceFile(soScene, gsource, gameArt, 0);

            // boot clip
            //CPH.Wait(200);
            // send obs raw to preserve transitions
            CPH.LogInfo($"enabling vso sources.");
            CPH.ObsSendRaw("SetInputMute", "{\"inputName\":\""+vsource+"\",\"inputMuted\":false}", 0);
            foreach (var source in soSources) {
                CPH.ObsSendRaw("SetSceneItemEnabled", "{\"sceneName\":\""+ soScene +"\",\"sceneItemId\":"+ source +",\"sceneItemEnabled\":true}", 0);
            }
            CPH.Wait(1000); //for animations
            CPH.LogInfo($"playing clip: {clip.Url}");
            
            // uncomment to post link to clip link to chat
            //CPH.SendMessage($"Clip von @{clip.BroadcasterName}: {clip.Url}", true);

            // wait for clip to finish    
            CPH.Wait(delay);

            // shutdown clip
            CPH.LogInfo($"disabling vso sources.");
            CPH.ObsSendRaw("SetInputMute", "{\"inputName\":\""+vsource+"\",\"inputMuted\":true}", 0);
            foreach (var source in soSources) {
                CPH.ObsSendRaw("SetSceneItemEnabled", "{\"sceneName\":\""+ soScene +"\",\"sceneItemId\":"+ source +",\"sceneItemEnabled\":false}", 0);
            }
            // save played clip to list
            clipList.Add(clip.Id);
            CPH.SetGlobalVar("clipList", clipList, true);

            CPH.Wait(1000); //for animations
            // reset sources
            CPH.ObsSendRaw("SetInputSettings", "{\"inputName\":\""+vsource+"\",\"inputSettings\":{\"input\":\"\"},\"overlay\":true}", 0);
            CPH.ObsSetGdiText(soScene, usource, "", 0);
            CPH.ObsSetGdiText(soScene, tsource, "", 0);
            CPH.ObsSetImageSourceFile(soScene, gsource, "", 0);
        } else {
            CPH.LogInfo($"no clips found for {userName}.");
            //CPH.SendMessage($"no clips found for {userName}.", true);
        }
		return true;
	}
    public bool triggerLoop(){
        int count = int.Parse(args["loops"].ToString());
        string scenes = args["triggerScene"].ToString();
        string queue = "vso"; // the blocking queue, this should be imported with the sb imports
        string soScene = CPH.GetGlobalVar<string>("vsoScene", true);
        string vsource = CPH.GetGlobalVar<string>("vsoVideoSrc", true);
        string tsource = CPH.GetGlobalVar<string>("vsoTitleSrc", true);
        string usource = CPH.GetGlobalVar<string>("vsoUserSrc", true);
        string gsource = CPH.GetGlobalVar<string>("vsoImageSrc", true);
        string nSourceId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ vsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nTextId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ tsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nUserId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ usource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nGameImgId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ gsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        List<string> soSources = new List<string> { nGameImgId , nSourceId, nTextId, nUserId };
        // reset the global cliplist var
        CPH.UnsetGlobalVar("clipList", true);
        
        if (scenes.Contains(CPH.ObsGetCurrentScene(0)) == false) {
            CPH.LogInfo($"{CPH.ObsGetCurrentScene(0)} is not my scene.");
            CPH.ObsSendRaw("SetInputMute", "{\"inputName\":\""+vsource+"\",\"inputMuted\":true}", 0);
            foreach (var source in soSources) {
                CPH.ObsSendRaw("SetSceneItemEnabled", "{\"sceneName\":\""+ soScene +"\",\"sceneItemId\":"+ source +",\"sceneItemEnabled\":false}", 0);   
            CPH.LogInfo($"sceneItemId {source} disabled.");
            }
            CPH.ResumeActionQueue(queue, true);
            CPH.LogInfo($"{queue} queue cleared.");
            return false;
        }

        string streamers = args["streamers"].ToString();
        string[] names = streamers.Split(',');
        List<string> streamerlist = new List<string>(names.Length);
        streamerlist.AddRange(names);

        for (int i = 0; i < count; i++) {
            var rnd = new Random();
            var randomList = streamerlist.OrderBy(item => rnd.Next());
            CPH.LogInfo($"trigger {randomList.Count()} shoutouts.");
            foreach (var item in randomList) {
                string name = item.Trim();
                CPH.SetArgument("rawInput", name);
                CPH.LogInfo($"added {name} to queue.");
                CPH.TriggerEvent("soTrigger", true);
                CPH.Wait(200);
            }
        }
		return true;
    }
}
