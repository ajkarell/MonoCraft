# MonoCraft

A modest Minecraft clone built with MonoGame (C#, XNA) with the focus on world generation.

Made with the intent to learn low-level game development (no game engine), 3D graphics and procedural world generation.

## Building

Building is for **Windows-only**, because the game relies on Texture Arrays, which MonoGame's OpenGL version does not yet support.

* Clone the project  
```git clone https://github.com/ajkarell/MonoCraft.git```
* Change into the project directory  
```cd MonoCraft```
* Build and run the project  
```dotnet run```