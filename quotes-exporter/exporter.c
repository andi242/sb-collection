using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    public bool Execute()
    {
        var quoteExportFile = args["exportFilePath"].ToString();
        CPH.LogInfo("quotes export startet nach: " + quoteExportFile);

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        JToken quotesJson = JToken.Parse(File.ReadAllText(baseDirectory + @"/data/quotes.dat"));
        JArray quotesJArray = (JArray)quotesJson["quotes"];

        try
        {
            File.Create(quoteExportFile).Close(); // clear file
            foreach (JToken result in quotesJArray)
            {   
                // available json fields per quote (SB v0.2.1)
                // "timestamp", "id", "userId", "user", "platform", "gameId", "gameName", "quote"
                string quoteDate = result["timestamp"].ToString();
                // extended export
                //string line = $"{result["id"]}: {result["quote"]} ({result["user"]} {result["gameName"]}, {quoteDate})";
                // std export
                string line = $"{result["id"]}: {result["quote"]} ({result["user"]})";
                using (var fs = new FileStream(quoteExportFile, FileMode.Append))
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(line);
                }
            }
        }
        catch (Exception ex)
        {
            CPH.LogInfo(ex.Message);
            CPH.LogInfo("err");
        }
        CPH.LogInfo("quotes export abgeschlossen");
        return true;
    }
}