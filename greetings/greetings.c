using System;

class CPHInline
{
    public void sendGreetings(string userName, string msgId){
        //string userName = args["userName"].ToString();
        string greetings = CPH.GetTwitchUserVar<string>(userName, "greetings", true);
        if( greetings != null){
            CPH.TwitchReplyToMessage($"{greetings}", msgId, true);
        }
    }
    public void removeGreetings(string userName, string msgId){
        //string userName = args["userName"].ToString();
        CPH.UnsetTwitchUserVar(userName, "greetings", true);
        CPH.TwitchReplyToMessage($"Begrüßung entfernt.", msgId, true);
    }

    public void setGreetings(string userName, string msg, string msgId){
        //string userName = args["userName"].ToString();
        // string msg = args["rawInput"].ToString();
        CPH.SetTwitchUserVar(userName, "greetings", msg, true);
        CPH.TwitchReplyToMessage($"deine neue Begrüßung: {msg}", msgId, true);
    }

    public bool Execute(){
        string trigger = args["triggerName"].ToString();
        string userName = args["userName"].ToString();
        string msg = args["rawInput"].ToString();
        string msgId = args["msgId"].ToString();
        string command = "";
        if ( trigger == "First Words"){
            sendGreetings(userName, msgId);
        } else {
            command = args["command"].ToString();
        }
        if ( command == "!greet set"){
            setGreetings(userName, msg, msgId);
        }
        if ( command == "!greet del"){
            removeGreetings(userName, msgId);
        }
        return true;
    }
}
