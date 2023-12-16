using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

class CPHInline
{
    public string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    public string voteStartText = "Vote starting!";
    public string dlSound = "https://www.myinstants.com/media/sounds/ding-sound-effect_2.mp3";
    private string alertSound;

    public bool voteStart(){
        alertSound = baseDirectory + "ding.mp3";
        if (!File.Exists(alertSound)){
            CPH.LogInfo("downloading soundalert");
            using (var client = new WebClient()) {
                // download soundalert file to streamerbot directory
                client.DownloadFile(dlSound, alertSound);
                CPH.LogInfo("downloaded soundalert to " + alertSound);
            }
        }
        CPH.EnableAction("voteRegister");
        voteStartText = args["voteStartText"].ToString();
        CPH.SendMessage(voteStartText, true);
        return true;
    }

    public bool voteRegister(){
        string user = args["user"].ToString();
        string voteInput = args["message"].ToString();
        int i;
        bool success = int.TryParse(voteInput, out i);
        if(success){
            if(int.Parse(voteInput) > 10){
                CPH.LogInfo("vote out of range for: " + user );
                return true;
            }
		    CPH.LogInfo($"vote for {user}: {voteInput}");
            CPH.SetTwitchUserVar(user, "pollVote", voteInput, true);
            CPH.PlaySound(alertSound, 0.1f, false);
        } else {
		    CPH.LogInfo($"just text from {user}");
        }
        return true;
    }

    public bool voteResolve(){
        CPH.LogInfo($"resolving votes");
        CPH.DisableAction("voteRegister");
        int voteCount = 0;
        int voteSum = 0;
        float voteResult = 0;
        JToken usersJson = JToken.Parse(File.ReadAllText(baseDirectory + @"/data/users.dat"));
        JObject userNames = usersJson.Value<JObject>("users");
        foreach (KeyValuePair<string, JToken> result in userNames)
        {
            JObject commandArray = userNames.Value<JObject>(result.Key);
            var userName = commandArray["name"].ToString();
            if (commandArray["type"].ToString() == "twitch") {
                if (CPH.GetTwitchUserVar<int>(userName, "pollVote", true) != 0){
                    int pollVote = CPH.GetTwitchUserVar<int>(userName, "pollVote", true);
                    CPH.LogInfo($"{userName}: {pollVote}");
                    voteCount++;
                    voteSum = voteSum + pollVote;
                }
            }
        }
        if (voteCount == 0){
            CPH.LogInfo($"votes {voteCount}: no votes");
            CPH.SendMessage("No votes... sad.", true);
        } else {
            voteResult = (float)voteSum/(float)voteCount;
            CPH.LogInfo($"votes {voteCount}: {voteResult.ToString("N2")}");
            CPH.SendMessage($"Results are: {voteCount} votes, mean {voteResult.ToString("N2")}", true);
        }
        voteReset();
        return true;
    }

    public void voteReset(){
        CPH.LogInfo($"resetting votes");
        CPH.UnsetAllUsersVar("pollVote", true);
    }
}
