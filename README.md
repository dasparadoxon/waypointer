 ![WayPointerDokuLogoHeader](https://github.com/user-attachments/assets/51885a52-c6e9-481a-b3cb-6ea8f4d6d49f)

 # About

Waypointer is a visual interactive waypoints system for Unity3D in the editor. 

Example usage is for NPC walking or traffic networks. 

![scene - example network](https://github.com/user-attachments/assets/bf0e4b88-8d6a-4809-ae76-9ef762c742f8)

Waypoints can have one or more connections to other waypoints.

They also have status flags for being exit or entry points, recognizable by icons. This can be used for spawing and despawning objects automatically.

The main object is the waypoint network. 

![inspector - waypoint network - smaller](https://github.com/user-attachments/assets/b1f3d208-d0b3-491d-8f5a-86bc529e4af8)

You can create new waypoints by clicking the button "Add new waypoint" and then click 
into the scene where you want to place the new waypoint. (for this to work the object on which to place the waypoint has to have some sort of collider on it)

Waypoints are child objects of the waypoint network. 

![inspector - waypoint - smaller](https://github.com/user-attachments/assets/34eccc9b-1e26-486a-bcf7-40a8207b156e)

By selecting a waypoint you can change the entry/exit point flags, set a placement variation radius to use for create a variation for npcs. 

You also see to which waypoints the waypoint connects to and set new one way, new two way connections to other waypoints. You can also add a new connected waypoint that is connected to this waypoint.

# Install

Right now it is a git package. So by using it in the package manager with the git url you can directly load it from github as package. You can also download the zip and unpack it into a directory in your project and use it as a embedded package. 

## Create a new waypoint network

Click on the hierarchy and select "Add  waypoint network" from the Waipointer submenu.

![hierarchy - add waypoint network context menu](https://github.com/user-attachments/assets/3fb2893e-7dad-4a4a-9147-24f5f4248b73)

## Remarks

This is still under development and has certain small bugs. But since I use it in all my projects that require this sort of logic be sure I will update it. 
Also the drawing was originally made with the excelent ALINE paid asset, but since this is a paid asset I had to rewrite
the editor draw methods new and they do not look as pretty as they did with ALINE. I will do a version that recognize if you ALINE soon, too.


