import debug = require('debug');
import express = require('express');
import path = require('path');

import routes from './routes/index';
import users from './routes/user';
import allBooksHack from './routes/allBooksHack';
import allBooksMongo from './routes/allBooksMongo';
import addDummyBook from './routes/addDummyBook';
import allBooks from './routes/allBooks';
import allBooksByMonthMongo from './routes/allBooksByMonthMongo'

var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'pug');

app.use(express.static(path.join(__dirname, 'public')));

app.use('/', routes);
app.use('/users', users);
app.use('/allBooksHack', allBooksHack);
app.use('/allBooksMongo', allBooksMongo);
app.use('/addDummyBook', addDummyBook);
app.use('/allBooks', allBooks);
app.use('/allBooksByMonthMongo', allBooksByMonthMongo);

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
    app.use((err: any, req, res, next) => {
        res.status(err['status'] || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use((err: any, req, res, next) => {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});

app.set('port', process.env.PORT || 3000);

var server = app.listen(app.get('port'), function () {
  debug('JMCR Express server listening on port ' + server.address().port);
  console.log('JMCR Express server listening on port ' + server.address().port);
});
