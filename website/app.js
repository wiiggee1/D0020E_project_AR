const express = require("express");
const app = express();
const port = 5000;
const database = require('./db.js');

app.set("views", "./views");
app.set("view engine", "pug");

app.use(express.static(__dirname + '/views'));
app.use('/js', express.static('js'));
app.use('/css', express.static('css'));

app.get('/', (req,res) => {
    res.render("index")
});

app.get('/db', (req, res) => {
    database.SSHConnection().then(function(connection){
        connection.query('select * from game_data_mockup', function(error, results, fields){
            if (error)
            {
                console.log(error);
                return;
            }

            res.send(results);
        });
    })
})
/*
app.get('/db/example', (req, res) => {
    database.SSHConnection().then(function(connection){
        connection.query('select * from game_data_mockup', function(error, results, fields){
            if (error)
            {
                console.log(error);
                return;
            }
            
            const {angle_up, coord_x} = results[0];
            const test = {
                angle_up,
                coord_x
            }

            res.render('dbtest', { test });
        });
    })
})*/

app.get('/walkthroughs', (req, res) => {
	database.SSHConnection().then(function (connection) {
		connection.query(
			'select distinct(obstacle_course) obstacle_course, start_position from course_data_mockup order by start_position',
			function (error, results, fields) {
				if (error) {
					console.log(error);
					return;
				}
				let courses = {};
				for (let i = 0; i < results.length; i++) {
					const { start_position, obstacle_course } = results[i];
					console.log(results[i]);
					if (typeof courses[start_position] === 'undefined') {
						courses[start_position] = [];
					}
					courses[start_position].push(obstacle_course);
				}
				console.log(courses);
				res.render('walkthroughs', { courses });
			}
		);
	});
});

app.get('/courses', (req,res) => {
    res.render("courses");
});

app.listen(port, () => console.log("Listening on port " + port));
