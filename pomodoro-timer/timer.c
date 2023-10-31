using System;
using System.Timers;

class CPHInline
{
    public System.Timers.Timer pomoTimer;
    public int pomoSeconds;
    public string statusText;
    public string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    public void Init(){
        pomoTimer = new System.Timers.Timer(1000);
        pomoTimer.Elapsed += OnTimedEvent;
        pomoTimer.AutoReset = true;
        pomoTimer.Enabled = true;
        pomoTimer.Stop();
    }
    // setup obs sources    
    public bool obsSetup(){
        // set some global vars from arguments
        CPH.SetGlobalVar("pomoSound", args["finishSound"].ToString(), true);
        CPH.SetGlobalVar("pomoObsScene", args["obsScene"].ToString(), true);
        CPH.SetGlobalVar("pomoObsText", args["obsText"].ToString(), true);
        CPH.SetGlobalVar("pomoEndeText", args["pomoEnde"].ToString(), true);
        CPH.SetGlobalVar("pomoWorkText", args["workText"].ToString(), true);
        CPH.SetGlobalVar("pomoPauseText", args["pauseText"].ToString(), true);
        // create scene, skipped if exists
        CPH.ObsSendRaw("CreateScene", "{\"sceneName\":\""+args["obsScene"].ToString()+"\"}", 0);
        // create text source in obs, skipped if exists
        CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\""+args["obsScene"].ToString()+"\",\"inputName\":\""+args["obsText"].ToString()+"\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":{\"align\": \"center\",\"valign\": \"center\",\"text\":\"TIMER\", \"bk_color\": 4286513237, \"bk_opacity\": 50, \"color\": 4278190080, \"font\": { \"size\": 512,\"face\": \"Calibri\", \"style\": \"Regular\" }},\"sceneItemEnabled\":true}", 0);
        CPH.LogInfo("settings set.");
        return true;
    }

    public void OnTimedEvent(Object source, ElapsedEventArgs e){
        pomoSeconds--;
        TimeSpan time = TimeSpan.FromSeconds(pomoSeconds);
        string cmd = args["command"].ToString().Replace("!", string.Empty);
        switch (cmd) {
            case "work":
                statusText = CPH.GetGlobalVar<string>("pomoWorkText", true);
                break;
            case "pause":
                statusText = CPH.GetGlobalVar<string>("pomoPauseText", true);
                break;
            default:
                statusText = "";
                break;
        }
        string countdownString = string.Format("{2} {0}:{1}", (int)time.TotalMinutes, time.ToString("ss"), statusText);
        if (pomoSeconds == 0){
            stopTimer(CPH.GetGlobalVar<string>("pomoEndeText", true));
            CPH.PlaySound(CPH.GetGlobalVar<string>("pomoSound", true), 50, true);
        } else {
            CPH.ObsSetGdiText(CPH.GetGlobalVar<string>("pomoObsScene", true), CPH.GetGlobalVar<string>("pomoObsText", true), countdownString);
        }
    }

    public bool startTimer(){
        double pomoMinutes = Convert.ToDouble(args["rawInput"]);
        CPH.SetGlobalVar("pomoDefaultMinutes", pomoMinutes, true);
        pomoSeconds = Convert.ToInt32(Math.Floor((pomoMinutes * 60) + 1));
        pomoTimer.Start();
        return true;
    }

    public bool stopTimer(string text){
        CPH.ObsSetGdiText(CPH.GetGlobalVar<string>("pomoObsScene", true), CPH.GetGlobalVar<string>("pomoObsText", true), text);
        pomoTimer.Stop();
        return true;
    }

    public bool cancelTimer(){
        stopTimer(CPH.GetGlobalVar<string>("pomoEndeText", true));
        return true;
    }
}