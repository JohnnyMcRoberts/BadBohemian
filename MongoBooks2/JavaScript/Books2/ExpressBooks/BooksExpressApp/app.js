"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const debug = require("debug");
const express = require("express");
const path = require("path");
const index_1 = require("./routes/index");
const user_1 = require("./routes/user");
const allBooksHack_1 = require("./routes/allBooksHack");
const allBooksMongo_1 = require("./routes/allBooksMongo");
const addDummyBook_1 = require("./routes/addDummyBook");
const allBooks_1 = require("./routes/allBooks");
const allBooksByMonthMongo_1 = require("./routes/allBooksByMonthMongo");
const allBooksByMonth_1 = require("./routes/allBooksByMonth");
const allAuthorsMongo_1 = require("./routes/allAuthorsMongo");
const authors_1 = require("./routes/authors");
var app = express();
// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'pug');
app.use(express.static(path.join(__dirname, 'public')));
app.use('/', index_1.default);
app.use('/users', user_1.default);
app.use('/allBooksHack', allBooksHack_1.default);
app.use('/allBooksMongo', allBooksMongo_1.default);
app.use('/addDummyBook', addDummyBook_1.default);
app.use('/allBooks', allBooks_1.default);
app.use('/allBooksByMonthMongo', allBooksByMonthMongo_1.default);
app.use('/allBooksByMonth', allBooksByMonth_1.default);
app.use('/allAuthorsMongo', allAuthorsMongo_1.default);
app.use('/authors', authors_1.default);
// catch 404 and forward to error handler
app.use(function (req, res, next) {
    var err = new Error('Not Found');
    err['status'] = 404;
    next(err);
});
// error handlers
// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use((err, req, res, next) => {
        res.status(err['status'] || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}
// production error handler
// no stacktraces leaked to user
app.use((err, req, res, next) => {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});
app.set('port', process.env.PORT || 9000);
var server = app.listen(app.get('port'), function () {
    debug('JMCR Express server listening on port ' + server.address().port);
    console.log('JMCR Express server listening on port ' + server.address().port);
});
//# sourceMappingURL=app.js.map