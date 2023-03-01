const express = require("express");
const app = express();
const port = 5000;

app.set("views", "./views");
app.set("view engine", "pug");

app.use(express.static(__dirname + '/views'));
app.use('/js', express.static('js'));
app.use('/css', express.static('css'));

app.get('/', (req,res) => {
    res.render("index")
});

app.get('/walkthroughs', (req,res) => {
    res.render("walkthroughs")
});

app.get('/courses', (req,res) => {
    res.render("courses");
});

app.listen(port, () => console.log("Listening on port " + port));
