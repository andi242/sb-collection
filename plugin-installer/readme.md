# [Work in Progress] download OBS plugins with streamer bot

## disclaimer 

> [!IMPORTANT]  
> running this will close all running OBS instances!  
> do not do this on stream!  
> Use at own risk. Do backups.  

Should work with installed OBS in c:\program files, but was only tested with a portable_mode OBS.  
Use at own risk.  
If you fear ruining your OBS install do not use this. You have been warned. At least do a backup (you should anyways).

## how to use

- create a `obs-plugins.json` in your obs folder (see [example file](./obs-plugins.json))
- Entries in the .json file can be moved in the sections to `install`, `uninstall` or `no action`. 
- add new lines to json file in the same format as the existing lines.
- Set the variables in the SB action

### add a new (or existing) plugin

1. add a new entry to `install` array in json file. 
1. run the SB action
1. two explorer folders should be popping up (obs install folder and downloaded plugins folder)
1. (!) check the contents of the plugins folder, arrange to obs folder structure (`plugin-install-logs`, `obs-plugins` and `data` folders)
1. drag and drop to the obs install folder

If you added an existing plugin files will be overwritten.  
If you updated or uninstalled a plugin there should be no prompt to overwrite files (if you used the installer before).

## what it does

- close all (!) running OBS
- check the `obs-plugins.json` in `$OBS_ROOT_FOLDER`
- delete existing `$OBS_ROOT_FOLDER\plugin-downloads`
- download all files and extract them to `$OBS_ROOT_FOLDER\plugin-downloads`
- create logfiles for all downloaded plugins in `$OBS_ROOT_FOLDER\plugin-downloads\plugin-install-logs`
- uninstall a plugin from your OBS if there is a logfile present in `$OBS_ROOT_FOLDER\plugin-install-logs`
- after all is done, open `$OBS_ROOT_FOLDER\` and `$OBS_ROOT_FOLDER\plugin-downloads` in explorer

all you need to do is double check the `$OBS_ROOT_FOLDER\plugin-downloads`, perhaps sort files and copy all the content of `$OBS_ROOT_FOLDER\plugin-downloads` folder to `$OBS_ROOT_FOLDER\`.

> [!IMPORTANT]  
> always check the streamerbot logs for information.

## what it does not

- check for latest releases (this is too tedious for now. I do not want to curate a list of plugins since some host their files on github, some in the obs forums... some to tags, some do releases...)
