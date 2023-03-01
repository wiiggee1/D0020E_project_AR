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
            
            const {angle_up, coord_x} = results[0];
            const test = {
                angle_up,
                coord_x
            }

            res.render('dbtest', { test });
        });
    })
})

app.get('/walkthroughs', (req,res) => {
    res.render("walkthroughs")
});

app.get('/courses', (req,res) => {
    res.render("courses");
});

app.listen(port, () => console.log("Listening on port " + port));
