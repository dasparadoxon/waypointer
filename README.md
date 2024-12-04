 ![WayPointerDokuLogoHeader](https://github.com/user-attachments/assets/51885a52-c6e9-481a-b3cb-6ea8f4d6d49f)

 # About

Waypointer is a visual interactive waypoints system for Unity3D in the editor. 

Example usage is for NPC walking or traffic networks. 

![scene - example network](https://github.com/user-attachments/assets/bf0e4b88-8d6a-4809-ae76-9ef762c742f8)

Waypoints can have one or more connections to other waypoints.

They also have status flags for being exit or entry points, recognizable by icons. This can be used for spawing and despawning objects automatically.

The main object is the waypoint network. 

You can create new waypoints by clicking the button "Add new waypoint" and then click 
into the scene where you want to place the new waypoint. (for this to work the object on which to place the waypoint has to have some sort of collider on it)

Waypoints are child objects of the waypoint network. 

By selecting a waypoint you can change the entry/exit point flags, set a placement variation radius to use for create a variation for npcs. 

You also see to which waypoints the waypoint connects to and set new one way, new two way connections to other waypoints. You can also add a new connected waypoint that is connected to this waypoint.

