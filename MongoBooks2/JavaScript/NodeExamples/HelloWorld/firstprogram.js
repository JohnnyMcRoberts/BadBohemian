// JavaScript source code
var http = require('http');
var url = require('url');
var fs = require('fs');
var uc = require('upper-case');
var dt = require('./firstModule');

var number = 0;


http.createServer(function (req, res) {
    res.writeHead(200, { 'Content-Type': 'text/html' });
    res.write(uc("Hello World!"));
    res.end();
}).listen(8080);


//http.createServer(function (req, res) {
//    var q = url.parse(req.url, true);
//    var filename = "." + q.pathname;
//    fs.readFile(filename, function (err, data) {
//        if (err) {
//            res.writeHead(404, { 'Content-Type': 'text/html' });
//            return res.end("404 Not Found");
//        }
//        res.writeHead(200, { 'Content-Type': 'text/html' });
//        res.write(data);
//        return res.end();
//    });
//}).listen(8080);



//http.createServer(function (req, res) {
//    fs.readFile('demoFile.html', function (err, data) {
//        res.writeHead(200, { 'Content-Type': 'text/html' });
//        res.write(data);
//        res.end();
//    });
//}).listen(8080);


//http.createServer(function (req, res) {
//    res.writeHead(200, { 'Content-Type': 'text/html' });
//    res.write("The date and time are currently: " + dt.myDateTime());
//    res.write(req.url);
//    var q = url.parse(req.url, true).query;
//    var txt = "<p> ====>>> the year and the month are "+ q.year + " " + q.month + "</p>";
//    res.end(txt);


//    var q = url.parse(req.url, true);

//    console.log('Some q.host = ' + q.host); //returns 'localhost:8080'
//    console.log('Some q.pathname =  ' + q.pathname); //returns '/default.htm'
//    console.log('Some q.search =  ' + q.search); //returns '?year=2017&month=february'

//    var qdata = q.query; //returns an object: { year: 2017, month: 'february' }
//    console.log('Some qdata.month =  ' + qdata.month); //returns 'february'

//    number++;

//    console.log('Some logging at ' + dt.myDateTime() + ' The result is displayed in the Command Line Interface: this is go number ' + number);

//    console.log("\n the txt is " + txt);


//}).listen(8080);
