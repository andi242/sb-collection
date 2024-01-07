using System;

public class CPHInline
{
	public bool Execute()
	{
        string userName = args["userName"].ToString();
        CPH.LogInfo($"yawn logged {userName.ToString()}");

        int userYawnCounter = 0;
        userYawnCounter = CPH.GetTwitchUserVar<int>(userName, "yawnCounter", true);
        int yawnGlobalCount = 0;
        yawnGlobalCount = CPH.GetGlobalVar<int>("yawnGlobalCount", true);
        int yawnStreamCount = 0;
        yawnStreamCount = CPH.GetGlobalVar<int>("yawnStreamCount", true);

        yawnGlobalCount++;
        yawnStreamCount++;
        userYawnCounter++;

        CPH.SetTwitchUserVar(userName, "yawnCounter", userYawnCounter, true);
        CPH.SetGlobalVar("yawnGlobalCount", yawnGlobalCount, true);
        CPH.SetGlobalVar("yawnStreamCount", yawnStreamCount, true);

        CPH.SetArgument("userYawnCounter", userYawnCounter);
        CPH.SetArgument("yawnGlobalCount", yawnGlobalCount);
        CPH.SetArgument("yawnStreamCount", yawnStreamCount);

		return true;
	}
	public bool yawnReset()
	{
        // reset var in global stream count on stream start
        CPH.LogInfo($"reset yawn counter.");
        CPH.SetGlobalVar("yawnStreamCount", 0, true);
		return true;
	}
}
