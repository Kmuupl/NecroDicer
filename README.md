# NecroDicer

**NecroDicer** is a turn-based roguelite dungeon crawler built in Unity, focused on a dice-driven combat system.

Instead of direct attacks, the player manipulates dice to influence combat outcomes, armor interactions, and damage resolution.

---

## 🎮 Gameplay Overview

The core gameplay loop is built around tactical dice usage:

- Draw dice from a bag into the tray
- Place dice onto the battlefield
- Assign dice to enemy armor slots
- Resolve damage based on slot conditions and damage curves
- End turn → enemy turn → repeat

Each decision affects the outcome, making combat more about planning than reflex.

## 🎯 Project Goal

This project focuses on building scalable and maintainable gameplay systems rather than delivering a fully polished game.

It is an actively developed project, where the main goal is to explore system design in Unity, including modular architecture, event-driven communication, and data-driven mechanics.

## 🎥 Gameplay Demo

<video src="./docs/gameplay.mp4" controls width="600"></video>

---

## ⚙️ Core Systems

### 🎲 Dice System
- Multi-stage flow: **bag → pool → tray → battlefield → discard**
- Player selects and places dice strategically
- Dice persist between turns (tray system)

### 🛡️ Armor System
- Slot-based armor with conditions
- Each slot reacts differently depending on inserted dice
- Damage calculated using customizable curves

### ⚔️ Combat System
- State-machine driven battle flow:
  - StartBattle → PlayerTurn → EnemyTurn → EndBattle
- Clear turn structure and separation of responsibilities

### 📦 Data-Driven Design
- Uses **ScriptableObjects** for:
  - Enemy data
  - Armor configuration
- Separates data from logic for scalability

### 🔌 Event-Driven Architecture
- UI does not directly control game logic
- Systems communicate through events managed by `BattleManager`

---

## 🧱 Architecture

The project follows a modular structure:

Assets/Scripts/
├── Gameplay/ # Core entities (Enemy, Armor, Dice, Player)
├── Systems/ # Game logic (CombatSystem, DiceBag)
├── Core/ # Managers (BattleManager, GameManager)
└── UI/ # Interface (Battle UI, Panels, Views)


Key design principles:
- Separation of concerns (logic vs data vs UI)
- Modular and extendable systems
- Focus on maintainability and scalability

---

## 🧪 Current State

### ✅ Implemented
- Full combat loop (player turn / enemy turn)
- Dice management system
- Armor with slot logic and damage curves
- Basic battle UI (buttons, HP bars, interaction)
- Event-driven system communication

### 🚧 In Progress
- Enemy AI (dice usage)
- Player armor system expansion
- Multiple enemies in combat

---

## 🛠️ Tech Stack

- Unity (2D)
- C#
- ScriptableObjects
- Git / GitHub
---

## 💡 What I Learned

- Designing modular gameplay systems
- Implementing state machines for game flow
- Building event-driven architecture in Unity
- Separating data and logic using ScriptableObjects

---

## 🚀 Future Plans

- Expand combat system with enemy AI
- Add more dice types and interactions
- Introduce progression and inventory systems