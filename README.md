# TowAR Defense
## An Augmented Reality 2-player game implementation of Tower Defense. 

Get ready for the battle of a lifetime! TowAR Defense is a Medieval Tower Defense game in 3D Augmented Reality, so the game board, towers, and knights all show up in 3D, superimposed as a computer-generated image on your camera view of the real world, thus providing a composite view. This game is 2-player, so grab a friend and get started!

Define your game board (target) with the button on-screen to set up your game board, then step back and send out your units to attack your enemy's (friend's!) tower. You can select one of the three units:
1 - Knight
2 - Archer
3 - Phalanx

As the units reach the “enemy” tower, the tower’s health goes down, and the first to destroy the enemy's tower wins!

## Tech Stack:

This game is built in Unity, with front-end logic written with C#, with Unity’s scriptable objects to facilitate rapid prototyping of in-game assets. We developed our entire network stack from scratch. This networking is accomplished via a custom-built Node.js server deployed to AWS inside a Docker container.

Every layer of the networking was custom-architected from the ground up, including: 
- Network manager in Unity to facilitate communication between server and client.
- Game controller in Unity to manage the local state and actions in Unity.
- Individual Behavior and Controller scripts for different game objects.
- Socket.io on the back end to create rooms and allow 2-way communication between clients and server.
- Loki.js to be the “single source of truth” that manages our game state as an in-memory database.
- Vuforia for augmented reality technologies, including User Defined Targets to define the game board.
