using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

class CPHInline{
    
    public bool Execute(){

        int count = int.Parse(args["loops"].ToString());
        string[] scenes = { "allowed scene a", "allowed scene b" };
        // name of the blocking queue for the shoutouts
        string queue = "vso";
        
        string soScene = "[n] videoshoutout"; // nested scene to use
        string vsource = "vso"; // video source
        string tsource = "clipTitle"; // text source
        string usource = "user"; // text source
        string gsource = "gameImg"; // img source
        // get some obs info
        string nSourceId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ vsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nTextId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ tsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nUserId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ usource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        string nGameImgId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+ soScene +"\",\"sourceName\":\""+ gsource +"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
        List<string> soSources = new List<string> { nGameImgId , nSourceId, nTextId, nUserId };
        // reset the global cliplist var
        CPH.UnsetGlobalVar("clipList", true);
        
        if (scenes.Contains(CPH.ObsGetCurrentScene(0)) == false) {
            CPH.LogInfo($"{CPH.ObsGetCurrentScene(0)} is not my scene.");
            foreach (var source in soSources) {
                CPH.ObsSendRaw("SetSceneItemEnabled", "{\"sceneName\":\""+ soScene +"\",\"sceneItemId\":"+ source +",\"sceneItemEnabled\":false}", 0);   
            }
            CPH.ResumeActionQueue(queue, true);
            return false;
        }

        List<string> streamerlist = CPH.GetGlobalVar<List<string>>("vsoStreamerlist", true);
        if (streamerlist == null){
            streamerlist = new List<string>();
            //streamerlist.Add("yourFavTwitchStreamer");
            //streamerlist.Add("add more of this for shoutouts");
            
        }
        for (int i = 0; i < count; i++) {
            // randomize the list
            var rnd = new Random();
            var randomList = streamerlist.OrderBy(item => rnd.Next());
            CPH.LogInfo($"trigger {randomList.Count()} shoutouts.");
            foreach (var item in randomList) {
                // set the user to shoutout to this argument for the triggered action
                CPH.SetArgument("rawInput", item);
                // the custom trigger that triggers the action on singleVSO
                CPH.TriggerEvent("soTrigger", true);
            }
        }
		return true;
	}
}