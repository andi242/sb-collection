using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class CPHInline
{
	public bool Execute()
	{
        string soScene = "[n] videoshoutout";
        string vsource = "vso";
        string tsource = "clipTitle";
        string usource = "user";
        string gsource = "gameImg";
        // nested scene items
        string nSourceId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ vsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nTextId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ tsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nUserId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ usource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nGameImgId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ gsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        CPH.LogInfo($"nested scene ids: nSourceId {nSourceId}, nTextId {nTextId}, nUserId {nUserId}, nGameImgId {nGameImgId}");
        List<string> soSources = new List<string> { nGameImgId , nSourceId, nTextId, nUserId };
        
        DateTime now = DateTime.Now;
		DateTime startdate = DateTime.Now;
		Random randomNumber = new Random();
		startdate = startdate.AddDays(-300);
        string vsoFile = args["vsoFile"].ToString();
        string userName = args["rawInput"].ToString();
        List<string> clipList = CPH.GetGlobalVar<List<string>>("clipList", true);
        if (clipList == null){
            clipList = new List<string>();
        }
        
        CPH.LogInfo($"getting clips for {userName}");
        var clips = CPH.GetClipsForUser(userName,startdate,now);
        if (clips.Count != 0) {
            CPH.Wait(100);
            int clipid = randomNumber.Next(0,clips.Count);
            var clip = clips[clipid];
            // if already played, skip this. no recall, to prevent infinite loop if only one clip availables
            if (clipList.Contains(clip.Id)) {
                CPH.LogInfo($"{clip.BroadcasterName}s {clip.Id} already played.");
                return false;
            }
            string videoUrl = Regex.Replace(clip.ThumbnailUrl, "-preview-.*",".mp4");
            string videoplayerfile = vsoFile;
            videoplayerfile += "?video=" + videoUrl;
            int delay = 700 + (int)(clip.Duration * 1000);
            CPH.ObsSetBrowserSource(soScene, vsource, videoplayerfile);
            CPH.ObsSetGdiText(soScene, usource, clip.BroadcasterName, 0);
            CPH.ObsSetGdiText(soScene, tsource, clip.Title, 0);
            string gameArt = "https://static-cdn.jtvnw.net/ttv-boxart/" + clip.GameId + "-300x400.jpg";
            CPH.ObsSetImageSourceFile(soScene, gsource, gameArt, 0);

            // boot clip
            CPH.Wait(300);
            // send obs raw to preserve transitions
            CPH.LogInfo($"enabling vso sources.");
            foreach (var source in soSources) {
                CPH.ObsSendRaw("SetSceneItemEnabled", "{\"sceneName\":\""+ soScene +"\",\"sceneItemId\":"+ source +",\"sceneItemEnabled\":true}", 0);
            }
            CPH.Wait(800); //for animations
            
            // uncomment to post link to clip link to chat
            //CPH.SendMessage($"Clip von @{clip.BroadcasterName}: {clip.Url}", true);

            // wait for clip to finish    
            CPH.Wait(delay);

            // shutdown clip
            CPH.LogInfo($"disabling vso sources.");
            foreach (var source in soSources) {
                CPH.ObsSendRaw("SetSceneItemEnabled", "{\"sceneName\":\""+ soScene +"\",\"sceneItemId\":"+ source +",\"sceneItemEnabled\":false}", 0);
            }
            // save played clip to list
            clipList.Add(clip.Id);
            CPH.SetGlobalVar("clipList", clipList, true);

            CPH.Wait(1000); //for animations
            // reset sources
            CPH.ObsSetBrowserSource(soScene, vsource, "about:blank");
            CPH.ObsSetGdiText(soScene, usource, "", 0);
            CPH.ObsSetGdiText(soScene, tsource, "", 0);
            CPH.ObsSetImageSourceFile(soScene, gsource, "", 0);
        } else {
            CPH.LogInfo($"no clips found for {userName}.");
            CPH.SendMessage($"no clips found for {userName}.", true);
        }
		return true;
	}
}
