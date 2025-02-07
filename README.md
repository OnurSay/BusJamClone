
# Bus Jam Clone

A case study that clones the hit game called "Bus Jam".

![Logo](https://i.postimg.cc/8Cg1PvBC/Bus-Jam-Clone-Logo.png)
## Screenshots

![Screenshot](https://i.postimg.cc/B6GG4G3P/SS-1.png)![Screenshot](https://i.postimg.cc/pVCtb76y/SS-2.png)![Screenshot](https://i.postimg.cc/tCgGX5Kk/SS-4.png)![Screenshot](https://i.postimg.cc/7hKv3G2S/SS-3.png)

  
## Requirements and Dependencies

Unity Version: 2022.3.19f1

It's useful to clear PlayerPref's for any case.

## Level Creator

### Usage

If a Level instance is present in the scene when it first opens, simply delete it. Next, go to the LevelCreator GameObject in the Hierarchy. You will see the LevelCreator script in the Inspector.

To create a new level, start by assigning values to the Level Index, Grid Width, and Grid Height input fields. Before assigning a Level Index, the level designer should check its value, as pressing the Reset button will erase all previous data associated with that index.

Once the values are set correctly, a blank grid will appear beneath the buttons. To design the level, select a color from the Color Types dropdown menu and click on the grid rectangles. To clear a rectangle, simply right-click on it.

For additional features, the designer can use the flags IsSecretStickman and IsReservedStickman, which are found under Color Types. These flags can be used individually or together.

To set the Level Timer, modify the Level Time variable in the Inspector. To define the Level Goals, add them to the corresponding list named Level Goals.

If everything is set up correctly and no errors appear under the grid, the Save button will become interactable.

#### IMPORTANT NOTES:

Before clicking the Save button, first use the Generate Grid button. After that, click Save.

To test the level, click the Test button.
After clicking Test, do not stop the editor manually. Instead, complete the level by either winning or losing. The editor will automatically return to the Level Creation Scene.

#### POSSIBLE BUG:
Occasionally, the Addressables package may become unresponsive (the exact cause is unknown, and previous attempts to fix it have been unsuccessful). If the Load button does not function properly, restart the Unity Editor and delete any remaining level instances.

## Copyright

Anyone wishing to use some or all of the materials in this project should consult the author. The consequences of unauthorized distribution concern the person or institution that carried out the action.

## Author

LinkedIn: https://www.linkedin.com/in/onursay/

E-Mail: onursay97@gmail.com

Onur Say, 2025


