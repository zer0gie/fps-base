## FPS Base

FPS Base - a simple FPS game example using UniTask, Animancer, DoTween and Zenject. This example provides fundamental mechanics for creating an FPS game, including character control, shooting, physics, and interactive objects.

**NOTE:** This projevt made as a test, so the project has no best practices and must be used only as an example, not as a guide.

<img src="https://github.com/user-attachments/assets/19aeff66-8e12-4cc4-9c02-230eb2c74b5e" width="800">

## 🚀 Features
- First-person character control
- Shooting, reloading, and weapon switching
- Character movement and physics
- Interactive demo scene with various targets
- Unity New Input System based keyboard and gamepad input support 🎮
- Finite State Machine pattern for weapon logic

## 📚 Libraries
- **UniTask** - for asynchronous operations, including FSM pattern implementation for weapon mechanics
- **Animancer** - animation management with UniTask-based completion handling
- **DoTween** - smooth animations for objects and UI
- **Zenject** - dependency injection, factories, and signals

## 🛠️ Launch
1. **Clone the repository:**
   ```sh
   git clone https://github.com/zer0gie/fps-base.git
   ```
2. **Open the project** (recommended version Unity 6+).
3. **Install dependencies** via Unity Package Manager.
4. **Run the `SampleScene`** to test game mechanics.
5. **To create a new weapon:** add the `Weapon` script to the prepared model and configure scriptable `WeaponConfig`.
   
<img src="https://github.com/user-attachments/assets/0419838e-2bc7-44c1-9f2d-9e19bfdf1b6f" width="800">

