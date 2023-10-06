# Alerts Importer

Import media files from a directory into a scene in OBS.  
You can play media files from SB natively, but only audio. And you can't create SB actions from SB itself (afaik).

SB action options:
- `alertScene`: OBS scene media sources get imported to, will be created if not exists
- `soundsDir`: local directory to import sound files from (supported formats: `"*.mp3", "*.wav", "*.aac", "*.ogg"`)
- `videosDir`: local directory to import video files from (supported formats: `"*.mp4", "*.mov", "*.avi", "*.mkv"`)
- `scale`: scale video files to given value (e.g.: `0.4` = `40%` in size, `1` is `100%`)

Sources will be named after the file name that is being imported (without file extension).  
Sources in OBS need to be unique, so if you already have a source with a specific name, this will be skipped.

There is no reliable way to determine the actual size of a video file with OBS websocket in order to decide if scaling is appropriate or not.  
So every video file is scaled and positioned to setting, no matter what.  
Adjust sources to your liking after import.

## changelogdiarything

- create OBS scene if not exists
- added skip import if already exists so existing items will not be reset
- added scaling of video files and position on creation in OBS scene
- add video media import
- init with audio import
