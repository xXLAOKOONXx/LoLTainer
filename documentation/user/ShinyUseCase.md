# Shiny Use Case (Step-by-Step)

## Start App

1. Download the recent version from [Releases](https://github.com/xXLaokoonXx/LoLTainer/releases).
2. Extract the package wherever you want. (But take care the folder does not get limited access rights.)
3. Start the Executable.

## Add Mappings
(Mapping means mapping between event and sound file)  
![Main Window](https://github.com/xXLaokoonXx/LoLTainer/blob/master/documentation/img/MainWindow_NoSettings.png?raw=true)  
On the main window you can click on "Add Mapping" to add a new mapping.  
![Add Mapping](https://github.com/xXLaokoonXx/LoLTainer/blob/master/documentation/img/AddSettingWindow_Blank.png?raw=true)  
On the left side select side select your event, on the right side you can click on the "Select File" button and select your sound file (WAV).  
After selecting both you can add the mapping and it will be active in the next game.  
The soundfile need to be longer than 10 seconds, while the first 10 seconds will be played when triggered.

## Change Mapping
Changing a Mapping is not available yet. You can use the delete button and add a new mapping for the event.  
To get this option first click onto the event you would like to change.  
![Extended Main Window](https://github.com/xXLaokoonXx/LoLTainer/blob/master/documentation/img/MainWindow_Collapsed.png?raw=true)

## App not starting
If the App does not start this might be caused by an issue with the saved settings. Go into the apps folder and search for a file named "soundsettings.lt".

## Feature requests and bugs
[Link to issues](https://github.com/xXLaokoonXx/LoLTainer/issues)  
I love any kind of feedback <3
### More events
Please enter an issue with an explaination for the event.  
Example: DoubleKill  
When the active player scores a doublekill in the game. The timespan between both kills is 5 seconds or lower.  
  
Please be aware that the implementation of the event is limited by the data provided by riot apis.
### Feature requests
Please enter an issue with a detailed description on what you would love to do.
### Bug report
Please enter an issue with a detailed report on what you did and what then happened.