 ![WayPointerDokuLogoHeader](https://github.com/user-attachments/assets/51885a52-c6e9-481a-b3cb-6ea8f4d6d49f)
 
Waypointer is a visual waypoints system for Unity3D. Example usage is for NPC or Traffic control. 
Waypoints can have one or more connections to other waypoints.
They also have status flags for being exit or entry points. This can be used for spawing and despawning objects automatically.
The main object is the waypoint network. You can create new waypoints by clicking the button "Add new waypoint" and then click 
into the scene where you want to place the new waypoint. (for this to work the object on which to place the waypoint has to have some sort of collider on it)
Waypoints are child objects of the waypoint network. 
By selecting a waypoint you can change the entry/exit point flags, set a placement variation radius to use for create a variation for npcs. You also see to which waypoints the waypoint connects to and set new one way, new two way connections to other waypoints. You can also add a new connected waypoint that is connected to this waypoint.

