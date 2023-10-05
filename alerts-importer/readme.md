# Alerts Importer

Import media files from a directory into a scene in OBS.

SB action options:
- `alertScene`: OBS scene media sources get imported to
- `soundsDir`: local directory to import sound files from (supported formats: `"*.mp3", "*.wav", "*.aac", "*.ogg"`)
- `videosDir`: local directory to import video files from (supported formats: `"*.mp4", "*.mov", "*.avi", "*.mkv"`)

Sources will be named after the file name that is being imported (without file extension).  
Sources in OBS need to be unique, so if you already have a source with a specific name, this will be overwritten.
