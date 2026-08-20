# EffectParticles

Visual particles for player status effects, inspired by Minecraft.  
When a player has an active status effect, colored particles appear around them, corresponding to the specific effect type.

<img width="1280" height="720" alt="plugin_showcase" src="https://github.com/user-attachments/assets/b291f106-f3b6-4ddf-a07e-155124866d9b" />

## ✨ Features

- 🎨 **Color Coding** — Each effect has its own unique color.
- 🎯 **Color Blending** — When multiple effects are active, particle colors blend.
- ⚡ **High Performance** — Uses object pooling; no runtime GameObject creation or destruction.
- ⚙️ **Customizable** — Configure colors, spawn frequency, lifetime, and flight arc via config.
- 👁️ **Player Settings** — Players can hide their own or other players' particles via Server-Specific Settings.

## 📋 Requirements

- LabAPI 1.1.7 (included by default on most servers)
- Harmony 2.4.2 (.NET 4.8 version)

## 📥 Installation

1. Download the latest release from [Releases](https://github.com/Aphin4/EffectParticles/releases/latest).
2. Download [Harmony](https://github.com/pardeike/Harmony/releases/latest/) (get the **Fat** zip version).
3. Place `EffectParticles.dll` in your server's plugin folder:  
   `~/AppData/Roaming/SCP Secret Laboratory/LabAPI/plugins/global/`  
   *(Or the specific port folder if you run multiple instances)*.
4. Extract `0Harmony.dll` from the `Harmony-Fat.zip/net48` folder and place it in:  
   `~/AppData/Roaming/SCP Secret Laboratory/LabAPI/dependencies/global/`  
   *(Or the specific port folder)*.
5. Restart the server and configure the plugin if needed.

## ⚙️ Configuration

After the first launch, a `config.yml` file will be generated in your server's configuration folder:

```yaml
# Text for the Enabled setting
s_s_setting_enabled_string: On

# Text for the Disabled setting
s_s_setting_disabled_string: Off

# Text for the setting responsible for the own particles
s_s_setting_own_particles_string: Show own particles

# Text for the setting responsible for the all particles
s_s_setting_all_particles_string: Show all particles

# Colors for each effect, use only the original names of the effect classes
colors:
  Scp1853: '#FF00FF80'
  Invigorated: '#00FF0080'
  RainbowTaste: '#FF69B480'
  # ... and many more effects

# Lifetime for every particle in seconds
lifetime: 2

# How many particles spawning per second
particles_per_second: 3

# Modifier for the particle motion curve
arc_height: 0.2

# Type of particles
type_particles: SphereParticles
```

### Configuration Notes

- **Effect Names:** Effect names correspond to the actual class names in the server assembly. The default config includes valid names.
- **Visibility Logic:** If a player disables "Show all particles" in Server-Specific Settings, they will also stop seeing their own particles.
- **Particle Types:** The particle style is determined by the class name within the plugin.
- **Default Colors:** If a color is missing from the config for a specific effect, a default color will be assigned based on its category (Positive, Negative, Mixed, Technical).
- **Invisibility:** The Invisibility effect makes the particles invisible as well.

## 🧑‍💻 For Developers

You can create custom particle styles by creating a class that inherits from `ParticleController`.

```csharp
public class CustomParticles : ParticleController
{
    // You must override the method for creating a particle instance
    protected override GameObject CreateParticle()
    {
      // Your logic for creating the GameObject
      return GameObject;
    }

    // You must also override the method to recolor all particles in the pool
    internal override void Recolor(Color color)
    {
        if (color == null)
            color = Color.white; // Fallback in case color is missing

        foreach(var go in _pool)
        {
          // Apply the color to each GameObject in the pool
          // Example: go.GetComponent<Renderer>().material.color = color;
        }
    }
}
```

After creating your class and compiling it into a DLL, place your plugin DLL in the plugins folder and change the `type_particles` value in the config to the name of your new class.
