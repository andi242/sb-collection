using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Newtonsoft.Json;

class CPHInline
{
    public string errorMessage = "Error, use command like: !vip user";
    public string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    public int vipDays;

    public bool checkUserVip(string userName){

        try {
            var vipDate = CPH.GetTwitchUserVar<string>(userName, "giftVipDate", true);
            DateTime now = DateTime.Now;
            var parsedVipDate = DateTime.Parse(vipDate);
            if ((now - parsedVipDate).TotalDays > 0){
                CPH.LogInfo($"VIP expired for {userName}");
                CPH.TwitchRemoveVip(userName);
                CPH.SendMessage($"VIP ist für {userName} ausgelaufen.",true);
                CPH.UnsetTwitchUserVar(userName, "giftVipDate");
            }
        } catch (Exception ex) {
        }
        return true;
    }
    public bool removeUserVip(){
        string userName = args["rawInput"].ToString();
        CPH.TwitchRemoveVip(userName);
        CPH.SendMessage($"VIP wurde für {userName} entfernt.",true);
        CPH.UnsetTwitchUserVar(userName, "giftVipDate");
        return true;
    }
    public bool setUserVip(){
        var days = args["days"].ToString();
        string userName = args["rawInput"].ToString();
        if (Int32.TryParse(days, out int vipDays)){
            CPH.LogInfo($"vipDays is {vipDays.ToString()}.");
        } else {
            CPH.LogInfo("vipDays could not be parsed, setting to 30.");
            vipDays = 30;
        }
        CPH.UnsetTwitchUserVar(userName, "giftVipDate");
        DateTime now = DateTime.Now;
        DateTime expireDate = now.AddDays(vipDays);
        CPH.SetTwitchUserVar(userName, "giftVipDate", expireDate, true);
        // check userVar
        var vipDate = CPH.GetTwitchUserVar<string>(userName, "giftVipDate", true);
        CPH.LogInfo($"{userName} is VIP until {vipDate.ToString()}");
        CPH.TwitchAddVip(userName);
        CPH.SendMessage($"{userName} ist jetzt für {vipDays} Tage VIP.",true);
        return true;
    }

    public bool checkUsers(){
        JToken usersJson = JToken.Parse(File.ReadAllText(baseDirectory + @"/data/users.dat"));
        JObject userNames = usersJson.Value<JObject>("users");
        foreach (KeyValuePair<string, JToken> result in userNames)
        {
            JObject commandArray = userNames.Value<JObject>(result.Key);
            var userName = commandArray["name"].ToString();
            if (commandArray["type"].ToString() == "twitch") {
                checkUserVip(userName);
            }
        }
        return true;
    }
}