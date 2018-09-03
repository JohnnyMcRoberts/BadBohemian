/*
 * GET home page.
 */
import express = require('express');
const router = express.Router();

router.get('/', (req: express.Request, res: express.Response) => {
  console.log('router.get / ');
    res.render('index', { title: 'Express (Not the jungle or Terrordome)' });
});

export default router;