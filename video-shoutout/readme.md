# video shoutout

## features

- `!vsosetup`: creates a new scene with all items for the video shoutout. can be triggered manually right click -> test trigger.
- `!vso <streamername>`: starts a video shoutout for `streamername`
- `brb player`: on activation of a scene the player can loop clips from a list of streamers. Increase `loops` for x times the clips, if the list is a bit shorter.

## install

- import to streamerbot
- check settings in `triggerAction`
  - `triggerScene`: this is your brb scene to host the clip player
  - `streamers`: comma separated list of streamers
  - `loops`: loop the list of `streamers` x times
- check the new scene and customise design, best done when a clip is playing
- add the new scene to your previously existing scenes as a nested scene

## more

- no html file required
- queue clip play from a loop into blocking action queue on custom trigger
- queue gets cleared on scene change, if scene is not whitelisted
- currently playing clip is being shut down on scene change (no more clips/sounds running in background)
- maintains a list of already played clips to prevent more of just the same clips
