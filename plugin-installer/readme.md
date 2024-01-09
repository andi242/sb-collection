# [Work in Progress] Download Plugins with streamer bot

## disclaimer 

> WARNING!  
>
> running this will close all running OBS completely!  
> do not do this on stream!

Should work with installed OBS in c:\program files, but was only tested with a portable_mode OBS.  
Use at own risk.  
If you fear ruining your OBS install do not use this. You have been warned. At least do a backup (you should anyways).

## how to use

- create a `plugins.txt` in your obs folder (see [example file](./plugins.txt))
- the file has one plugin with `name` and `download link` per line `;` is the delimiter 
- lines beginning with `#` uninstalls the plugin in that line (if it was installed with this action)
    - remove line after uninstall

## what it does

- close all (!) running OBS
- check the `plugins.txt` in `$OBS_ROOT_FOLDER`
- delete existing `$OBS_ROOT_FOLDER\plugin-downloads`
- download all files and extract them to `$OBS_ROOT_FOLDER\plugin-downloads`
- create logfiles for all downloaded plugins in `$OBS_ROOT_FOLDER\plugin-downloads\plugin-install-logs`
- uninstall a plugin from your OBS if there is a logfile present in `$OBS_ROOT_FOLDER\plugin-install-logs`
- after all is done, open `$OBS_ROOT_FOLDER\` and `$OBS_ROOT_FOLDER\plugin-downloads` in explorer

all you need to do is double check the `$OBS_ROOT_FOLDER\plugin-downloads`, perhaps sort files and copy all (!) the content of `$OBS_ROOT_FOLDER\plugin-downloads` folder to `$OBS_ROOT_FOLDER\`.

> always check the streamerbot logs for information.

## todo

- zipfiles containing subfolders are logged correctly, but files should be moved within the downloads folder. how, c#? how!?