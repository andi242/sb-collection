# video shoutout

# works on my machine

not intended for importing, just a reference on what to do.

- clip is played in a nested scene (for scaling, texts, effects, etc.)
- queue clip play from a loop into blocking action queue on custom trigger
- queue gets cleared on scene change, if scene is not whitelisted
- currently playing clip is being shut down on scene play (no more clips/sounds running in background)
- maintains a list of already played clips to prevent more of just the same clips
- can be triggered from `!vso <user>` command

relies heavily on my local setup, please grab what you need to build your own.