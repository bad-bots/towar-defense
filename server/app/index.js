if(process.env.NODE_ENV !== 'production') require('../config/env.local');
const express = require('express');
const logger = require('morgan');
const path = require('path');
const cors = require('cors');
const compression = require('compression');

const { ResponseMessage } = require('./utils');

const app = express();

const createApp = () => {
  // logging middleware
  app.use(logger('dev'));

  // body parsing middleware
  app.use(express.json());
  app.use(express.urlencoded({ extended: true }));
  app.use(cors());

  // static file-serving middleware
  app.use('/public', express.static(path.join(__dirname, '..', 'public')));

  // error handling endware
  app.use((err, req, res, next) => {
    res.status(err.status || 500).send(new ResponseMessage(null, err));
  });
};

createApp();

module.exports = app;
