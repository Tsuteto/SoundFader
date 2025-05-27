# Sound Fader
![](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FTsuteto%2FSoundFader%2Frefs%2Fheads%2Fmaster%2Fmanifest.json&query=Version&label=Version&color=green)
[![Elgato Marketplace](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fmp-gateway.elgato.com%2Forganizations%2Fffa54d68-c821-41b7-a4ad-30f1b26859f6%2Fproducts%2F73e7ba2a-09ec-43ac-8e1d-1cc71a3f721d&query=download_count&prefix=Elgato%20Marketplace%20%7C%20&suffix=%20downloads&logo=elgato&logoColor=white&label=%20&labelColor=1231ac&color=gray)](https://marketplace.elgato.com/product/sound-fader-73e7ba2a-09ec-43ac-8e1d-1cc71a3f721d)  
Allows you to fade in/out the sound of apps and sound devices with a single touch. This was created as a solution for my friend who was in trouble, and honestly I don't know what it's used for 😅, but I guess it could at least be useful for smooth scene transitions in your livestream.

Features:
- Toggle action: Fade out first, fade in second and resume previous volume.
- Fade out action: Always fade out to volume 0.
- Fade in action: Always fade in up to the volume you set. Be aware of the volume, especially on devices.
- Adjustable duration: From 0.1 to 10 seconds.
- When fading in, the target volume after the effect is displayed on the key to avoid an unexpected increase in volume.
- Bending: Tweaks the fading progress to make it smoother with these two types that have different sound remnants:
  - Exponential - Soft start, tight finish, and better for short duration
  - Logarithmic - Tight start, soft finish, and better for long duration

## Distribution
Elgato Marketplace  
https://marketplace.elgato.com/product/sound-fader-73e7ba2a-09ec-43ac-8e1d-1cc71a3f721d

## Dependencies
Only works on Windows
- Stream Deck
- Stream Deck SDK
- .NET Framework SDK
- StreamDeckLib for C#
- NAudio

See the .csproj file for details.

## License
MIT