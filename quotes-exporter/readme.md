# Quotes Exporter

If you are using quotes a lot and want to export them to a list for viewers to check them out, this does exactly that.

SB action options:
- `exportFilePath`: file (full path) to export the quotes to

Recommended:
- Set the export file in a directory that is synced to a file sharing service, like NextCloud or Google Drive.
- create a `OBS > OBS started streaming` trigger to the action to refresh the list at stream start  
  (streamer.bot writes the quotes list at shutdown, so if quotes are added while SB is running, those won't be in quotes.dat, yet)
- create a chat command (e.g. `!quoteslist`) and have it send a message to chat with the link to the shared file (e.g. `drive.google.com/.../...`)
