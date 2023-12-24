using System;
using System.Collections.Generic;

class CPHInline
{
    public bool checkMessage(){
        string msg = args["rawInput"].ToString().ToLower();
        string userName = args["userName"].ToString();
        string reason = "blacklisted chatting.";
        List<string> blockList = CPH.GetGlobalVar<List<string>>("blockList", true);
        foreach (var entry in blockList) {
            CPH.LogInfo($"{entry} found in {msg}.");
            if(msg.Contains(entry.ToLower())){
                if(!CPH.UserInGroup(userName, args["friendliesGroup"].ToString())){
                    CPH.AddUserToGroup(userName, "blockedBots");
                    CPH.TwitchTimeoutUser(userName, 30, reason, true);
                    CPH.TwitchBanUser(userName, reason, true);
                    CPH.SendMessage($"/me {userName} banned for {reason}", true);
                }
            }
        }
        return true;
    }
    public string deListing(List<string> blockList){
    	string reply ="";
        blockList.ForEach(delegate(string element){
            reply = reply + $"\"{element}\" ";
        });
        return reply;
    }
    public bool removeMatch(){
        List<string> blockList = CPH.GetGlobalVar<List<string>>("blockList", true);
        string msg = args["rawInputEscaped"].ToString();
        string reply = "";
        if (msg == ""){
            reply = deListing(blockList);
            CPH.SendMessage($"/me currently blocking: {reply}", true);
        } else {
            blockList.Remove(msg);
            reply = deListing(blockList);
            CPH.SendMessage($"/me {msg} removed, {reply} remaining.", true);
            CPH.SetGlobalVar("blockList", blockList, true);
        }
        return true;
    }
    public bool addMatch(){
        List<string> blockList = CPH.GetGlobalVar<List<string>>("blockList", true);
        if (blockList == null){
            blockList = new List<string>();
        }
        string msg = args["rawInputEscaped"].ToString();
        if (!blockList.Contains(msg)){
            CPH.SendMessage($"/me {msg} added.", true);
            blockList.Add(msg);
        }
        CPH.SetGlobalVar("blockList", blockList, true);
        return true;
    }
}
