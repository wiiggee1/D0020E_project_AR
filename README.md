# Gamification of evacuation drills
This project is made during the third-year project course for computer engineering students at Luleå University of Technology. The course aims to develop students within software development and prepare for practices within the industry.

## Project description 
For our project an augmented reality (AR) game was created with the aim to generate research data on flow patterns in emergency situations. The game scenes an unguided fire evacuation drill with time pressure, a fire alarm bell ringing, additional obstacles such as fire and smoke. These features were added to make the evacuation drill as realistic and stressful as possible to ensure that the data collected also reflects the reality of an evacuation drill.

A website was created to visualize the data collected. See section [Website](#website)

## Game installation: 
The game is playable on Android as for now. To install the game follow these steps in Unity:
	File -> Build settings… -> Make sure that Android is selected in the Platform window -> Press Build and download the APK file to an android device -> Install the app from the devices file manager.

![Capture1](https://user-images.githubusercontent.com/22946017/225952504-fb35d1a9-7ab4-47c5-b72b-55caa2635670.PNG)

## Usage: 
To play the game properly, the player needs to know where the spawns are placed and go to the location the player chooses. The image below shows where those locations are as well as which direction the camera needs to be pointed for the obstacles to be in the correct locations.

![Capture](https://user-images.githubusercontent.com/22946017/225952201-5cf5d5bf-7144-457c-8fff-74ed5406320c.PNG)

When the game begins, players are presented with various obstacles such as chairs, tables and fire. The timer starts counting down, and is visible to the player at all times. The timer changes color from green to yellow to red as time runs out, indicating the urgency to find the safe zone. The objective of the game is to reach the safe zone before the time runs out. If a player crosses an AR obstacle, they will receive a notification in the game to avoid them.

## Features:

* Main menu
* AR gameplay
* Timer
* Obstacles such as fire and chairs
* Safe zones
* End screen

## Technologies: 
Platforms used to develop the game:

* Unity
* AR Core
* AR Foundation
* Blender
* Polycam

## Screenshots: 

## <a name="website"></a>Website

### Run the website:
In order to run the website you need to navigate into the folder "website" and type `npm start`, then proceed to open a browser and go to `localhost:5000`. If it does not work, double check if you have installed npm and nodemon.
Note that in order to make the website display any data stored in the database you must have the correct .env file. To achieve that, ask the developers to get it, or copy example.env into an empty .env file and fill it with authentication data. .Gitignore will ignore .env file in order to maintain security and limit access to the set up VM for anyone out there.

### Technologies:
 
* NodeJS
* PugJS
* Leaflet
* MapWarper

### Features:
- Displaying all obstacle courses, chosen by selecting a value of the starting point from dropdown
- Upon clicking LTU's logo:
  - Displaying a map of Luleå with zoom in to campus
  - Highlighting F house by rendering a new polygon
- Upon clicking the start button:
  - Timer starts counting down
  - The georeferenced game model is rendered
