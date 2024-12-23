# Components
Components representing hardware or data processing functions for use with **[Sardine](https://github.com/orger-lab/sardine)**.

## Components
### Constructs
| Project Name | NuGet  | Description |
| :---------------- | :------: | :----: |
|         |   ❌  |  |
### Data Processes
| Project Name | NuGet  | Description |
| :---------------- | :------: | :----: |
|         |   ❌  |  |
### Devices
| Project Name | NuGet  | Description |
| :---------------- | :------: | :----: |
|         |   ❌  |  |
### Utils
| Project Name | NuGet  | Description |
| :---------------- | :------: | :----: |
|         |   ❌  |  |
### Visualizers
| Project Name | NuGet  | Description |
| :---------------- | :------: | :----: |
|         |   ❌  |  |

## Quick Start

### Larval zebrafish tail tracker
Operating System requirements: Windows 10 version 1607 or above
This example shows live tracking of a video recording of the tail of a head-restrained larval zebrafish, collected at 700 fps and played back. 
To accomplish the tracking, OpenCV (through the  is used to process the video images. 

1. Clone this repository
```
git clone https://github.com/orger-lab/sardine-components
```
2. Obtain the openCV v2.4.13.6 runtime (download available [here](https://sourceforge.net/projects/opencvlibrary/files/opencv-win/2.4.13/opencv-2.4.13.6-vc14.exe/download)
3. Extract the downloaded files, and copy the runtime libraries (contained in 'opencv\build\x64\vc14\bin') to the folder 'samples\FictiveFishTrackerExampleApp\opencv'
2. Open `Sardine-Components.sln` in Visual Studio
3. Select `FictiveFishTrackerExampleApp` as the startup project
4. Build and run
