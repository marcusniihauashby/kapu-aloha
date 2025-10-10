# Kapu Aloha - Published Puzzle Adventure Game
### A 2D puzzle-adventure game that leverages optical illusions and unique inventory mechanics to tell a culturally-rich story.

[Link to the Game](https://marcusna.itch.io/kapu-aloha)

The image below is a hyperlink to a 3-minute demo video, check it out if you would like.

[![Kapu Aloha Thumbnail](http://img.youtube.com/vi/2AfgTELR-6o/0.jpg)](http://www.youtube.com/watch?v=2AfgTELR-6o "Video Title")

### About The Project
As the sole programmer on a 6-person team, I engineered Kapu Aloha from concept to publication. The project required me to quickly master C# and the Unity Engine to build a robust and performant game. 
The core challenge was designing systems that were not only technically sound but also intuitive for my non-technical teammates (artists, narrative designers) to use.

This project showcases my ability to take complete ownership of a technical product, collaborate with a multidisciplinary team, and deliver a polished final product under a deadline.

### Key Features

Seamless Teleportation System: Engineered an infinitely looping forest by precisely managing player and camera transforms to create a seamless infinite hallway effect.

Dynamic Inventory System: Implemented an inventory that directly alters the environment. This was exclusively done for ambient/environmental storytelling purposes.

Multi-State Enemy AI: Designed and coded intelligent enemy behaviors with distinct states (patrol, chase, attack), modularizing the system for organizational purposes.

Optimized 2D Rendering: Modeled 2D sprite-based assets in Blender and implemented them to ensure high-performance rendering, especially given this game was planned to run in a web browser.

### Built With
Game Engine: Unity

Programming Language: C#

3D Modeling: Blender

## Challenges & Learnings

### Challenge 1 (Seamless Teleportation): 

My goal was to create an [antichamber-esque looping mechanic](https://www.youtube.com/watch?v=7ovV7Uc8OtA) within the game, which permanently traps you within the forest until you solve the puzzle.
This required a technically challenging sleight-of-hand: seamlessly teleporting the first-person player while accounting for their position, destination, and camera angle, especially problematic 
given my game's complex lighting and tree layouts (unlike Antichamber's shadowless environment).

The solution I had to the first problem came with several moving parts.
The smallest discrepancy in teleportation is felt significantly by the player, as the player POV is in first-person. Thus, I started with normalizing all of the hallway lengths, which simplified 
level building and precise teleportation to integer coordinates. This also allowed me to modularly create the tree layouts of the levels such that they wouldn't seem to move around when the player teleports.
Additionally, I created a script that always tracked the player camera's orientation such that we can apply inverse camera rotations when going through specific teleports, ensuring the orientation was consistent through teleports.
Crucially, but albeit not as glamorous, I made some basic by-hand calculations that helped me balance occluding the player's vision during the transition with maintaining a natural forest 
feel, making the teleportation truly invisible from any angle.


### Challenge 2 (Collaboration tools with non-coding teammates):

A core part of this project involved enabling non-technical teammates (narrative, sound, UI/UX designers) to contribute effectively without writing code, while also ensuring features were easily tweakable for rapid iteration on feedback.

I addressed this by developing two key modular systems:

Dialogue System: I created a custom Dialogue Entry object in Unity (essentially an N-sized array of tuples) that allowed our narrative designer to simply create the object, fill in dialogue, 
and drag-and-drop it into trigger zones within Unity. This eliminated the need for coding and streamlined content integration.

Sound Manager: For sound, I built a robust sound manager that supports global, proximal (coordinate-offset), and static sounds. I provided simple functions with documented boilerplate at key trigger points 
(e.g., "Death Sound," "Jumpscare Sound"), allowing our sound designer to simply drag-and-drop their edited audio files into the relevant slots, significantly expediting their workflow.
