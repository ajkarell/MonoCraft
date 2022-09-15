# MonoCraft

A modest Minecraft clone built with MonoGame (C#, XNA) with focus on world generation.

Made with the intent to learn low-level game development, 3D graphics and procedural world generation.

## Building

The project relies on DirectX for shaders, due to limitations in the OpenGL version that MonoGame supports. This means that building is for **Windows only (or possibly [through Wine on Linux](https://docs.monogame.net/articles/getting_started/1_setting_up_your_development_environment_ubuntu.html#optional-set-up-wine-for-effect-compilation))**.

* Clone the project  
```git clone https://github.com/ajkarell/MonoCraft.git```
* Change into the project directory  
```cd MonoCraft```
* Build and run the project  
```dotnet run```
