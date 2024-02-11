using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class CPHInline
{
	public int maxWidth = 960;
	public string textColor = "3707764735";
    public List<string> rndColors = new List<string>() {"3699375872", "3691031295", "3707742720", "3699354112", "3707699285", "3707721215", "3691009280", "3699332437"};

    public bool Execute()
	{
        string widgetScene = args["widgetScene"].ToString();
        string rawInput = args["rawInput"].ToString();
        if (string.IsNullOrEmpty(rawInput)) {
			CPH.LogInfo("polly has no content!");
			rawInput = "Quickpoll?yes,no";
        }
        string[] input = rawInput.Split(new char[]{'?'}, 2);
        string pollTopic = input[0].ToString()+"?";
		string[] options = input[1].Split(new char[]{',', ' '}, StringSplitOptions.RemoveEmptyEntries);
        
        Dictionary<string, int> pollBarsDict = new Dictionary<string, int>();
        List<string> pollVoters = new List<string>();
        CPH.SetGlobalVar("pollVoters", pollVoters, true);
        foreach (string item in options)
        {
            pollBarsDict.Add(item, 0);
        }
        CPH.SetGlobalVar("pollBarsDict", pollBarsDict, true);
        // create title
        string titleId = createTitle(pollTopic, widgetScene);
        CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\""+ widgetScene +"\",\"sceneItemId\":"+ titleId +",\"sceneItemTransform\":{\"positionX\":40,\"positionY\":100}}", 0);
        // create bars
        createBars(pollBarsDict, widgetScene);
        CPH.EnableAction("pollVote");
        return true;
	}

    public string createBar(string name, string widgetScene){
        var randomIndex = new Random().Next(rndColors.Count);
        var randomColor = rndColors.ElementAtOrDefault(randomIndex);
        //string barId = JToken.Parse(CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\""+widgetScene+"\",\"inputName\":\""+ name +"\",\"inputKind\":\"color_source_v3\",\"inputSettings\":{\"height\":80,\"width\":10,\"color\":"+randomColor+"},\"sceneItemEnabled\":false}", 0)).Value<JToken>("sceneItemId").ToString();
        string barId = JToken.Parse(CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\""+widgetScene+"\",\"inputName\":\""+ name +"\",\"inputKind\":\"color_source_v3\",\"inputSettings\": { \"color\": "+randomColor+"},\"sceneItemEnabled\":false}", 0)).Value<JToken>("sceneItemId").ToString();
        CPH.Wait(100);
        CPH.ObsSendRaw("CreateSourceFilter", "{\"sourceName\":\""+name+"\",\"filterName\":\"mask\",\"filterKind\":\"advanced_masks_filter_v2\",\"filterSettings\":{ \"rectangle_corner_radius\": 40,\"rectangle_corner_type\": 1, \"rectangle_height\": 80,\"rectangle_width\": 800,\"shape_feather_amount\": 3,\"shape_feather_type\": 4, }}", 0);
        CPH.ObsSendRaw("CreateSourceFilter", "{\"sourceName\":\""+name+"\",\"filterName\":\"glow\",\"filterKind\":\"obs_glow_filter\",\"filterSettings\":{ \"blur_type\": 2,\"glow_size\": 6, \"fill\": false}}", 0);
        CPH.Wait(100);
        CPH.ObsSendRaw("CreateSourceFilter", "{\"sourceName\":\""+name+"\",\"filterName\":\"move\",\"filterKind\":\"move_value_filter\",\"filterSettings\":{ \"filter\": \"mask\",\"setting_name\": \"rectangle_width\", \"value_type\": 2, \"setting_float\": 100}}", 0);
        CPH.Wait(100);
        CPH.ObsSendRaw("SetSourceFilterEnabled", "{\"sourceName\":\""+name+"\",\"filterName\":\"move\",\"filterEnabled\":true}", 0);

        CPH.LogInfo($"created bar {name}");
        return barId;
    }
    public string createTitle(string name, string widgetScene){
        string titleId = JToken.Parse(CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\""+widgetScene+"\",\"inputName\":\"pollTitle\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":{\"text\":\""+name.ToUpper()+"\",\"color\":"+textColor+", \"bk_opacity\": 0, \"font\":{\"size\":72}},\"sceneItemEnabled\":true}", 0)).Value<JToken>("sceneItemId").ToString();
        CPH.LogInfo($"created title {name}");
        return titleId;
    }
    public string createLabel(string name, string widgetScene, int index){
        string labelId = JToken.Parse(CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\""+widgetScene+"\",\"inputName\":\""+ name +"_label\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":{\"text\":\""+index+": "+name.ToUpper()+"\",\"color\":"+textColor+", \"bk_opacity\": 0, \"font\":{\"size\":60}},\"sceneItemEnabled\":false}", 0)).Value<JToken>("sceneItemId").ToString();
        CPH.LogInfo($"created label {name}");
        return labelId;
    }
    public void createBars(Dictionary<string, int> pollBarsDict, string widgetScene){
        // get text width, save max
        var widthList = new List<int>();
        int index = 1;
        foreach (KeyValuePair<string, int> item in pollBarsDict)
        {
            CPH.LogInfo($"index: {index}");
            string barId = createBar(item.Key, widgetScene);
            string labelId = createLabel(item.Key, widgetScene, index);
            CPH.Wait(300);
            JToken sceneItemTransform = JToken.Parse(CPH.ObsSendRaw("GetSceneItemTransform", "{\"sceneName\":\""+ widgetScene +"\",\"sceneItemId\":"+ labelId +"}", 0)).Value<JToken>("sceneItemTransform");
            int textWidth = Convert.ToInt32(sceneItemTransform.Value<JToken>("sourceWidth").ToString());
            //CPH.LogInfo($"textWidth: {textWidth.ToString()}");
            widthList.Add(textWidth);
            index++;
        }

        int posX = (int)widthList.Max() + 40;
        int i = 0;
        foreach (var item in pollBarsDict)
        {
            int posY = 200 + (i*110);
            string barId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+widgetScene+"\",\"sourceName\":\""+item.Key+"\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();
            string labelId = JToken.Parse(CPH.ObsSendRaw("GetSceneItemId", "{\"sceneName\":\""+widgetScene+"\",\"sourceName\":\""+item.Key+"_label\",\"searchOffset\":0}", 0)).Value<JToken>("sceneItemId").ToString();

            // position label
            CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\""+ widgetScene +"\",\"sceneItemId\":"+ labelId +",\"sceneItemTransform\":{\"positionX\":20,\"positionY\":"+ (posY+10) +"}}", 0);
            // position adv. mask
            CPH.ObsSendRaw("SetSourceFilterSettings", "{\"sourceName\":\""+item.Key+"\",\"filterName\":\"mask\",\"filterSettings\":{\"shape_center_x\": 0, \"shape_center_y\":"+ (posY + 40)+", \"rectangle_width\": "+(posX*2)+" },\"overlay\":true}", 0);
            CPH.Wait(200);
            CPH.ObsSendRaw("SetSceneItemEnabled", "{\"sceneName\":\""+ widgetScene +"\",\"sceneItemId\":"+ barId +",\"sceneItemEnabled\":true}", 0);
            CPH.Wait(100);
            CPH.ObsSendRaw("SetSceneItemEnabled", "{\"sceneName\":\""+ widgetScene +"\",\"sceneItemId\":"+ labelId +",\"sceneItemEnabled\":true}", 0);
            i++;
        }
    }
    public bool resolvePoll(){
        Dictionary<string, int> pollBarsDict = CPH.GetGlobalVar<Dictionary<string, int>>("pollBarsDict", true);
        // var winner = pollBarsDict.OrderByDescending(x => x.Value).First();
        // CPH.LogInfo($"{winner.Key} won");
        // CPH.SendMessage($"Option \"{winner.Key.ToUpper()}\" gewinnt die Abstimmung! :tada:", true);
        string results = "";
        foreach (var item in pollBarsDict){
            CPH.ObsSendRaw("SetSourceFilterSettings", "{\"sourceName\":\""+item.Key+"\",\"filterName\":\"move\",\"filterSettings\":{\"setting_int\":0,\"setting_name\":\"width\"},\"overlay\":true}", 0);
            CPH.Wait(300);
            CPH.ObsSendRaw("SetSourceFilterEnabled", "{\"sourceName\":\""+item.Key+"\",\"filterName\":\"move\",\"filterEnabled\":true}", 0);
            CPH.Wait(300);
            CPH.ObsSendRaw("RemoveInput", "{\"inputName\":\""+item.Key+"\"}", 0);
            CPH.ObsSendRaw("RemoveInput", "{\"inputName\":\""+item.Key+"_label\"}", 0);
            CPH.LogInfo($"removed {item.Key}");
            results += $"{item.Key}: {item.Value} | ";
        };
        CPH.SendMessage($"| Results | {results}", true);
        CPH.Wait(300);
        CPH.ObsSendRaw("RemoveInput", "{\"inputName\":\"pollTitle\"}", 0);
        CPH.DisableAction("pollVote");
        CPH.UnsetGlobalVar("pollBarsDict", true);
        CPH.UnsetGlobalVar("pollVoters", true);
        CPH.UnsetAllUsersVar("pollVote",true);
        CPH.LogInfo("poll resolved, user vars deleted.");
        return true;
    }
    public void updateBars(){
        List<string> pollVoters = CPH.GetGlobalVar<List<string>>("pollVoters", true);
        Dictionary<string, int> pollBarsDict = CPH.GetGlobalVar<Dictionary<string, int>>("pollBarsDict", true);
        // set all to zero and then refill
        pollBarsDict = pollBarsDict.ToDictionary(x => x.Key, x => 0);
        foreach (string voter in pollVoters)
        {
            int userVote = CPH.GetTwitchUserVar<int>(voter, "pollVote", true);
            int currentVotes = pollBarsDict[pollBarsDict.Keys.ElementAt(userVote-1)];
            CPH.LogInfo($"current {currentVotes}");
            pollBarsDict[pollBarsDict.Keys.ElementAt(userVote-1)] = currentVotes + 1; 
            CPH.LogInfo($"{voter}: vote {userVote}");
        }
        CPH.SetGlobalVar("pollBarsDict", pollBarsDict, true);
        var sum = pollBarsDict.Values.Cast<int>().Sum();
        foreach (var item in pollBarsDict)
        {
            var percentage = Math.Round((double)item.Value / sum * 100);
            var setWidth = Math.Round(maxWidth * percentage / 100);
            CPH.ObsSendRaw("SetSourceFilterSettings", "{\"sourceName\":\""+item.Key+"\",\"filterName\":\"move\",\"filterSettings\":{\"setting_float\":"+(setWidth*2+40)+",},\"overlay\":true}", 0);
            CPH.Wait(100);
            CPH.ObsSendRaw("SetSourceFilterEnabled", "{\"sourceName\":\""+item.Key+"\",\"filterName\":\"move\",\"filterEnabled\":true}", 0);
        }
    }
    public bool registerVote(){
        int i;
        string user = args["user"].ToString();
        string voteInput = args["message"].ToString();
        bool success = int.TryParse(voteInput, out i);
        if(success){
            CPH.SetTwitchUserVar(user, "pollVote", voteInput, true);
            Dictionary<string, int> pollBarsDict = CPH.GetGlobalVar<Dictionary<string, int>>("pollBarsDict", true);
            CPH.LogInfo($"{voteInput} vs {pollBarsDict.Count}");
            // if out of range, return false
            if (int.Parse(voteInput) > pollBarsDict.Count)
            {
                CPH.LogInfo($"{user} invalid vote.");
                return false;
            } else {
                List<string> pollVoters = CPH.GetGlobalVar<List<string>>("pollVoters", true);
                // update voters
                if (!pollVoters.Contains(user))
                {
                    pollVoters.Add(user);
                }
                CPH.SetGlobalVar("pollVoters", pollVoters, true);
                updateBars();
            }
        } else {
		    CPH.LogInfo($"just text from {user}");
        }
        return true;
    }

}
