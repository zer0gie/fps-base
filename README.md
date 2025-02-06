### FPS Base

FPS Base - это базовый шаблон для разработки FPS-игр на Unity.

## 📌 Описание
Этот репозиторий содержит фундаментальные компоненты для создания шутера от первого лица (FPS). 
Проект предоставляет готовую архитектуру и базовые механики, которые можно использовать как основу для разработки собственной игры.

## 🚀 Возможности
- Управление персонажем от первого лица
- Стрельба, перезарядка, переключение оружия
- Движение и физика персонажа
- Интерактивная демо сцена с различными мишенями
- Поддержка ввода с клавиатуры и геймпада🎮
- Базовая система столкновений
- Использование  паттерна **Finite State Machine** для логики оружия

## 📚 Используемые библиотеки
- **UniTask** - для асинхронных операций (включая реализацию FSM паттерна для состояний оружия и его механик)
- **Animancer** - управление анимацией с возможностью ожидания завершения с помощью UniTask
- **DoTween** - анимации и интерактивные объекты
- **Zenject** - внедрение зависимостей, фабрики и сигналы

## 📂 Структура проекта
- **Assets/** - Основные игровые ресурсы (модели, текстуры, анимации)
- **Packages/** - Пакеты зависимостей проекта
- **ProjectSettings/** - Конфигурация проекта

## 🛠️ Установка и запуск
1. Склонируйте репозиторий:
   ```sh
   git clone https://github.com/zer0gie/fps-base.git
   ```
2. Откройте проект в Unity (рекомендуемая версия Unity 6+).
3. Установите зависимости через Unity Package Manager.
4. Запустите сцену с основными игровыми механиками.
5. Для создания нового оружия поместите на подготовленную модель скрипт `Weapon.Weapon` и отконфигурируйте его.

## 📝 Лицензия
Проект распространяется под лицензией [MIT](LICENSE.md).

## 📧 Обратная связь
Если у вас есть предложения или вы нашли баг, создайте issue в репозитории.

---
***

### FPS Base

FPS Base is a basic template for developing FPS games in Unity.

## 📌 Description

This repository contains fundamental components for creating a first-person shooter (FPS).
The project provides a ready-made architecture and core mechanics that can be used as a foundation for developing your own game.

## 🚀 Features

First-person character control

Shooting, reloading, and weapon switching

Character movement and physics

Interactive demo scene with various targets

Support for keyboard and gamepad input 🎮

Basic collision system

Utilization of the Finite State Machine pattern for weapon logic

## 📚 Libraries Used

UniTask - for asynchronous operations (including FSM pattern implementation for weapon states and mechanics)

Animancer - animation management with the ability to await completion using UniTask

DoTween - animations and interactive objects

Zenject - dependency injection, factories, and signals

## 📂 Project Structure

Assets/ - Main game assets (models, textures, animations)

Packages/ - Project dependency packages

ProjectSettings/ - Project configuration

## 🛠️ Installation and Setup

Clone the repository:

git clone https://github.com/zer0gie/fps-base.git

Open the project in Unity (recommended version: Unity 6+).

Install dependencies via Unity Package Manager.

Run the scene with core game mechanics.

To create a new weapon, attach the Weapon.Weapon script to a prepared model and configure it.

## 📝 License

This project is distributed under the MIT license.

## 📧 Feedback

If you have suggestions or found a bug, create an issue in the repository.
