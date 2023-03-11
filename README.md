# Gamification of evacuation drills
This project is made during the third-year project course for computer engineering students at Luleå University of Technology. The course aims to develop students within software development and prepare for practices within the industry.

## Project description 
For our project an augmented reality (AR) game was created with the aim to generate research data on flow patterns in emergency situations. The game scenes an unguided fire evacuation drill with time pressure, a fire alarm bell ringing, additional obstacles such as fire and smoke. These features were added to make the evacuation drill as realistic and stressful as possible to ensure that the data collected also reflects the reality of an evacuation drill.

A website was created to visualize the data collected. See section [Website](#website)

## Installation: 
The game is playable on Android as for now. To install the game….. **Add detailed instructions on how to install the game**

## Usage: 
Instructions on how to use the game, including any special features or controls.

## Features:
A list of features of the game, including any unique or innovative aspects.

## Technologies: 
Platforms used to develop the game: **(Add more)**

* Unity
* AR Core
* AR Foundation

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
* Displaying all obstacle courses, chosen by selecting a value of the starting point from dropdown
* Upon clicking LTU's logo:
 * Displaying a map of Luleå with zoom in to campus
 * Highlighting F house by rendering a new polygon
* Upon clicking the start button:
 * Timer starts counting down
 * The georeferenced game model is rendered
