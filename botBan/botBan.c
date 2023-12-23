using System;
using System.Collections.Generic;

class CPHInline
{
    
    public string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    // setup obs sources    
    public bool checkMessage(){
        string msg = args["rawInput"].ToString().ToLower();
        string userName = args["userName"].ToString();
        List<string> blockList = CPH.GetGlobalVar<List<string>>("blockList", true);
        foreach (var entry in blockList) {
            CPH.LogInfo($"{entry}");
            if(msg.Contains(entry.ToLower())){
                CPH.TwitchTimeoutUser(userName, 30, "blacklisted chatting.", true);
                CPH.TwitchBanUser(userName, "blacklisted chatting.", true);
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
        // alle ausgeben
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
