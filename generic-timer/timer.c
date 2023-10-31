using System;
using System.Timers;
using System.Net;

class CPHInline
{
    public System.Timers.Timer timerTimer;
    public int timerSeconds;
    public string statusText;
    public string errorMessage = "Error, use command like: !timer pause,10";
    public string dlSound = "https://www.myinstants.com/media/sounds/the_witcher_3_quests_completed_sound_2wq1RSS.mp3";
    public string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    public void Init(){
        timerTimer = new System.Timers.Timer(1000);
        timerTimer.Elapsed += OnTimedEvent;
        timerTimer.AutoReset = true;
        timerTimer.Enabled = true;
        timerTimer.Stop();
    }
    // setup obs sources    
    public bool obsSetup(){
        // set some global vars from arguments
        try {
            CPH.SetGlobalVar("timerSound", args["finishSound"].ToString(), true);
        }
        catch(Exception ex) {
            CPH.LogInfo("option not found, downloading sound.");
            using (var client = new WebClient()) {
                // download soundalert file to streamerbot directory
                client.DownloadFile(dlSound, baseDirectory + "timerAlarm.mp3");
            }
            CPH.SetGlobalVar("timerSound", baseDirectory + "timerAlarm.mp3", true);
        }
        CPH.SetGlobalVar("timerObsScene", args["obsScene"].ToString(), true);
        CPH.SetGlobalVar("timerObsText", args["obsText"].ToString(), true);
        CPH.SetGlobalVar("timerEndeText", args["timerEnde"].ToString(), true);
        // create scene, skipped if exists
        CPH.ObsSendRaw("CreateScene", "{\"sceneName\":\""+args["obsScene"].ToString()+"\"}", 0);
        // create text source in obs, skipped if exists
        CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\""+args["obsScene"].ToString()+"\",\"inputName\":\""+args["obsText"].ToString()+"\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":{\"align\": \"center\",\"valign\": \"center\",\"text\":\"TIMER\", \"bk_color\": 4286513237, \"bk_opacity\": 50, \"color\": 4278190080, \"font\": { \"size\": 512,\"face\": \"Calibri\", \"style\": \"Regular\" }},\"sceneItemEnabled\":true}", 0);
        CPH.LogInfo("settings saved.");
        return true;
    }

    public void OnTimedEvent(Object source, ElapsedEventArgs e){
        timerSeconds--;
        TimeSpan time = TimeSpan.FromSeconds(timerSeconds);
        string cmd = args["command"].ToString().Replace("!", string.Empty);
        string rawInput = args["rawInput"].ToString();
        string[] options = rawInput.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var statusText = options[0];
        string countdownString = string.Format("{2} | {0}:{1}", (int)time.TotalMinutes, time.ToString("ss"), statusText);
        if (timerSeconds == 0){
            stopTimer(CPH.GetGlobalVar<string>("timerEndeText", true));
            CPH.PlaySound(CPH.GetGlobalVar<string>("timerSound", true), 0.5, true);
        } else {
            CPH.ObsSetGdiText(CPH.GetGlobalVar<string>("timerObsScene", true), CPH.GetGlobalVar<string>("timerObsText", true), countdownString);
        }
    }

    public bool startTimer(){
        string rawInput = args["rawInput"].ToString();
        string[] options = rawInput.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (options.Length != 2){
            CPH.SendMessage(errorMessage, true);
            return false;
        }
        try {
            double timerMinutes = Convert.ToDouble(options[1]);
            CPH.SetGlobalVar("timerDefaultMinutes", timerMinutes, true);
            timerSeconds = Convert.ToInt32(Math.Floor((timerMinutes * 60) + 1));
        }
        catch(Exception ex) {
            CPH.SendMessage(errorMessage, true);
            return false;
        }
        timerTimer.Start();
        return true;
    }

    public bool stopTimer(string text){
        CPH.ObsSetGdiText(CPH.GetGlobalVar<string>("timerObsScene", true), CPH.GetGlobalVar<string>("timerObsText", true), text);
        timerTimer.Stop();
        return true;
    }

    public bool cancelTimer(){
        stopTimer(CPH.GetGlobalVar<string>("timerEndeText", true));
        return true;
    }
}